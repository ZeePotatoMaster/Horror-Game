using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class TheOvergame : NetworkBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] private NetworkObject elevatorPrefab;
    [SerializeField] private List<Transform> startingSpots;
    public string[] levels;
    [HideInInspector] public Dictionary<ulong, NetworkObject> elevators = new Dictionary<ulong, NetworkObject>();

    public static TheOvergame instance;
    private int clientsLoaded = 0;
    [HideInInspector] public bool gameStarted = false;

    void SC_OnSceneEvent(SceneEvent sceneEvent)
    {
        switch (sceneEvent.SceneEventType)
        {
            case SceneEventType.LoadComplete:
                {
                    ClientLoadedSceneRpc();
                    break;
                }
            case SceneEventType.UnloadComplete:
            case SceneEventType.LoadEventCompleted:
            case SceneEventType.UnloadEventCompleted:
                {
                    break;
                }
        }
    }

    [Rpc(SendTo.Server)]
    void ClientLoadedSceneRpc()
    {
        clientsLoaded++;
        if (clientsLoaded == NetworkManager.Singleton.ConnectedClientsList.Count)
        {
            gameStarted = true;
            clientsLoaded = 0;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        NetworkManager.Singleton.SceneManager.ActiveSceneSynchronizationEnabled = true;
        NetworkManager.Singleton.SceneManager.OnSceneEvent += SC_OnSceneEvent;
        NetworkManager.Singleton.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Single);

        /*foreach (ulong i in NetworkManager.Singleton.ConnectedClients.Keys) {
            NetworkObject newPlayer = Instantiate(playerPrefab);
            newPlayer.SpawnAsPlayerObject(i, true);
        }*/
        if (IsOwner) SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);

        instance = this;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong id)
    {
        NetworkObject newPlayer = Instantiate(playerPrefab);
        newPlayer.SpawnAsPlayerObject(id);
    }

    public void StartGame()
    {
        if (!IsServer) return;
        DontDestroyOnLoad(this.gameObject);

        foreach (ulong i in NetworkManager.Singleton.ConnectedClients.Keys)
        {
            Debug.Log(i);
            NetworkManager.Singleton.ConnectedClients[i].PlayerObject.Despawn(true);

            NetworkObject newPlayer = Instantiate(playerPrefab);
            newPlayer.SpawnAsPlayerObject(i, false);
            DontDestroyOnLoad(newPlayer);

            NetworkObject elevator = Instantiate(elevatorPrefab);
            elevator.Spawn(false);
            DontDestroyOnLoad(elevator);
            elevator.GetComponent<Elevator>().ownerid = i;

            int spot = Random.Range(0, startingSpots.Count);
            elevator.transform.position = startingSpots[spot].position;
            newPlayer.transform.position = startingSpots[spot].position + new Vector3(0, 1, 0);

            elevators.Add(i, elevator);
            //newPlayer.TrySetParent(elevator.transform);
        }
        foreach (KeyValuePair<ulong, NetworkObject> e in elevators) Debug.Log(e);

        int rand = Random.Range(0, levels.Length);
        LoadLevel(levels[rand]);
    }

    public void LoadLevel(string level)
    {
        /*if (!oldLevel.IsNullOrEmpty())
        {
            NetworkManager.Singleton.SceneManager.UnloadScene(SceneManager.GetSceneByName(oldLevel));
            while (SceneManager.GetSceneByName(oldLevel).isLoaded) yield return null;
        }
        var status = NetworkManager.SceneManager.LoadScene(level, LoadSceneMode.Additive);
        while (status != SceneEventProgressStatus.SceneNotLoaded) yield return null;*/
        NetworkManager.SceneManager.LoadScene(level, LoadSceneMode.Single);
    }
}
