using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Curse : NetworkBehaviour
{
    public virtual void OnActivate(float cost, GameObject player)
    {
        RoleClass rc = player.GetComponent<RoleClass>();
        if (rc.curseEnergy.Value < cost) return;
        else RemoveCurseEnergyServerRpc(this.OwnerClientId, cost);
    }

    [ServerRpc]
    private void RemoveCurseEnergyServerRpc(ulong id, float cost)
    {  
        NetworkManager.ConnectedClients[id].PlayerObject.GetComponent<RoleClass>().curseEnergy.Value -= cost;
    }
}
