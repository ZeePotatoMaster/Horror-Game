using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FlashCamItem : WorldItem
{
    public override void SetupWorldItem(NetworkObject item)
    {
        DestroySelfServerRpc();
    }

    [ServerRpc]
    private void DestroySelfServerRpc()
    {
        GetComponent<NetworkObject>().Despawn(true);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void PickupItemRpc(RpcParams rpcParams = default)
    {
        GetComponent<WorldItem>().FinishInteract(NetworkManager.LocalClient.PlayerObject.gameObject);
    }
}
