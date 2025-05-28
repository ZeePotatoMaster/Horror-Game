using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PrisonDoor : Interactable
{

    public override void FinishInteract(GameObject player)
    {
        OpenRpc(5f);
    }

    public override void OnInteract(GameObject player)
    {
        if (Prison.instance.GetComponent<Prison>().guards.Contains(player.GetComponent<NetworkObject>().OwnerClientId)) base.OnInteract(player);
        else
        {
            InventoryManager m = player.GetComponent<InventoryManager>();
            ItemInSlot s = m.GetItemInSlot(m.selectedSlot);
            if (s == null) return;
            if (s.item.itemId != 7) return;
            base.OnInteract(player);
        }
    }

    [Rpc(SendTo.Server)]
    public void OpenRpc(float timeOpen)
    {
        this.GetComponent<Animator>().SetBool("isOpen", true);

        if (timeOpen > -1f) Invoke(nameof(Close), timeOpen);
    }

    void Close()
    {
        if (!IsServer) return;
        this.GetComponent<Animator>().SetBool("isOpen", false);
    }
}
