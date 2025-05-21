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
        amount = item.GetComponent<Grenade>().GetGrenadeAmount();
        if (amount <= 0) DestroySelfServerRpc();
    }
    
    [ServerRpc]
    private void DestroySelfServerRpc()
    {
        GetComponent<NetworkObject>().Despawn(true);
    }
}
