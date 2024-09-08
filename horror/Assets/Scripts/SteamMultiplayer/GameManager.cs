using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private GameObject menu, lobbyMenu;
    public TMP_FontAsset defaultGlobalFont;
    public Sprite readySprite;
    public Sprite unreadySprite;
    public Camera mainCamera;
 
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    public bool connected;
    public bool inGame;
    public bool isHost;
    public ulong myClientId;
    public Dictionary<ulong, GameObject> playerInfo = new Dictionary<ulong, GameObject>();
    [SerializeField] private GameObject playerFieldBox, playerCardPrefab;
    [SerializeField] private GameObject readyButton, notReadyButton, startButton;

    public void HostCreated()
    {
        isHost = true;
        connected = true;
        CheckUI();
    }

    public void ConnectedAsClient()
    {
        isHost = false;
        connected = true;
        CheckUI();
    }

    public void Disconnected()
    {
        playerInfo.Clear();
        GameObject[] playerCards = GameObject.FindGameObjectsWithTag("PlayerCard");
        foreach (GameObject card in playerCards) Destroy(card);
        isHost = false;
        connected = false;
        CheckUI();
    }

    public void AddPlayerToDictionary(ulong clientId, string steamName, ulong steamId)
    {
        if (!playerInfo.ContainsKey(clientId))
        {
            PlayerInfo pi = Instantiate(playerCardPrefab, playerFieldBox.transform).GetComponent<PlayerInfo>();
            pi.updateFont(defaultGlobalFont);
            pi.steamId = steamId;
            pi.steamName = steamName;
            playerInfo.Add(clientId, pi.gameObject);
        }
    }

    public void UpdateClients()
    {
        foreach(KeyValuePair<ulong, GameObject> player in playerInfo)
        {
            ulong steamId = player.Value.GetComponent<PlayerInfo>().steamId;
            string steamName = player.Value.GetComponent<PlayerInfo>().steamName;
            ulong clientId = player.Key;

            NetworkTransmission.instance.UpdateClientsPlayerInfoClientRPC(steamId, steamName, clientId);
        }
    }

    public void RemovePlayerFromDictionary(ulong steamId)
    {
        GameObject value = null;
        ulong key = 100;
        foreach(KeyValuePair<ulong,GameObject> player in playerInfo)
        {
            if (player.Value.GetComponent<PlayerInfo>().steamId == steamId)
            {
                value = player.Value;
                key = player.Key;
            }
        }
        if (key != 100) playerInfo.Remove(key);
        if (value != null) Destroy(value);
    }

    public void ReadyButton(bool ready)
    {
        NetworkTransmission.instance.IsClientReadyServerRPC(ready, myClientId);
    }

    public bool CheckIfPlayersAreReady()
    {
        bool ready = false;
        foreach(KeyValuePair<ulong,GameObject> player in playerInfo)
        {
            if (!player.Value.GetComponent<PlayerInfo>().isready)
            {
                startButton.SetActive(false);
                return false;
            }
            else{
                startButton.SetActive(true);
                ready = true;
            }
        }
        return ready;
    }

    public void Quit()
    {
        
    }

    public void StartButton()
    {
        if (!isHost) return;
        NetworkTransmission.instance.StartGameServerRPC();
    }

    private void CheckUI()
    {
        if (!connected)
        {
            menu.SetActive(true);
            lobbyMenu.SetActive(false);
        }
        else 
        {
            menu.SetActive(false);
            lobbyMenu.SetActive(true);
        }
    }
}
