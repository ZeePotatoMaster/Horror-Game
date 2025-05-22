using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] private ArmoryDoor ad;
    [SerializeField] private GameObject effects;
    [SerializeField] private float gasNeeded;
    private float gasAmount = 0f;
    private bool completed = false;

    /*public override void FinishInteract(GameObject player)
    {
        throw new System.NotImplementedException();
    }

    public override void OnInteract(GameObject player)
    {
        if (completed) return;

        InventoryManager m = player.GetComponent<InventoryManager>();
        ItemInSlot s = m.GetItemInSlot(m.selectedSlot);

        if (s == null) return;
        if (s.item.itemId != 5) return;

        
    }*/

    public void FillGenerator(Gasoline g)
    {
        if (completed) return;
        gasAmount += Time.deltaTime;
        g.currentGas -= Time.deltaTime;

        if (gasAmount >= gasNeeded) ActivateGenerator();
    }

    void ActivateGenerator()
    {
        completed = true;
        ad.CompleteGeneratorRpc();
        effects.SetActive(true);
    }
}
