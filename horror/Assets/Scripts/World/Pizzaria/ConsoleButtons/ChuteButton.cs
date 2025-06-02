using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuteButton : ConsoleButton
{
    [SerializeField] private BotPosition chutePosition;

    protected override void Start()
    {
        chutePosition.Occupied = true;
        base.Start();
    }

    public override void Activate()
    {
        chutePosition.Occupied = false;
        StartCoroutine(CloseChute());
    }

    IEnumerator CloseChute()
    {
        yield return new WaitForSeconds(60);

        if (chutePosition.Occupied) yield return null;

        chutePosition.Occupied = true;
    }
}
