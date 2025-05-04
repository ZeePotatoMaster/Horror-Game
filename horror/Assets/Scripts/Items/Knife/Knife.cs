 using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Knife : NetworkBehaviour
{
    [SerializeField] private float attackDelay = 0.2f;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float attackDamage = 25f;

    //[SerializeField] private GameObject hitEffect;

    private bool attacking = false;
    private bool canAttack = true;
    private int attackCount;
    private PlayerBase pb;
    private PlayerHealth ph;

    const string IDLE = "Idle";
    const string WALK = "Walk";
    const string ATTACK1 = "Attack 1";
    const string ATTACK2 = "Attack 2";
    private string currentAnimationState;

    [SerializeField] private Transform rightIKTarget, leftIKTarget, viewmodel, worldmodel;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) {
            viewmodel.gameObject.SetActive(false);
            return;
        }
        pb = this.transform.parent.GetComponent<PlayerBase>();
        ph = this.transform.parent.GetComponent<PlayerHealth>();
        viewmodel.SetParent(pb.playerCamera.transform, true);
        worldmodel.GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        if (pb.attacked) Attack();

        if (attackCount > 0 && !attacking) attackCount = 0;
        SetAnimations();
    }

    private void Attack()
    {
        if (!canAttack || attacking) return;

        Debug.Log("YAHA");

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackCollider), attackDelay);

        attacking = true;
        canAttack = false;

        if (attackCount == 0)
        {
            ChangeAnimationState(ATTACK1);
            attackCount++;
        }
        else 
        {
            ChangeAnimationState(ATTACK2);
            attackCount = 0;
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
        attacking = false;
    }

    private void AttackCollider()
    {
        RaycastHit hit;
        if (Physics.Raycast(pb.playerCamera.transform.position, new Vector3(pb.playerCamera.transform.forward.x, pb.playerCamera.transform.forward.y, pb.playerCamera.transform.forward.z), out hit))
        {
            if (hit.transform.tag != "Player" || hit.distance > 1) return;
            
            GameObject p = hit.transform.gameObject;

            p.GetComponent<PlayerHealth>().TryDamageServerRpc(attackDamage);

            if (attackCount == 0) SetKnockback(p, -1, 1, 45);
            if (attackCount == 1) SetKnockback(p, 1, -1, 45);
        }
    }

    private void SetAnimations()
    {
        pb.RHandTarget.SetPositionAndRotation(rightIKTarget.position, rightIKTarget.rotation);
        pb.LHandTarget.SetPositionAndRotation(leftIKTarget.position, leftIKTarget.rotation);

        if (attacking) return;
        
        if (Mathf.Abs(pb.moveDirection.x) > 0.1f || Mathf.Abs(pb.moveDirection.z) > 0.1f) ChangeAnimationState(WALK);
        else ChangeAnimationState(IDLE);
    }

    private void SetKnockback(GameObject p, int defaultY, int defaultZ, float intensity)
    {
        Vector3 localDirection = p.transform.InverseTransformPoint(this.transform.position);
        PlayerBase pb = p.GetComponent<PlayerBase>();

        ulong id = p.GetComponent<NetworkObject>().OwnerClientId;
        
        if (localDirection.z > 0 && Mathf.Abs(localDirection.x) < 0.5) pb.CamKnockbackServerRpc(defaultY, defaultZ, intensity, id);
        else if (localDirection.z < 0 && Mathf.Abs(localDirection.x) < 0.5) pb.CamKnockbackServerRpc(-defaultY, -defaultZ, intensity, id);
        else if (localDirection.x < 0) pb.CamKnockbackServerRpc(1, -1, intensity, id);
        else if (localDirection.x > 0) pb.CamKnockbackServerRpc(-1, 1, intensity, id);
    }
    private void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return;

        Debug.Log(newState);
        currentAnimationState = newState;
        worldmodel.GetComponent<Animator>().CrossFadeInFixedTime(newState, 0.2f);
        viewmodel.GetComponent<Animator>().CrossFadeInFixedTime(newState + " View", 0.2f);
    }

    void OnEnable(){
        if (!IsOwner) return;
        viewmodel.gameObject.SetActive(true);
        if (!viewmodel.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(currentAnimationState + " View")) viewmodel.GetComponent<Animator>().CrossFadeInFixedTime(currentAnimationState + " View", 0.2f);
    }

    void OnDisable(){
        if (!IsOwner) return;
        viewmodel.gameObject.SetActive(false);
    }

    void OnNetworkDestroy ()
    {
        Destroy(viewmodel.gameObject);
    }
}
