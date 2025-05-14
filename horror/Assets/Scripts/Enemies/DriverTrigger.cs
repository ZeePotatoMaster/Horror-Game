using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverTrigger : MonoBehaviour
{
    private Transform parent;

    void Start()
    {
        parent = transform.parent;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);

        if (other.transform.tag == "Player" || other.transform.tag == "test") return;
        if (other.transform == parent) return;

        foreach (Transform child in parent.transform)
        {
            PlayerHealth p = child.GetComponent<PlayerHealth>();
            if (p != null) p.TryDamageServerRpc(parent.GetComponent<NewDriver>().bumpDamage);
        }

        Destroy(parent.gameObject);
    }
}
