using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DoorScript : Interactable
{
    public override void FinishInteract(GameObject player)
    {
        Animator door = this.transform.GetComponent<Animator>();

        if (door.GetCurrentAnimatorStateInfo(0).IsName("openidle")) {
            DoorServerRpc(this.transform.parent.GetComponent<NetworkObject>(), false);
        }
        if (door.GetCurrentAnimatorStateInfo(0).IsName("closeidle")) {
            DoorServerRpc(this.transform.parent.GetComponent<NetworkObject>(), true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DoorServerRpc(NetworkObjectReference reference, bool open) {
        if (reference.TryGet(out NetworkObject door)) {
            door.GetComponentInChildren<Animator>().SetBool("isOpen", open);
        }
    }
}
