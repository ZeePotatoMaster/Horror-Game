using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShapeTask : Interactable
{
    private int currentShape;
    [SerializeField] private int combinationSlot;
    [SerializeField] private Teleporter tp;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        currentShape = Random.Range(0, 2);
        tp.currentCombination[combinationSlot] = currentShape;
    }

    public override void FinishInteract(GameObject player)
    {
        ChangeShapeRpc();
    }

    [Rpc(SendTo.Server)]
    void ChangeShapeRpc()
    {
        currentShape++;
        if (currentShape > 2) currentShape = 0;
        tp.currentCombination[combinationSlot] = currentShape;
        //animate

        if (tp.currentCombination == tp.correctCombination) tp.ActivateTeleporterRpc();
    }
}
