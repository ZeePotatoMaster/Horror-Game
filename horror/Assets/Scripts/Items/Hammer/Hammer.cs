 using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Hammer : NetworkBehaviour
{
    /*[SerializeField] private float attackDelay = 0.2f;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float attackDamage = 25f;

    //[SerializeField] private GameObject hitEffect;

    private bool attacking = false;
    private bool canAttack = true;
    private int attackCount;
    private PlayerBase pb;
    private PlayerHealth ph;
    [SerializeField] private GameObject mh;

    const string IDLE = "Idle";
    const string WALK = "Walk";
    const string ATTACK1 = "Attack 1";
    const string ATTACK2 = "Attack 2";
    const string BLOCK = "Block";
    string currentAnimationState;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;
        pb = this.transform.parent.GetComponent<PlayerBase>();
        ph = this.transform.parent.GetComponent<PlayerHealth>();
        mh.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        if (pb.attacked) Attack();

        if (pb.altAttacked) Block();

        else if (ph.isBlocking) {
            pb.currentSpeed += 5;
            ph.isBlocking = false;
        } 

        if (attackCount > 0 && !attacking) attackCount = 0;
        SetAnimations();
    }

    private void Attack()
    {
        if (!canAttack || attacking) return;

        Debug.Log("YAHA");

        mh.SetActive(true);

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
        mh.SetActive(false);
    }

    private void AttackCollider()
    {
        MeleeHitbox meleeHitbox = mh.GetComponent<MeleeHitbox>();

        foreach (GameObject p in meleeHitbox.targets)
        {
            if (p == null) {
                meleeHitbox.targets.Remove(p);
                return;
            }
            else if (p == this.gameObject) return;

            bool b = CheckIfBlockable(p);

            p.GetComponent<PlayerHealth>().TryDamageServerRpc(attackDamage, p.GetComponent<NetworkObject>().OwnerClientId, b);

            if (attackCount == 0) SetKnockback(p, -1, 1, 45, b);
            if (attackCount == 1) SetKnockback(p, 1, -1, 45, b);
        }
    }

    private void SetAnimations()
    {
        if (attacking) return;

        if (Mathf.Abs(pb.moveDirection.x) > 0.1f || Mathf.Abs(pb.moveDirection.z) > 0.1f) ChangeAnimationState(WALK);
        else ChangeAnimationState(IDLE);

        if (ph.isBlocking) ChangeAnimationState(BLOCK);
    }

    private void SetKnockback(GameObject p, int defaultY, int defaultZ, float intensity, bool blockable)
    {
        Vector3 localDirection = p.transform.InverseTransformPoint(this.transform.position);
        PlayerBase pb = p.GetComponent<PlayerBase>();

        ulong id = p.GetComponent<NetworkObject>().OwnerClientId;
        
        if (localDirection.z > 0 && Mathf.Abs(localDirection.x) < 0.5) pb.CamKnockbackServerRpc(defaultY, defaultZ, intensity, id, blockable);
        else if (localDirection.z < 0 && Mathf.Abs(localDirection.x) < 0.5) pb.CamKnockbackServerRpc(-defaultY, -defaultZ, intensity, id, blockable);
        else if (localDirection.x < 0) pb.CamKnockbackServerRpc(1, -1, intensity, id, blockable);
        else if (localDirection.x > 0) pb.CamKnockbackServerRpc(-1, 1, intensity, id, blockable);
    }

    private bool CheckIfBlockable(GameObject p)
    {
        bool canBlock = false;
        Vector3 localDirection = p.transform.InverseTransformPoint(this.transform.position);

        if (localDirection.z > 0 && Mathf.Abs(localDirection.x) < 0.5 && localDirection.y >= 0) canBlock = true;
        return canBlock;
    }

    private void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return;

        Debug.Log(newState);
        currentAnimationState = newState;
        this.GetComponent<Animator>().CrossFadeInFixedTime(newState, 0.2f);
    }

    private void Block()
    {
        if (attacking || ph.isBlocking) return;
        ph.isBlocking = true;
        pb.currentSpeed -= 5;
    }*/
}
