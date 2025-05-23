using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SecurityMonitor : NetworkBehaviour
{
    private int activatedBoxes = 0;
    [SerializeField] private int neededBoxes;
    [SerializeField] private GameObject monitor;
    [SerializeField] private GameObject completedText;

    [Rpc(SendTo.Server)]
    public void BoxRpc()
    {
        activatedBoxes++;
        if (activatedBoxes != neededBoxes) return;

        ActivateRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void ActivateRpc()
    {
        monitor.SetActive(true);
        
        completedText = Instantiate(completedText, GameObject.Find("Canvas").transform);
        Invoke(nameof(DestroyText), 2f);
    }

    void DestroyText()
    {
        Destroy(completedText);
    }
}
