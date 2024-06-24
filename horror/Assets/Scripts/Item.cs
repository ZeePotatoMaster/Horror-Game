using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Item : NetworkBehaviour
{
    public float lootTime = 1f;
    [SerializeField] private NetworkObject item;
    private GameObject g_camera;
    private NetworkObject summonItem;
    private ulong id;

    public void OnPickup(){

        OnPickupServerRpc(NetworkManager.LocalClient.ClientId);
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnPickupServerRpc(ulong id) 
    {
        summonItem = Instantiate(item);
        summonItem.SpawnWithOwnership(id);
        g_camera = NetworkManager.ConnectedClients[id].PlayerObject.gameObject;
        summonItem.transform.SetParent(g_camera.transform, false);
        this.GetComponent<NetworkObject>().Despawn(true);
    }
}
