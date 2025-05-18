using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Ragdoll : NetworkBehaviour
{
    private Rigidbody rb;
    [SerializeField]
    private Vector3 force;
    [SerializeField]
    private Vector3 rotation;
    [SerializeField]
    private float destroyDelay;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(Destroy(destroyDelay));
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
        rb.AddForce(force.x, force.y, force.z, ForceMode.Impulse);
        this.transform.Rotate(rotation.x, rotation.y, rotation.z);
    }

    private IEnumerator Destroy(float destroyDelay)
    {
        yield return new WaitForSeconds(destroyDelay);

        Destroy(this.gameObject);
    }
}
