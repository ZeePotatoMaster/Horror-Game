using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Curse : NetworkBehaviour
{
    [HideInInspector] public bool activated = false;
    public float chargeTime = 1f;
    [HideInInspector] public float currentCharge = 0f;
    private float cost;

    private void Update()
    {
        if (!activated) return;

        currentCharge += Time.deltaTime;

        if (currentCharge >= chargeTime) {
            Debug.Log("curse finished charging");
            OnChargeUp();
            EndCasting();
        }
    }

    public void SetCost(float pCost)
    {
        cost = pCost;
    }

    public virtual void OnChargeUp()
    {
        
    }

    public virtual void OnActivate(GameObject player)
    {
        RoleClass rc = player.GetComponent<RoleClass>();
        if (rc.curseEnergy.Value < cost) return;
        
        RemoveCurseEnergyServerRpc(this.gameObject.GetComponent<NetworkObject>().OwnerClientId, cost);
        activated = true;
    }

    public virtual void EndCasting()
    {
        currentCharge = 0f;
        activated = false;
    }

    [ServerRpc]
    private void RemoveCurseEnergyServerRpc(ulong id, float cost)
    {  
        Debug.Log(NetworkManager);
        NetworkManager.ConnectedClients[id].PlayerObject.GetComponent<RoleClass>().curseEnergy.Value -= cost;
    }
}
