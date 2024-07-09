using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private InventorySlot[] inventorySlots;
    [SerializeField] private GameObject inventoryItemPrefab;
    private int selectedSlot = -1;

    private void Start() {
        ChangeSelectedSlot(0);
    }

    public void ChangeSelectedSlot(int newValue) {
        if (selectedSlot >= 0) inventorySlots[selectedSlot].Deselect();
        inventorySlots[newValue].Select();
        selectedSlot = newValue;
    }

    public bool AddItem(InventoryItem item)
    {
        for (int i = 0; i <= inventorySlots.Length; i++)
        {
            ItemInSlot slotItem = inventorySlots[i].GetComponentInChildren<ItemInSlot>();
            if (slotItem == null) {
                SpawnNewItem(item, inventorySlots[i]);
                return true;
            }
        }

        return false;
    }

    private void SpawnNewItem(InventoryItem item, InventorySlot slot)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        ItemInSlot inventoryItem = newItemGo.GetComponent<ItemInSlot>();
        inventoryItem.InitializeItem(item);
    }
}
