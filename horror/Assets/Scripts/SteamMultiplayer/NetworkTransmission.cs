using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkTransmission : NetworkBehaviour
{
    public static NetworkTransmission instance;
 
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddMeToDictionaryServerRPC(ulong steamId, string steamName, ulong clientId)
    {
        GameManager.instance.AddPlayerToDictionary(clientId, steamName, steamId);
        GameManager.instance.UpdateClients();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveMeFromDictionaryServerRPC(ulong steamId)
    {
        RemovePlayerFromDictionaryClientRPC(steamId);
    }

    [ClientRpc]
    public void RemovePlayerFromDictionaryClientRPC(ulong steamId)
    {
        GameManager.instance.RemovePlayerFromDictionary(steamId);
    }

    [ClientRpc]
    public void UpdateClientsPlayerInfoClientRPC(ulong steamId, string steamName, ulong clientId)
    {
        GameManager.instance.AddPlayerToDictionary(clientId, steamName, steamId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void IsClientReadyServerRPC(bool ready, ulong clientId)
    {
        ClientMaybeReadyClientRPC(ready, clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartGameServerRPC()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    [ClientRpc]
    public void ClientMaybeReadyClientRPC(bool ready, ulong clientId)
    {
        foreach (KeyValuePair<ulong, GameObject> player in GameManager.instance.playerInfo)
        {
            if (player.Key == clientId)
            {
                player.Value.GetComponent<PlayerInfo>().isready = ready;
                player.Value.GetComponent<PlayerInfo>().readyImage.SetActive(true);
                if (NetworkManager.Singleton.IsHost)
                {
                    Debug.Log(GameManager.instance.CheckIfPlayersAreReady());
                }
            }
        }
    }
}
