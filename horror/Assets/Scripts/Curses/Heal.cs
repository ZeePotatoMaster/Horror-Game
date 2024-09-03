using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Heal : Curse
{

    private void Start()
    {
        chargeTime = 1f;
    }

    public override void OnChargeUp()
    {
        Debug.Log("healed up");
        HealServerRpc(this.OwnerClientId, 25f);
    }

    public override void OnActivate()
    {
        base.OnActivate();
        if (!activated) return;
        this.GetComponent<PlayerBase>().currentSpeed -= 4f;
    }

    public override void EndCasting()
    {
        base.EndCasting();
        this.GetComponent<PlayerBase>().currentSpeed += 4f;
    }

    [ServerRpc]
    private void HealServerRpc(ulong id, float healthBack)
    {  
        NetworkManager.ConnectedClients[id].PlayerObject.GetComponent<PlayerHealth>().health.Value += healthBack;
    }
}
