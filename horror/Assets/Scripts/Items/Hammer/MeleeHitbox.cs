using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{

    [HideInInspector] public List<PlayerHealth> targets = new List<PlayerHealth>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerHealth>() == null) return;

        PlayerHealth h = other.GetComponent<PlayerHealth>();
        if (!targets.Contains(h)) targets.Add(h);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerHealth>() == null) return;
        
        targets.Remove(other.GetComponent<PlayerHealth>());
    }
}
