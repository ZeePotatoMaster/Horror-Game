using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Security.Cryptography;

public class CupboardScript : Interactable
{
    public override void FinishInteract(GameObject player)
    {
        Animator cup = this.transform.GetComponent<Animator>();

        if (cup.GetCurrentAnimatorStateInfo(0).IsName("openidle")) {
            CupServerRpc(this.transform.GetComponent<NetworkObject>(), false);
        }
        if (cup.GetCurrentAnimatorStateInfo(0).IsName("closeidle")) {
            CupServerRpc(this.transform.GetComponent<NetworkObject>(), true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CupServerRpc(NetworkObjectReference reference, bool open) {
        if (reference.TryGet(out NetworkObject cup)) {
            cup.GetComponent<Animator>().SetBool("isOpen", open);
        }
    }
}
