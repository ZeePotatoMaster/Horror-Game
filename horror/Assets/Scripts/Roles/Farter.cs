using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Farter : RoleClass
{
    private float cylinderMunchTime = 3f;

    private void Awake()
    {
        roleName = "Farter";
        isHuman = false;
    }

    public override void Update()
    {
        base.Update();
        if (pb.interactObject != null) {
            if (pb.interactObject.tag == "test") {
                pb.isInteracting = true;

                pb.interactTick += Time.deltaTime;
                energyIcon.GetComponent<Image>().fillAmount = pb.interactTick/cylinderMunchTime;
                if (pb.interactTick >= cylinderMunchTime) {
                    pb.EndInteract();
                }
            }
        }
    }
}
