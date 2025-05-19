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

    private bool started = false;
    
    void Awake()
    {
        base.Awake();
        uit = Instantiate(ui, GameObject.Find("Canvas").transform).GetComponent<TMP_Text>();
    }

    private void StartMinigame()
    {
        flashCam = Instantiate(flashCam, transform.position, transform.rotation);
        flashCam.Spawn();
        foreach (KeyValuePair<ulong, NetworkObject> e in TheOvergame.instance.elevators)
        {
            int i = Random.Range(0, elevatorSpawns.Count);

            FirstTeleportRpc(i, RpcTarget.Single(e.Key, RpcTargetUse.Temp));
            e.Value.transform.position = elevatorSpawns[i].position;

            Debug.Log("a ");
            elevatorSpawns.Remove(elevatorSpawns[i]);
            flashCam.GetComponent<FlashCamItem>().PickupItemRpc(RpcTarget.Single(e.Key, RpcTargetUse.Temp));

            alivePlayers++;
        }
        Debug.Log("alive players: " + alivePlayers);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void FirstTeleportRpc(int i, RpcParams rpcParams = default)
    {
        NetworkObject p = NetworkManager.Singleton.LocalClient.PlayerObject;
        p.GetComponent<CharacterController>().enabled = false;
        p.transform.position = elevatorSpawns[i].position + new Vector3(0, 2, 0);

        PlayerBase pb = p.GetComponent<PlayerBase>();
        pb.lockPosition = elevatorSpawns[i].position;
        pb.Invoke(nameof(pb.removeLockPosition), 2f);

        p.TryRemoveParent(true);
        p.GetComponent<CharacterController>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        uit.text = shotPaintings + " / " + totalPaintings.Value + " paintings";

        if (!IsServer) return;
        if (TheOvergame.instance.gameStarted && !started)
        {
            started = true;
            StartMinigame();
        }

        if (cookedPlayer != null) cookedPlayer.GetComponent<PlayerHealth>().TryDamageServerRpc(1);
        if (cookedPlayer == null && alivePlayers == 1) Invoke(nameof(EndGame), 5f);
        //if (shotPaintings == totalPaintings) 
    }

    public override void OnPlayerDeath(ulong id)
    {
        RespawnPlayerRpc(id, 5f);
    }

    [Rpc(SendTo.Server)]
    void RespawnPlayerRpc(ulong id, float delayTime)
    {
        if (alivePlayers == 1) return;
        StartCoroutine(RespawnPlayer(id ,delayTime));
    }

    IEnumerator RespawnPlayer(ulong id, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        NetworkObject newPlayer = Instantiate(playerPrefab);
        newPlayer.SpawnAsPlayerObject(id, true);
        flashCam.GetComponent<FlashCamItem>().PickupItemRpc(RpcTarget.Single(id, RpcTargetUse.Temp));

        Transform t = TheOvergame.instance.elevators[id].transform;
        TeleportRpc(t.position.x, t.position.y, t.position.z, RpcTarget.Single(id, RpcTargetUse.Temp));
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void TeleportRpc(float x, float y, float z, RpcParams rpcParams = default)
    {
        NetworkObject p = NetworkManager.Singleton.LocalClient.PlayerObject;
        p.GetComponent<CharacterController>().enabled = false;
        p.transform.position = new Vector3(x, y, z);
        p.GetComponent<CharacterController>().enabled = true;
    }

    public override void OnPlayerEnterElevator(ulong id)
    {
        if (shotPaintings == totalPaintings.Value) PlayerWinRpc(id);
    }

    [Rpc(SendTo.Server)]
    private void PlayerWinRpc(ulong id)
    {
        if (alivePlayers == 1) return;

        NetworkObject p = NetworkManager.Singleton.ConnectedClients[id].PlayerObject;
        if (winners.Contains(p)) return;
        Debug.Log("winning ahh");

        alivePlayers--;
        winners.Add(p);
        p.GetComponent<PlayerHealth>().invulnerable = true;
        p.gameObject.layer = nullLayer;

        if (alivePlayers == 1) {
            foreach (KeyValuePair<ulong, NetworkClient> i in NetworkManager.Singleton.ConnectedClients) {
                if (!winners.Contains(i.Value.PlayerObject) && i.Value.PlayerObject != null)
                {
                    EliminatePlayerRpc(RpcTarget.Single(i.Key, RpcTargetUse.Temp));
                    TheOvergame.instance.elevators[i.Key].Despawn(true);
                    TheOvergame.instance.elevators.Remove(i.Key);
                    cookedPlayer = i.Value.PlayerObject;
                }
            }
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void EliminatePlayerRpc(RpcParams rpcParams = default)
    {
        Instantiate(cookedText, GameObject.Find("Canvas").transform);
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
