using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightteam : MonoBehaviour
{
    [HideInInspector] public int teamNumber;
    [HideInInspector] public List<ulong> teammates = new List<ulong>();

    public void SetTeamNumber(int n)
    {
        teamNumber = n;
    }
}
