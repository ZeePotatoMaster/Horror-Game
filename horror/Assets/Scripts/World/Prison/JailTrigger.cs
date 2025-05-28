using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class JailTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag != "Player") return;

        Prison prison = Prison.instance.GetComponent<Prison>();
        ulong i = other.transform.GetComponent<NetworkObject>().OwnerClientId;
        if (prison.prisoners.Contains(i)) prison.ChangeImprisonedRpc(i, true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.tag != "Player") return;

        Prison prison = Prison.instance.GetComponent<Prison>();
        ulong i = other.transform.GetComponent<NetworkObject>().OwnerClientId;
        if (prison.prisoners.Contains(i)) prison.ChangeImprisonedRpc(i, false);
    }
}
