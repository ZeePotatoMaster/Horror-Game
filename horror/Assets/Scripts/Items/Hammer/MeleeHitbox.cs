using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{

    [HideInInspector] public List<GameObject> targets = new List<GameObject>();

    void OnDisable()
    {
        targets.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerHealth>() == null) return;

        if (!targets.Contains(other.gameObject)) targets.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerHealth>() == null) return;
        
        targets.Remove(other.gameObject);
    }
}
