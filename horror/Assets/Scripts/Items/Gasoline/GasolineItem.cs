using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GasolineItem : WorldItem
{
    [SerializeField] private NetworkVariable<float> gas = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override void SetupItem(NetworkObject item)
    {
        Gasoline g = item.GetComponent<Gasoline>();
        g.currentGas = gas.Value;
    }

    public override void SetupWorldItem(NetworkObject item)
    {
        Gasoline g = item.GetComponent<Gasoline>();

        if (g.currentGas <= 0) DestroySelfServerRpc();

        gas.Value = g.currentGas;
    }

    [ServerRpc]
    private void DestroySelfServerRpc()
    {
        GetComponent<NetworkObject>().Despawn(true);
    }
}
