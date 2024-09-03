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
    private MeleeHitbox mh;

    const string IDLE = "Idle";
    const string WALK = "Walk";
    const string ATTACK1 = "Attack 1";
    const string ATTACK2 = "Attack 2";
    string currentAnimationState;
    
    // Start is called before the first frame update
    void Start()
    {
        pb = this.transform.parent.GetComponent<PlayerBase>();
        mh = this.GetComponentInChildren<MeleeHitbox>();
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
        foreach (GameObject p in mh.targets)
        {
            if (p == null) {
                mh.targets.Remove(p);
                return;
            }
            p.GetComponent<PlayerHealth>().DamageServerRpc(attackDamage);
            p.GetComponent<PlayerBase>().
        }
    }

    private void SetAnimations()
    {
        if (attacking) return;
        if (Mathf.Abs(pb.moveDirection.x) > 0.1f || Mathf.Abs(pb.moveDirection.z) > 0.1f) ChangeAnimationState(WALK);
        else ChangeAnimationState(IDLE);
    }

    private void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return;

        Debug.Log(newState);
        currentAnimationState = newState;
        this.GetComponent<Animator>().CrossFadeInFixedTime(newState, 0.2f);
    }
}
