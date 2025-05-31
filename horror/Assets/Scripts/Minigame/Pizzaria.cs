using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Pizzaria : MinigameManager
{
    private bool started = false;
    private bool gameOver = false;
    [SerializeField] private Camera deathCam;
    
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        DisableOtherPlayersRpc();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
        /*if (!started) // && TheOvergame.instance.gameStarted
        {
            started = true;
            StartMinigame();
        }*/
    }

    [Rpc(SendTo.ClientsAndHost)]
    void DisableOtherPlayersRpc()
    {
        foreach (ulong i in NetworkManager.Singleton.ConnectedClients.Keys)
        {
            if (i != NetworkManager.Singleton.LocalClientId) NetworkManager.Singleton.ConnectedClients[i].PlayerObject.gameObject.SetActive(false);
        }
    }

    public override void OnPlayerEnterElevator(ulong id)
    {
        throw new System.NotImplementedException();
    }

    public override void OnPlayerDeath(ulong id)
    {
        throw new System.NotImplementedException();
    }

    public void Kill(Transform animatronic)
    {
        if (gameOver) return;

        deathCam.enabled = true;
        animatronic.SetPositionAndRotation(deathCam.transform.position, deathCam.transform.rotation);
        //animatronic.GetComponent<Animator>().Play("Jumpscare");
    }
}
