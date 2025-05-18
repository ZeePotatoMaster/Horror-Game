using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HammerItem : WorldItem
{
    [SerializeField] private float decayTime = 5f;

    public override void SetupItem(NetworkObject item)
    {
        slotItem.SetDecay(decayTime, 0f, true);
    }

    public override void SetupWorldItem(NetworkObject item)
    {
        DestroySelfServerRpc();
    }

    [ServerRpc]
    private void DestroySelfServerRpc()
    {
        this.GetComponent<NetworkObject>().Despawn(true);
    }
}
