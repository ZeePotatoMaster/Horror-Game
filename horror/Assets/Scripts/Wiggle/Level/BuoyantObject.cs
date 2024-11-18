using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
     
    public Rigidbody rigidBody;
    public float maxDepth = 1f;
    public float displacement = 3f;
    public int floaters = 1;
    public float waterDrag = 0.99f;
    public float waterAngularDrag = 0.5f;

    private void FixedUpdate() {

        rigidBody.AddForceAtPosition(Physics.gravity / floaters, transform.position, ForceMode.Acceleration);

        float currentWaveHeight = WaveManager.instance.GetWaveHeight(transform.position.x, transform.position.z);

        if (transform.position.y <= (currentWaveHeight)) {

            float multiplier = Mathf.Clamp01((currentWaveHeight - transform.position.y) / maxDepth) * displacement;
            rigidBody.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * multiplier, 0f), transform.position, ForceMode.Acceleration);
            rigidBody.AddForce(multiplier * -rigidBody.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            rigidBody.AddTorque(multiplier * -rigidBody.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }
}
