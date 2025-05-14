using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class FakeDoor : Interactable
{
    [SerializeField] private float attackDelay;
    [SerializeField] private float attackSpeed;
    private Transform target;
    private bool attacking = false;
    [SerializeField] private Transform destination;

    public override void FinishInteract(GameObject player)
    {
        AnimateRpc(player.GetComponent<NetworkObject>().OwnerClientId);
    }

    [Rpc(SendTo.Server)]
    void AnimateRpc(ulong id)
    {
        Animator door = this.transform.GetComponent<Animator>();
        door.Play("attack");
        
        target = NetworkManager.Singleton.ConnectedClients[id].PlayerObject.transform;
        Debug.Log(NetworkManager.Singleton.ConnectedClients[id].PlayerObject.transform);
        Invoke(nameof(StartAttack), attackDelay);
    }

    void StartAttack()
    {
        attacking = true;
        target.GetComponent<CharacterController>().enabled = false;
    }

    void Update()
    {
        if (!IsServer) return;
        if (attacking) {
            target.position = Vector3.MoveTowards(target.position, destination.position, attackSpeed * Time.deltaTime);
            if (Vector3.Distance(target.position, destination.position) < 0.1) {
                target.GetComponent<PlayerHealth>().TryDamageServerRpc(100);
                attacking = false;
            } 
        }
    }
}
