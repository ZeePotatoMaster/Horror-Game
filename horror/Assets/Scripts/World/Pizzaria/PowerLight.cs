using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerLight : Interactable
{
    [SerializeField] private BotPosition shinePosition;

    private bool canUse = true;
    [SerializeField] private PowerScript ps;
    private bool draining = false;
    private PlayerBase pb;

    public override void OnInteract(GameObject player)
    {
        if (!canUse || ps.power == 0) return;

        pb = player.GetComponent<PlayerBase>();
        if (!pb.interacted) return;

        if (draining) return;

        draining = true;
        this.GetComponent<Animator>().SetBool("On", true);
        ps.ChangeDrain(1);

        //if (shinePosition.occupied) playsound;
        if (shinePosition.Occupied) shinePosition.spotted = true;
    }

    void Update()
    {
        if (draining && (!pb.interacted || ps.power == 0))
        {
            draining = false;
            this.GetComponent<Animator>().SetBool("On", false);
            ps.ChangeDrain(-1);

            Debug.Log("goopy " + draining + pb.interacted);
        }
    }

    public override void FinishInteract(GameObject player)
    {
        throw new System.NotImplementedException();
    }
}
