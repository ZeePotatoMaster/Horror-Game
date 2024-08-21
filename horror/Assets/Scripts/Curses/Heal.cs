using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Heal : Curse
{
    public override void OnActivate(float cost, GameObject player)
    {
        base.OnActivate(cost, player);
        Debug.Log("healed up");
        HealServerRpc(this.OwnerClientId, 25f);
    }

    [ServerRpc]
    private void HealServerRpc(ulong id, float healthBack)
    {  
        NetworkManager.ConnectedClients[id].PlayerObject.GetComponent<PlayerHealth>().health.Value += healthBack;
    }
}
