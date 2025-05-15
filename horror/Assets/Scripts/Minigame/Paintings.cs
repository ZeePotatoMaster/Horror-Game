using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Paintings : MinigameManager
{
    [SerializeField] private NetworkObject playerPrefab;

    [HideInInspector] public NetworkVariable<int> totalPaintings = new NetworkVariable<int>(1);
    [HideInInspector] public int shotPaintings = 0;
    [SerializeField] private GameObject ui;
    private TMP_Text uit;
    private int alivePlayers = 0;
    private List<NetworkObject> winners = new List<NetworkObject>();
    [SerializeField] private GameObject cookedText;
    private NetworkObject cookedPlayer;

    [SerializeField] private LayerMask nullLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private NetworkObject flashCam;

    [SerializeField] private List<Transform> elevatorSpawns;

    void Awake()
    {
        base.Awake();
        uit = Instantiate(ui, GameObject.Find("Canvas").transform).GetComponent<TMP_Text>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        foreach (KeyValuePair<ulong, NetworkObject> e in TheOvergame.instance.elevators) {
            int i = Random.Range(0, elevatorSpawns.Count);

            NetworkObject p = NetworkManager.Singleton.ConnectedClients[e.Value.GetComponent<Elevator>().ownerid].PlayerObject;
            p.GetComponent<CharacterController>().enabled = false;
            p.transform.position = elevatorSpawns[i].position + p.transform.localPosition;
            p.TryRemoveParent();

            e.Value.transform.position = elevatorSpawns[i].position;
            p.GetComponent<CharacterController>().enabled = true;

            Debug.Log("a ");
            elevatorSpawns.Remove(elevatorSpawns[i]);
            flashCam.Spawn();
            PickupItemRpc(flashCam, RpcTarget.Single(e.Key, RpcTargetUse.Temp));

            alivePlayers++;
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void PickupItemRpc(NetworkObjectReference itemRef, RpcParams rpcParams = default)
    {
        if (itemRef.TryGet(out NetworkObject item)) item.GetComponent<WorldItem>().FinishInteract(NetworkManager.LocalClient.PlayerObject.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        uit.text = shotPaintings + " / " + totalPaintings.Value + " paintings";
        
        if (cookedPlayer != null) cookedPlayer.GetComponent<PlayerHealth>().TryDamageServerRpc(1);
        //if (shotPaintings == totalPaintings) 
    }

    public override void OnPlayerDeath(ulong id)
    {
        if (alivePlayers == 1) return;
        RespawnPlayerRpc(id, 5f);
    }

    [Rpc(SendTo.Server)]
    void RespawnPlayerRpc(ulong id, float delayTime)
    {
        StartCoroutine(RespawnPlayer(id ,delayTime));
    }

    IEnumerator RespawnPlayer(ulong id, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        NetworkObject newPlayer = Instantiate(playerPrefab);
        newPlayer.SpawnAsPlayerObject(id, true);
        PickupItemRpc(flashCam, RpcTarget.Single(id, RpcTargetUse.Temp));
    }

    public override void OnPlayerEnterElevator(ulong id)
    {
        if (alivePlayers == 1) return;
        if (shotPaintings == totalPaintings.Value) PlayerWinRpc(id);
    }

    [Rpc(SendTo.Server)]
    private void PlayerWinRpc(ulong id)
    {
        alivePlayers--;
        NetworkObject p = NetworkManager.Singleton.ConnectedClients[id].PlayerObject;
        winners.Add(p);
        p.GetComponent<PlayerHealth>().invulnerable = true;
        p.gameObject.layer = nullLayer;

        if (alivePlayers == 1) {
            foreach (KeyValuePair<ulong, NetworkClient> i in NetworkManager.Singleton.ConnectedClients) {
                if (!winners.Contains(i.Value.PlayerObject)) {
                    EliminatePlayerRpc(RpcTarget.Single(i.Key, RpcTargetUse.Temp));
                    Destroy(TheOvergame.instance.elevators[i.Key]);
                    TheOvergame.instance.elevators.Remove(i.Key);
                }
            }
            EndGame();
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void EliminatePlayerRpc(RpcParams rpcParams = default)
    {
        Instantiate(cookedText, GameObject.Find("Canvas").transform);
        cookedPlayer = NetworkManager.Singleton.LocalClient.PlayerObject;
    }

    private void EndGame()
    {
        if (!IsServer) return;
        foreach (NetworkObject p in winners)
        {
            p.GetComponent<PlayerHealth>().invulnerable = false;
            p.gameObject.layer = playerLayer;
            p.TrySetParent(TheOvergame.instance.elevators[p.OwnerClientId].transform);
        }
        
        TheOvergame o = TheOvergame.instance;
        int rand = Random.Range(0, o.levels.Length);
        o.LoadLevel(o.levels[rand]);
    }
}
