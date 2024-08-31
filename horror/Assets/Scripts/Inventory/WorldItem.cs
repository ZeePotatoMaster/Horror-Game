using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WorldItem : Interactable
{
    [SerializeField] private InventoryItem item;
    [HideInInspector] protected ItemInSlot slotItem;

    public override void FinishInteract(GameObject player){
        InventoryManager inventoryManager = player.GetComponent<InventoryManager>();
        
        int canPickup = inventoryManager.AddItem(item);
        if (canPickup == -1) return;

        slotItem = inventoryManager.GetItemInSlot(canPickup);
        OnPickupServerRpc(NetworkManager.LocalClientId, canPickup);
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
    }

    [ClientRpc]
    private void AddItemClientRpc(NetworkObjectReference reference, int slot, ClientRpcParams clientRpcParams)
    {
        if (reference.TryGet(out NetworkObject item)) {
            NetworkManager.LocalClient.PlayerObject.gameObject.GetComponent<InventoryManager>().AddItemObject(slot, item);
            SetupItem(item);
        }
    }

    virtual public void SetupWorldItem(NetworkObject item)
    {
        
    }

    virtual public void SetupItem(NetworkObject item)
    {

    }
}
