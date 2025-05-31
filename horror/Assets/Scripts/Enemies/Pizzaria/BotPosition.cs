using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotPosition : MonoBehaviour
{
    public BotPosition[] positions;
    public float[] chances;

    public float moveTime;
    public float randomMoveTime;

    //private string "animationname";

    public bool killSpot;

    [HideInInspector] public bool occupied = false;
    [HideInInspector] public bool spotted = false;

    void Update()
    {
        if (spotted && !occupied) spotted = false;
    }
}
