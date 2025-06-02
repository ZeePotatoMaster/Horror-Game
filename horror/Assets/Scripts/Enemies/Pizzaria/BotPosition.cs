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

    private bool occupied = false;
    [HideInInspector] public bool spotted = false;
    
    [SerializeField] private BotPosition[] linkedPoses;

    void Update()
    {
        if (spotted && !occupied) spotted = false;
    }

    public bool Occupied
    {
        get
        {
            return occupied;
        }
        set
        {
            if (value == occupied) return;
            occupied = value;

            foreach (BotPosition bp in linkedPoses)
            {
                if (bp.Occupied != occupied) bp.Occupied = occupied;
            }
        }
    }
}
