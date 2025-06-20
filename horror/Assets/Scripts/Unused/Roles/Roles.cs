using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Collections;
using System.Linq;

public class Roles : Interactable
{
    [SerializeField] private List<RoleObject> goodRoles = new List<RoleObject>();
    [SerializeField] private List<RoleObject> badRoles = new List<RoleObject>();
    [SerializeField] private RoleObject defaultGood;
    [SerializeField] private RoleObject defaultBad;
    [SerializeField] private GameObject blankPlayer;

    [SerializeField] private GameObject menuPrefab;
    [SerializeField] private GameObject energyIconPrefab;

    [SerializeField] private GameObject defPlayer;
    [SerializeField] private RoleObject[] allRoles;
    [SerializeField] private string[] allRoleComponents;
    [SerializeField] private CurseObject[] allCurseComponents;


    public override void OnNetworkSpawn()
    {
        if (IsOwner) SpawnPlayerServerRpc(NetworkManager.LocalClientId);
    }

    public override void FinishInteract(GameObject player)
    {
        SetupRoles();
    }

    private void SetupRoles()
    {
        if (!IsHost) return;

        float playerCount = NetworkManager.Singleton.ConnectedClientsList.Count;
        int badCount = (int)Mathf.Floor(playerCount/2f);
        int goodCount = (int)Mathf.Ceil(playerCount/2f);

        Debug.Log("initial bad " + badCount);
        Debug.Log("initial good " + goodCount);


        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {   
            int coin = 0;
            if (badCount > 0 && goodCount > 0)  coin = UnityEngine.Random.Range(0, 2);
            else if (badCount > 0 && goodCount == 0) coin = 0;
            else if (badCount == 0 && goodCount > 0) coin = 1;
            
            RoleObject role;
            if (coin == 0) {
                if (badRoles.Count == 0) role = defaultBad;
                else role = badRoles[UnityEngine.Random.Range(0, badRoles.Count)];
                badRoles.Remove(role);
                badCount--;
            }
            else {
                if (goodRoles.Count == 0) role = defaultGood;
                else role = goodRoles[UnityEngine.Random.Range(0, goodRoles.Count)];
                goodRoles.Remove(role);
                goodCount--;
            }

            Debug.Log("bad " + badCount);
            Debug.Log("good " + goodCount);

            NetworkManager.Singleton.ConnectedClients[client.ClientId].PlayerObject?.Despawn(true);

            GameObject newPlayer = Instantiate(blankPlayer);

            if (Type.GetType(role.scriptName) != null)
            {
                foreach (string c in allRoleComponents) if (Type.GetType(c) != Type.GetType(role.scriptName)) Destroy(newPlayer.GetComponent(Type.GetType(c)));
            } 
            
            List<CurseObject> allCurses = allCurseComponents.ToList();
            for (int i=0; i < role.curseObjects.Length; i++) foreach (CurseObject c in allCurseComponents) if (c == role.curseObjects[i]) allCurses.Remove(c);
            foreach (CurseObject c in allCurses) Destroy(newPlayer.GetComponent(c.abilityType));

            newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(client.ClientId, true);

            RoleClass roleClass = newPlayer.GetComponent<RoleClass>();
            roleClass.rolePrefabs = role.prefabs;
            roleClass.isHuman.Value = role.isHuman;

            newPlayer.GetComponent<PlayerHealth>().health.Value = role.health;

            SetupCursesClientRPC(Array.IndexOf(allRoles, role), new ClientRpcParams { Send = new ClientRpcSendParams {TargetClientIds = new List<ulong> {client.ClientId}}});
            
            for (int i=0; i < role.startItems.Length; i++) {
                NetworkObject worldItem = Instantiate(role.startItems[i].worldItemObject);
                worldItem.Spawn(true);
                PickupItemClientRpc(worldItem, new ClientRpcParams { Send = new ClientRpcSendParams {TargetClientIds = new List<ulong> {client.ClientId}}});
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlayerServerRpc(ulong id)
    {
        GameObject newPlayer = Instantiate(defPlayer);
        newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(id, false);
    }

    [ClientRpc]
    private void PickupItemClientRpc(NetworkObjectReference itemRef, ClientRpcParams clientRpcParams)
    {
        if (itemRef.TryGet(out NetworkObject item)) item.GetComponent<WorldItem>().FinishInteract(NetworkManager.LocalClient.PlayerObject.gameObject);
    }

    [ClientRpc]
    private void SetupCursesClientRPC(int rolenumber, ClientRpcParams clientRpcParams)
    {
        RoleObject role = allRoles[rolenumber];
        NetworkObject p = NetworkManager.LocalClient.PlayerObject;

        p.GetComponent<RoleClass>().roleName = role.roleName;
        p.GetComponent<RoleClass>().Intro();

        if (p.GetComponent<CurseManager>() != null) p.GetComponent<CurseManager>().SetupCurses(role.curseObjects, menuPrefab, energyIconPrefab);
    }
}
