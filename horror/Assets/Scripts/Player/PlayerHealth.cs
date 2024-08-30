using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    public NetworkVariable<float> health = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private GameObject hurtScreen;
    [SerializeField] private GameObject ragDoll;
    [SerializeField] private Camera playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) return;
        GameObject canvas = GameObject.Find("Canvas");
        hurtScreen = canvas.transform.Find("HurtImage").gameObject;
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        health.OnValueChanged += (float previousHealth, float newHealth) => {
            HurtEffect();
            if (newHealth <= 0) Death();
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        if (hurtScreen.GetComponent<Image>().color.a > 0)
        {
            var color = hurtScreen.GetComponent<Image>().color;
            color.a -= 0.01f;
            hurtScreen.GetComponent<Image>().color = color;
        }
    }

    private void HurtEffect()
    {
        if (hurtScreen == null) return;
        var color = hurtScreen.GetComponent<Image>().color;
        color.a = 0.5f;
        hurtScreen.GetComponent<Image>().color = color;
    }

    private void Death()
    {
        SummonRagdollServerRpc(this.transform.position, this.OwnerClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DamageServerRpc(float damage)
    {
        health.Value -= damage;
    }

    [ServerRpc]
    private void SummonRagdollServerRpc(Vector3 pos, ulong id)
    {
        GameObject doll = Instantiate(ragDoll, pos, Quaternion.identity);
        doll.GetComponent<NetworkObject>().Spawn(true);
        AttachCamClientRpc(doll.GetComponent<NetworkBehaviour>().NetworkObjectId, new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> {id}}});
        NetworkManager.Singleton.ConnectedClients[id].PlayerObject.Despawn(true);
    }

    [ClientRpc]
    private void AttachCamClientRpc(ulong id, ClientRpcParams clientRpcParams)
    {
        NetworkManager.LocalClient.PlayerObject.transform.GetChild(0).SetParent(NetworkManager.SpawnManager.SpawnedObjects[id].transform, true);
    }
}
