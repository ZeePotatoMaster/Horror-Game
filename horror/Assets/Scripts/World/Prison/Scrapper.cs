using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrapper : Interactable
{
    [SerializeField] private Crafter crafter;

    public override void FinishInteract(GameObject player)
    {
        InventoryManager m = player.GetComponent<InventoryManager>();
        ItemInSlot s = m.GetItemInSlot(m.selectedSlot);

        if (s == null) return;
        if (s.item.itemId == 5) return;

        crafter.AddScrapRpc(1);
        m.DestroyItem(m.selectedSlot);
    }
}
