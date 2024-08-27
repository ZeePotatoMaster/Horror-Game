using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class EnergyGiver : Interactable
{
    [SerializeField] private float amount;
    public override void FinishInteract(GameObject player)
    {
        GiveEnergyServerRpc(NetworkManager.LocalClientId, amount);
    }

    [ServerRpc(RequireOwnership = false)]
    private void GiveEnergyServerRpc(ulong id, float amount) {
        NetworkManager.Singleton.ConnectedClients[id].PlayerObject.GetComponent<CurseManager>().curseEnergy.Value += amount;
    }
}
