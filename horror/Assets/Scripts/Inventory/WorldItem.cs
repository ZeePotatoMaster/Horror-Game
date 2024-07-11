using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WorldItem : NetworkBehaviour
{
    public float lootTime = 1f;
    [SerializeField] private InventoryItem item;
    private InventoryManager inventoryManager;
    private NetworkObject itemObject;

    private void Awake() {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }

    public void OnPickup(){
        int canPickup = inventoryManager.AddItem(item);
        if (canPickup == -1) return;

        OnPickupServerRpc(NetworkManager.LocalClient.ClientId);
        inventoryManager.AddItemObject(canPickup, itemObject);
        Debug.Log(itemObject);
        DestroyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnPickupServerRpc(ulong id) 
    {
        itemObject = Instantiate(item.itemObject);
        itemObject.SpawnWithOwnership(id);
        GameObject g_camera = NetworkManager.ConnectedClients[id].PlayerObject.gameObject;
        itemObject.transform.SetParent(g_camera.transform, false);
        itemObject.gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyServerRpc()
    {
        this.GetComponent<NetworkObject>().Despawn(true);
    }
}
