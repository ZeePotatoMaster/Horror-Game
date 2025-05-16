using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Bomber : NetworkBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;

    //patrolling
    [SerializeField] private Vector3 walkPoint;
    private bool walkPointSet;
    [SerializeField] private float walkPointRange;

    //attacking
    [SerializeField] private float despawnTimeAfterAttack;
    private bool alreadyAttacked;

    //states
    [SerializeField] private float sightRange, attackRange;
    private bool playerInSightRange, playerInAttackRange;

    [SerializeField] private NetworkObject bomb;

    //find player
    private float nearestDist = 100;
    private Collider nearestPlayer;
    private float tick = 0;
    [SerializeField] private float timeBetweenChecks = 0.2f;

    private Animator animator;

    public override void OnNetworkSpawn()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsServer) return;
        if (alreadyAttacked) return;

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) {
            FindPlayer();
            ChasePlayer();
        }
        if (playerInSightRange && playerInAttackRange) {
            FindPlayer();
            AttackPlayer();
        }
        //Debug.Log(invisTick + " invise");
    }

    private void FindPlayer()
    {
        if (tick != 0) {
            tick = Mathf.Clamp(tick -= Time.deltaTime, 0f, timeBetweenChecks);
            Debug.Log(tick);
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sightRange + 5, whatIsPlayer);
        Debug.Log(hitColliders);
        foreach (var hitCollider in hitColliders)
        {
            Vector3 distance = transform.position - hitCollider.transform.position;
            float sqr = distance.sqrMagnitude;

            if (sqr < nearestDist) {
                nearestDist = sqr;
                nearestPlayer = hitCollider;
            }
        }
        if (nearestPlayer != null) player = nearestPlayer.GetComponent<Transform>();

        tick = timeBetweenChecks;
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector4 distanceToWalkPoint = transform.position - walkPoint;

        //walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        if (player == null) return;
        agent.SetDestination(player.position);
    }

    void AttackPlayer()
    {
        //make sure enemy dont move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            animator.Play("atttack");

            NetworkObject bombie = Instantiate(bomb, this.transform.position, this.transform.rotation);
            bombie.Spawn(true);

            bombie.GetComponent<Rigidbody>().AddForce(this.transform.forward * 5, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(Despawn), despawnTimeAfterAttack);
        }
    }

    private void Despawn()
    {
        this.GetComponent<NetworkObject>().Despawn(true);
    }
}
