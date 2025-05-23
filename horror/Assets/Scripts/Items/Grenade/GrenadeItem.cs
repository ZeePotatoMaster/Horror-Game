using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GrenadeItem : WorldItem
{
    public override void SetupItem(NetworkObject item)
    {
        item.GetComponent<Grenade>().SetSlotItem(slotItem);
    }

    public override void SetupWorldItem(NetworkObject item)
    {
        SetAmountRpc(item.GetComponent<Grenade>().GetGrenadeAmount());
        if (amount <= 0) DestroySelfServerRpc();
    }

    [Rpc(SendTo.Everyone)]
    void SetAmountRpc(int a)
    {
        amount = a;
    }
    
    [ServerRpc]
    private void DestroySelfServerRpc()
    {
        GetComponent<NetworkObject>().Despawn(true);
    }
}
