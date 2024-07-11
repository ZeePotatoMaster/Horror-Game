using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WorldItem : NetworkBehaviour
{
    public float lootTime = 1f;
    [SerializeField] private InventoryItem item;

    public void OnPickup(InventoryManager inventoryManager){
        int canPickup = inventoryManager.AddItem(item);
        if (canPickup == -1) return;

        OnPickupServerRpc(NetworkManager.LocalClient.ClientId, canPickup);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnPickupServerRpc(ulong id, int slot) 
    {
        NetworkObject itemObject = Instantiate(item.itemObject);
        itemObject.SpawnWithOwnership(id);
        GameObject g_camera = NetworkManager.ConnectedClients[id].PlayerObject.gameObject;
        itemObject.transform.SetParent(g_camera.transform, false);
        AddItemClientRpc(itemObject, slot, new ClientRpcParams { Send = new ClientRpcSendParams {TargetClientIds = new List<ulong> {id}}});
        Debug.Log(itemObject + " was given to " + id);
        this.GetComponent<NetworkObject>().Despawn(true);
    }

    [ClientRpc]
    private void AddItemClientRpc(NetworkObjectReference reference, int slot, ClientRpcParams clientRpcParams)
    {
        if (reference.TryGet(out NetworkObject item)) NetworkManager.LocalClient.PlayerObject.gameObject.GetComponent<InventoryManager>().AddItemObject(slot, item);
    }
}
