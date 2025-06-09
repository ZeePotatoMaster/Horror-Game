using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    private bool on = false;
    private PlayerBase pb;

    [SerializeField] private Light kira;

    // Start is called before the first frame update
    void Start()
    {
        pb = this.transform.parent.GetComponent<PlayerBase>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pb.attacked) Switch();

        if (!on) return;

        RaycastHit hit;
        if (Physics.SphereCast(pb.playerCamera.transform.position, 0.25f, pb.playerCamera.transform.forward, out hit))
        {
            if (hit.distance > 7) return;

            //if (hit.transform.tag != "Painting") hit.transform.GetComponent<Freddle>().OnFlashed();
        }
    }

    void Switch()
    {
        on = !on;

        kira.enabled = on ? true : false;
    }
}
