using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using TMPro;
using Steamworks.Data;
using Unity.Netcode;
using Netcode.Transports.Facepunch;

public class SteamManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyIdInputField;
    [SerializeField] private TextMeshProUGUI lobbyId;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject inLobbyMenu;

    void OnEnable()
    {
        SteamMatchmaking.OnLobbyCreated += LobbyCreated;
        SteamMatchmaking.OnLobbyEntered += LobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested += GameLobbyJoinRequested;
    }

    void OnDisable()
    {
        SteamMatchmaking.OnLobbyCreated -= LobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= LobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested -= GameLobbyJoinRequested;
    }

    private async void GameLobbyJoinRequested(Lobby lobby, SteamId id)
    {
        await lobby.Join();
    }

    private void LobbyEntered(Lobby lobby)
    {
        LobbySaver.instance.currentLobby = lobby;
        lobbyId.text = lobby.Id.ToString();
        CheckUI();
        Debug.Log("entering the matrix");
        if (NetworkManager.Singleton.IsHost) return;
        NetworkManager.Singleton.gameObject.GetComponent<FacepunchTransport>().targetSteamId = lobby.Owner.Id;
        NetworkManager.Singleton.StartClient();
    }

    private void LobbyCreated(Result result, Lobby lobby)
    {
        if (result == Result.OK)
        {
            lobby.SetPublic();
            lobby.SetJoinable(true);
            NetworkManager.Singleton.StartHost();
        }
    }

    public async void HostLobby()
    {
        Debug.Log("ligma?");
        await SteamMatchmaking.CreateLobbyAsync(4);
    }

    public async void JoinLobbyWithID()
    {
        ulong ID;
        if (!ulong.TryParse(lobbyIdInputField.text, out ID)) return;

        Lobby [] lobbies = await SteamMatchmaking.LobbyList.WithSlotsAvailable(1).RequestAsync();

        foreach (Lobby lobby in lobbies)
        {
            if (lobby.Id == ID)
            {
                await lobby.Join();
                return;
            }
        }
    }

    public void CopyID()
    {
        TextEditor textEditor = new TextEditor();
        textEditor.text = lobbyId.text;
        textEditor.SelectAll();
        textEditor.Copy();
    }

    public void LeaveLobby()
    {
        LobbySaver.instance.currentLobby?.Leave();
        LobbySaver.instance.currentLobby = null;
        NetworkManager.Singleton.Shutdown();
        CheckUI();
    }

    private void CheckUI()
    {
        if (LobbySaver.instance.currentLobby == null)
        {
            mainMenu.SetActive(true);
            inLobbyMenu.SetActive(false);
        }
        else 
        {
            mainMenu.SetActive(false);
            inLobbyMenu.SetActive(true);
        }
    }

    public void StartGameServer()
    {
        if (!NetworkManager.Singleton.IsHost) return;
        NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
