 using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Hammer : NetworkBehaviour
{
    [SerializeField] private float attackDelay = 0.2f;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float attackDamage = 25f;

    //[SerializeField] private GameObject hitEffect;

    private bool attacking = false;
    private bool canAttack = true;
    private int attackCount;
    private PlayerBase pb;
    [SerializeField] private GameObject mh;

    const string IDLE = "Idle";
    const string WALK = "Walk";
    const string ATTACK1 = "Attack 1";
    const string ATTACK2 = "Attack 2";
    string currentAnimationState;
    
    // Start is called before the first frame update
    void Start()
    {
        pb = this.transform.parent.GetComponent<PlayerBase>();
        mh.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (pb.attacked) Attack();
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
            p.GetComponent<PlayerHealth>().DamageServerRpc(attackDamage);
            if (attackCount == 0) SetKnockback(p, -1, 1, 45);
            if (attackCount == 1) SetKnockback(p, 1, -1, 45);
        }
    }

    private void SetAnimations()
    {
        if (attacking) return;
        if (Mathf.Abs(pb.moveDirection.x) > 0.1f || Mathf.Abs(pb.moveDirection.z) > 0.1f) ChangeAnimationState(WALK);
        else ChangeAnimationState(IDLE);
    }

    private void SetKnockback(GameObject p, int defaultY, int defaultZ, float intensity)
    {
        Vector3 localDirection = p.transform.InverseTransformPoint(this.transform.position);
        PlayerBase pb = p.GetComponent<PlayerBase>();
        
        if (localDirection.x < 0 && Mathf.Abs(localDirection.z) < 1) pb.CamKnockbackServerRpc(1, -1, intensity, p.GetComponent<NetworkObject>().OwnerClientId);
        else if (localDirection.x > 0 && Mathf.Abs(localDirection.z) < 1) pb.CamKnockbackServerRpc(-1, 1, intensity, p.GetComponent<NetworkObject>().OwnerClientId);
        else if (localDirection.z >= 1) pb.CamKnockbackServerRpc(defaultY, defaultZ, intensity, p.GetComponent<NetworkObject>().OwnerClientId);
        else if (localDirection.z <= -1) pb.CamKnockbackServerRpc(-defaultY, -defaultZ, intensity, p.GetComponent<NetworkObject>().OwnerClientId);
    }

    private void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return;

        Debug.Log(newState);
        currentAnimationState = newState;
        this.GetComponent<Animator>().CrossFadeInFixedTime(newState, 0.2f);
    }
}
