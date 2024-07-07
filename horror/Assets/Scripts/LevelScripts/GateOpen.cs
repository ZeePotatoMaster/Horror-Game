using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateOpen : MonoBehaviour
{

    public List<GameObject> targetGates;
    public bool openedGate = false;
    double height = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E)) {
            
            if (!openedGate) {

                openedGate = true;
            }
        }

        if (openedGate) {

            openGate();
        }
    } 

    void openGate() {

        if (height < 3) {

            height += 0.003;
        }

        foreach (GameObject gate in targetGates) {

            Transform transform = gate.transform;

            gate.transform.position = new Vector3(transform.position.x, (float) height, transform.position.z);
        }
    }
}
