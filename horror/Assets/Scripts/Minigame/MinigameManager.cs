using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class MinigameManager : NetworkBehaviour
{
    public static MinigameManager instance;
    [HideInInspector] public bool dieNormally = true;

    public void Awake()
    {
        instance = this;
    }

    public abstract void OnPlayerDeath(ulong id);

    public abstract void OnPlayerEnterElevator(ulong id);
}
