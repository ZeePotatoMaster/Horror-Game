using UnityEngine;
using Unity.Netcode;
using Steamworks;
using Steamworks.Data;
using Netcode.Transports.Facepunch;
using System;

public class GameNetworkManager : MonoBehaviour
{
    public static GameNetworkManager instance {get; private set; } = null;
    private FacepunchTransport transport = null;
    public Lobby? currentLobby {get; private set; } = null;
    public ulong hostId;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        transport = GetComponent<FacepunchTransport>();

        SteamMatchmaking.OnLobbyCreated += SteamMatchmaking_OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += SteamMatchmaking_OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += SteamMatchmaking_OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += SteamMatchmaking_OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite += SteamMatchmaking_OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated += SteamMatchmaking_OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested += SteamFriends_OnGameLobbyJoinRequested;
    }

    private async void SteamFriends_OnGameLobbyJoinRequested(Lobby lobby, SteamId id)
    {
         RoomEnter joinedLobby = await lobby.Join();
         if (joinedLobby != RoomEnter.Success) Debug.Log("failed to join lobby");
         else
         {
            Debug.Log("joined lobb");
            currentLobby = lobby;
            GameManager.instance.ConnectedAsClient();
         }
    }

    private void SteamMatchmaking_OnLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId id)
    {
         Debug.Log("lobbycreated");
    }

    private void SteamMatchmaking_OnLobbyInvite(Friend friend, Lobby lobby)
    {
         Debug.Log("invite from " + friend.Name);
    }

    private void SteamMatchmaking_OnLobbyMemberLeave(Lobby lobby, Friend friend)
    {
        Debug.Log("memb leave");
        NetworkTransmission.instance.RemoveMeFromDictionaryServerRPC(friend.Id);
    }

    private void SteamMatchmaking_OnLobbyMemberJoined(Lobby lobby, Friend friend)
    {
         Debug.Log("mbme join");
    }

    private void SteamMatchmaking_OnLobbyEntered(Lobby lobby)
    {
         if (NetworkManager.Singleton.IsHost) return;
         StartClient(currentLobby.Value.Owner.Id);
    }

    private void SteamMatchmaking_OnLobbyCreated(Result result, Lobby lobby)
    {
         if (result != Result.OK)
         {
            Debug.Log("didnt work lobby ceateat");
            return;
         }
         Debug.Log("lobby ceateat " + lobby.Owner.Name);
         lobby.SetPublic();
         lobby.SetJoinable(true);
         lobby.SetGameServer(lobby.Owner.Id);
         NetworkTransmission.instance.AddMeToDictionaryServerRPC(SteamClient.SteamId, SteamClient.Name, NetworkManager.Singleton.LocalClientId);
    }

    public async void StartHost(int maxMembers)
    {
        NetworkManager.Singleton.OnServerStarted += Singleton_OnServerStarted;
        NetworkManager.Singleton.StartHost();
        currentLobby = await SteamMatchmaking.CreateLobbyAsync(maxMembers);
    }

    public void StartClient(SteamId id)
    {
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;
        transport.targetSteamId = id;
        GameManager.instance.myClientId = id;
        if (NetworkManager.Singleton.StartClient()) Debug.Log("clinet started");
    }

    public void Disconnected()
    {
        currentLobby?.Leave();
        if (NetworkManager.Singleton == null) return;

        if (NetworkManager.Singleton.IsHost) NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
        else NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;

        NetworkManager.Singleton.Shutdown(true);
        GameManager.instance.Disconnected();
        Debug.Log("disconnected");
    }

    private void Singleton_OnClientConnectedCallback(ulong clientId)
    {
        NetworkTransmission.instance.AddMeToDictionaryServerRPC(SteamClient.SteamId, SteamClient.Name, clientId);
        GameManager.instance.myClientId = clientId;
        NetworkTransmission.instance.IsClientReadyServerRPC(false, clientId);
        Debug.Log("client connected " + SteamClient.Name + clientId);
    }

    private void Singleton_OnClientDisconnectCallback(ulong clientId)
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;
        if (clientId == 0) Disconnected();
    }

    private void Singleton_OnServerStarted()
    {
        Debug.Log("started host server");
        GameManager.instance.HostCreated();
    }

    private void OnApplicationQuit()
    {
        Disconnected();
    }

    private void OnDestroy()
    {
        SteamMatchmaking.OnLobbyCreated -= SteamMatchmaking_OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= SteamMatchmaking_OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= SteamMatchmaking_OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= SteamMatchmaking_OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite -= SteamMatchmaking_OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated -= SteamMatchmaking_OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested -= SteamFriends_OnGameLobbyJoinRequested;

        if (NetworkManager.Singleton == null) return;
        NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;
        NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
    }
}