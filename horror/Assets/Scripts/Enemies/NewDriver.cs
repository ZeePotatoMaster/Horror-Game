using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NewDriver : NetworkBehaviour
{
    [SerializeField] private LayerMask whatIsPlayer;

    //states
    [SerializeField] private float attackRange;
    private bool playerInAttackRange;

    //crash damage
    public float bumpDamage;
    private bool driveTime = false;
    [SerializeField] private GameObject bumpDetector;
    [SerializeField] private float startDriveTime;

    public override void OnNetworkSpawn()
    {
        Invoke(nameof(StartDrive), startDriveTime);
    }

    private void Update()
    {
        if (!IsServer) return;

        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (playerInAttackRange && driveTime) AttackPlayer();
        if (driveTime) transform.Translate(transform.forward / 7);
    }

    private void StartDrive()
    {
        driveTime = true;
        bumpDetector.SetActive(true);
    }

    private void AttackPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, whatIsPlayer);
        Debug.Log(hitColliders);
        foreach (var hitCollider in hitColliders)
        {
            hitCollider.GetComponent<Transform>().SetParent(this.transform);
            hitCollider.GetComponent<CharacterController>().enabled = false;
        }
    }
}
