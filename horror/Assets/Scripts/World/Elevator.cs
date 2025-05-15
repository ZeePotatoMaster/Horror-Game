using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [HideInInspector] public ulong ownerid;

    //void CloseDoors()

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != "Player") return;
        if (collision.gameObject.GetComponent<NetworkObject>().OwnerClientId == ownerid) Paintings.instance.OnPlayerEnterElevator(ownerid);
    }
}
