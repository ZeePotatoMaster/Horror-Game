using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
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

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        /*foreach (ulong i in NetworkManager.Singleton.ConnectedClients.Keys) {
            NetworkObject newPlayer = Instantiate(playerPrefab);
            newPlayer.SpawnAsPlayerObject(i, true);
        }*/
        instance = this;
    }

    public void StartGame()
    {
        if (!IsServer) return;
        DontDestroyOnLoad(this.gameObject);

        foreach (ulong i in NetworkManager.Singleton.ConnectedClients.Keys) {
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
            newPlayer.transform.position = startingSpots[spot].position + new Vector3 (0, 1, 0);

            elevators.Add(i, elevator);
            newPlayer.TrySetParent(elevator.transform);
        }
        foreach (KeyValuePair<ulong, NetworkObject> e in elevators) Debug.Log(e);

        int rand = Random.Range(0, levels.Length);
        LoadLevel(levels[rand]);
    }

    public void LoadLevel(string level)
    {
        if (!IsServer) return;
        NetworkManager.Singleton.SceneManager.LoadScene(level, LoadSceneMode.Single);
    }
}
