using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class BigChica : MonoBehaviour
{
    [SerializeField] private DefaultBot db;
    private BotPosition doorPos;
    private PowerScript powerManager;

    [SerializeField] private float timeBetweenPunches;

    private bool isPunching = false;

    // Update is called once per frame
    void Update()
    {
        if (db.currentPosition == doorPos && !isPunching)
        {
            Debug.Log("its time");
            Invoke(nameof(Punch), timeBetweenPunches);
            isPunching = true;
        }
    }

    void Punch()
    {
        if (db.currentPosition != doorPos) return;
        Debug.Log("punchington");

        powerManager.power -= 5f;
        //playsound
        isPunching = false;
    }

    public void Setup(BotPosition dp, PowerScript pm, BotPosition cp)
    {
        doorPos = dp;
        powerManager = pm;
        db.startingPosition = cp;
        db.Move(cp);
    }
}
