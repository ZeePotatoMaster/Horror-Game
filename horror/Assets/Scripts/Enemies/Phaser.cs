using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Phaser : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    private Transform player;
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;

    //patrolling
    [SerializeField] private Vector3 walkPoint;
    private bool walkPointSet;
    [SerializeField] private float walkPointRange;

    //attacking
    [SerializeField] private float timeBetweenAttacks;
    bool alreadyAttacked;

    //states
    [SerializeField] private float sightRange, attackRange;
    private bool playerInSightRange, playerInAttackRange;

    [SerializeField] private float damage = 20;

    //find player
    private float nearestDist = 100;
    private Collider nearestPlayer;

    //opacity
    private float opacityTimer = 1f;
    private bool flip = false;
    [SerializeField] private float changeMultiplier;
    private bool keepPhasing = true;
    [SerializeField] private float invisTime;

    public override void OnNetworkSpawn()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!IsServer) return;
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

        this.GetComponent<MeshRenderer>().material.color = new Color(1f, 1f, 1f, opacityTimer);

        float change = flip ? Time.deltaTime : -Time.deltaTime;
        if (keepPhasing) opacityTimer = Mathf.Clamp(opacityTimer + change * changeMultiplier, 0f, 1f);

        if (opacityTimer == 0f) {
            flip = true;
            keepPhasing = false;
            Invoke(nameof(ResetPhasing), invisTime);
        }
        if (opacityTimer == 1f) {
            flip = false;
        }
    }

    private void ResetPhasing()
    {
        keepPhasing = true;
    }

    private void FindPlayer()
    {
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
            RaycastHit attack;
            if (Physics.Raycast(transform.position, transform.forward, out attack))
            {
                float dist = Vector3.Distance(attack.transform.position, transform.position);
                if (dist <= 2)
                {
                    player.GetComponent<PlayerHealth>().TryDamageServerRpc(damage);
                }
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}
