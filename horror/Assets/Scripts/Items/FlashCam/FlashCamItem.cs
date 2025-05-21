using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FlashCamItem : WorldItem
{
    public override void SetupWorldItem(NetworkObject item)
    {
        if (item.transform.parent.GetComponent<PlayerHealth>().health.Value > 0) StartCoroutine(ReturnItem(item));
        else DestroySelfServerRpc();
    }

    IEnumerator ReturnItem(NetworkObject item)
    {
        while (item != null) yield return null;
        PickupItemRpc(RpcTarget.Single(NetworkManager.Singleton.LocalClientId, RpcTargetUse.Temp));
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
