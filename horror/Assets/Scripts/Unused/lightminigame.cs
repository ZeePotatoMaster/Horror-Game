using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.VisualScripting;

public class lightminigame : Interactable
{

    [SerializeField] private NetworkObject lightItem;
    [SerializeField] private GameObject teamObject;
    private int maxTeam;
    private List<NetworkObject> lights = new List<NetworkObject>();
    private List<lightteam> teams = new List<lightteam>();

    public override void FinishInteract(GameObject player) {
        
        if (!IsHost) return;
        int playerCount = NetworkManager.Singleton.ConnectedClientsList.Count;
        if (playerCount < 1 || playerCount % 2 == 1) return; //change to 4

        maxTeam = playerCount / 2;
        
        for (int i = 0; i < maxTeam; i++) {
            GameObject team = Instantiate(teamObject);
            team.GetComponent<lightteam>().SetTeamNumber(i);
            teams.Add(team.GetComponent<lightteam>());
        }

        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            NetworkObject worldItem = Instantiate(lightItem, client.PlayerObject.transform.position, Quaternion.identity);
            worldItem.SpawnWithOwnership(client.ClientId, true);
            GetMinigameClientRpc(worldItem, new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> {worldItem.OwnerClientId}}});
            lights.Add(worldItem);

            AssignTeam(client.ClientId);
        }
        
    }

    public void AssignTeam(ulong id)
    {
        List<lightteam> teamsClone = teams;

        bool assigned = false;

        while (!assigned) {
            int rand = UnityEngine.Random.Range(0, teamsClone.Count);
            List<ulong> team = teamsClone[rand].teammates;

            if (team.Count < 2) {
                team.Add(id);
                assigned = true;
            }
            else teamsClone.Remove(teamsClone[rand]);
        }
    }

    public void OnHit(NetworkObject shooter, NetworkObject shootee)
    {
        if (!IsHost) return;
        bool onSameTeam = false; 

        for (int i = 0; i < teams.Count; i++ )
        {
            if (teams[i].teammates.Contains(shooter.OwnerClientId) && teams[i].teammates.Contains(shootee.OwnerClientId)) onSameTeam = true;
        }

        if (onSameTeam && shootee.GetComponentInChildren<Light>().ready.Value == true) 
        {
            List<NetworkObject> cookedPlayers = new List<NetworkObject>();

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (client.ClientId == shooter.OwnerClientId || client.ClientId == shootee.OwnerClientId) return;
                cookedPlayers.Add(client.PlayerObject);
            }
            
            int whoDie = UnityEngine.Random.Range(0, cookedPlayers.Count);
            cookedPlayers[whoDie].GetComponent<PlayerHealth>().TryDamageServerRpc(100);
        }

        else
        {
            if (shootee.GetComponentInChildren<Light>().ready.Value == true) shootee.GetComponent<PlayerHealth>().TryDamageServerRpc(100);
            else shooter.GetComponent<PlayerHealth>().TryDamageServerRpc(100);
        }

        EndGame();
    }

    private void EndGame()
    {
        foreach (NetworkObject obj in lights)
        {
            ResetMinigameClientRpc(obj, new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> {obj.OwnerClientId}}});
            obj.Despawn();
        }
    }

    [ClientRpc]
    private void ResetMinigameClientRpc(NetworkObjectReference itemRef, ClientRpcParams clientRpcParams)
    {
       if (itemRef.TryGet(out NetworkObject item)) item.GetComponent<Light>().Reset();
    }

    [ClientRpc]
    private void GetMinigameClientRpc(NetworkObjectReference itemRef, ClientRpcParams clientRpcParams)
    {
        if (itemRef.TryGet(out NetworkObject item)) item.GetComponent<Light>().GetMinigameScript(this);
    }

    [ClientRpc]
    private void PickupItemClientRpc(NetworkObjectReference itemRef, ClientRpcParams clientRpcParams)
    {
        if (itemRef.TryGet(out NetworkObject item)) item.GetComponent<WorldItem>().FinishInteract(NetworkManager.LocalClient.PlayerObject.gameObject);
    }

}
