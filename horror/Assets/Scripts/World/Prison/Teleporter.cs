using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Teleporter : Interactable
{
    [SerializeField] private Transform teleportLocation;
    private bool on = false;
    [SerializeField] private NetworkObject playerPrefab;
    [HideInInspector] public List<int> correctCombination = new List<int>();
    public List<int> currentCombination;

    void Awake()
    {
        for (int i = 0; i < 2; i++)
        {
            int o = Random.Range(0, 2);
            correctCombination.Add(o);
        }
    }

    public override void OnInteract(GameObject player)
    {
        if (Prison.instance.GetComponent<Prison>().guards.Contains(player.GetComponent<NetworkObject>().OwnerClientId)) base.OnInteract(player);

        else
        {
            InventoryManager m = player.GetComponent<InventoryManager>();
            ItemInSlot s = m.GetItemInSlot(m.selectedSlot);

            if (s == null) return;
            if (s.item.itemId != 9) return;
            base.OnInteract(player);
        }
    }

    public override void FinishInteract(GameObject player)
    {
        ChooseTeleportRpc(player.GetComponent<NetworkObject>().OwnerClientId);
    }

    [Rpc(SendTo.Server)]
    void ChooseTeleportRpc(ulong id)
    {
        Prison prison = Prison.instance.GetComponent<Prison>();

        if (prison.prisoners.Contains(id) && prison.imprisoned.Count > 0)
        {
            int i = Random.Range(0, prison.imprisoned.Count - 1);
            TeleportPlayer(prison.imprisoned[i]);
            DestroyItemRpc(RpcTarget.Single(id, RpcTargetUse.Temp));
        }

        else if (prison.killedGuards.Count > 0 && on)
        {
            int i = Random.Range(0, prison.killedGuards.Count - 1);
            SpawnPlayer(prison.killedGuards[i]);
            TeleportPlayer(prison.killedGuards[i]);
            prison.killedGuards.Remove(prison.killedGuards[i]);
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    void DestroyItemRpc(RpcParams rpcParams = default)
    {
        InventoryManager m = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<InventoryManager>();
        m.DestroyItem(m.selectedSlot);
    }

    void SpawnPlayer(ulong id)
    {
        NetworkObject p = Instantiate(playerPrefab, transform.position, transform.rotation);
        p.SpawnAsPlayerObject(id, false);
    }

    void TeleportPlayer(ulong id)
    {
        NetworkObject p = NetworkManager.ConnectedClients[id].PlayerObject;

        p.GetComponent<CharacterController>().enabled = false;
        p.transform.position = teleportLocation.position;
        p.GetComponent<CharacterController>().enabled = true;
    }

    [Rpc(SendTo.Server)]
    public void ActivateTeleporterRpc()
    {
        on = true;
        //animations and stuff
    }
}
