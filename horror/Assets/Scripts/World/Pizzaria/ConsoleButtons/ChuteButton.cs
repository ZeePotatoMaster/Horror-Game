using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChuteButton : ConsoleButton
{
    [SerializeField] private BotPosition chutePosition;

    protected override void Start()
    {
        chutePosition.occupied = true;
        base.Start();
    }

    public override void Activate()
    {
        chutePosition.occupied = false;
        StartCoroutine(CloseChute());
    }

    IEnumerator CloseChute()
    {
        yield return new WaitForSeconds(60);

        if (chutePosition.occupied) yield return null;

        chutePosition.occupied = true;
    }
}
