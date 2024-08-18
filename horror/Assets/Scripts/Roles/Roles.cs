using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Roles : NetworkBehaviour
{
    [SerializeField] private GameObject farter;
    [SerializeField] private GameObject corpseEater;
    [SerializeField] private GameObject normie;
    
    public void AssignRoles()
    {
        if (!IsHost) return;

        float playerCount = NetworkManager.Singleton.ConnectedClientsList.Count;
        int badCount = (int)Mathf.Floor(playerCount/2f);
        int goodCount = (int)Mathf.Ceil(playerCount/2f);
        List<GameObject> goodClasses = new List<GameObject>{farter};
        List<GameObject> badClasses = new List<GameObject>{farter};

        Debug.Log("initial bad " + badCount);
        Debug.Log("initial good " + goodCount);


        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {   
            int coin = 0;
            if (badCount > 0 && goodCount > 0)  coin = Random.Range(0, 2);
            else if (badCount > 0 && goodCount == 0) coin = 0;
            else if (badCount == 0 && goodCount > 0) coin = 1;
            
            GameObject role;
            if (coin == 0) {
                if (badClasses.Count == 0) role = normie;
                else role = badClasses[Random.Range(0, badClasses.Count)];
                badClasses.Remove(role);
                badCount--;
            }
            else {
                if (goodClasses.Count == 0) role = normie;
                else role = goodClasses[Random.Range(0, goodClasses.Count)];
                goodClasses.Remove(role);
                goodCount--;
            }

            Debug.Log("bad " + badCount);
            Debug.Log("good " + goodCount);

            NetworkManager.Singleton.ConnectedClients[client.ClientId].PlayerObject.Despawn(true);
            GameObject newPlayer = Instantiate(role);
            newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(client.ClientId);
            
        }
    }
}
