using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public class Driver : NetworkBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;

    //patrolling
    [SerializeField] private Vector3 walkPoint;
    private bool walkPointSet;
    [SerializeField] private float walkPointRange;

    //states
    [SerializeField] private float attackRange;
    private bool playerInAttackRange;

    //crash damage
    public float bumpDamage;
    [SerializeField] private GameObject bumpDetector;
    [SerializeField] private float timeBetweenCharges;

    public override void OnNetworkSpawn()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (!IsServer) return;

        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (agent.enabled) Patroling();
        if (playerInAttackRange) AttackPlayer();

        if (!agent.enabled) transform.Translate(transform.forward / 10);
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector4 distanceToWalkPoint = transform.position - walkPoint;

        //walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f) {
            agent.SetDestination(transform.position);
            Invoke(nameof(WalkPointUnset), timeBetweenCharges);
        }
    }

    void WalkPointUnset()
    {
        walkPointSet = false;
    }

    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX * 3, transform.position.y, transform.position.z + randomZ * 3);

        if (Physics.Raycast(walkPoint, -transform.up, 10f, whatIsGround))
        {
            walkPointSet = true;
        }
    }

    void AttackPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, whatIsPlayer);
        Debug.Log(hitColliders);
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.GetComponent<Transform>().SetParent(this.transform);
            hitCollider.GetComponent<CharacterController>().enabled = false;
        }
        agent.enabled = false;
        bumpDetector.SetActive(true);
    }
}
