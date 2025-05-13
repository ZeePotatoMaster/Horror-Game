using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleKill : MonoBehaviour
{
    [SerializeField] private float killTime;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, killTime);
    }
}
