using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerDoor : Interactable
{
    [HideInInspector] public bool isOpen = true;
    private bool canUse = true;
    [SerializeField] private PowerScript ps;

    [SerializeField] private BotPosition deathPosition;

    public override void FinishInteract(GameObject player)
    {
        if (!canUse || ps.power == 0) return;

        if (isOpen)
        {
            this.GetComponent<Animator>().Play("Close");
            ps.ChangeDrain(2);
            deathPosition.occupied = true;

            isOpen = false;
        }
        else
        {
            this.GetComponent<Animator>().Play("Open");
            ps.ChangeDrain(-2);
            deathPosition.occupied = false;

            isOpen = true;
        }
    }

    void Update()
    {
        if (ps == null) return;
        if (ps.power == 0 && !isOpen)
        {
            this.GetComponent<Animator>().Play("Open");
            ps.ChangeDrain(-2);
            deathPosition.occupied = false;

            isOpen = true;
        }
    }
}
