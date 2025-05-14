using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    [SerializeField] private float damage;
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag != "Player") return;
        PlayerHealth p = other.transform.GetComponent<PlayerHealth>();
        p.TryDamageServerRpc(damage);
        Debug.Log("oaaiofajioafio");
    }
}
