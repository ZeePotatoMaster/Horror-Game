using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class InventoryManager : NetworkBehaviour
{
    [SerializeField] private InventorySlot[] inventorySlots;
    private Dictionary<int, NetworkObject> itemObjects = new Dictionary<int, NetworkObject>();
    [SerializeField] private GameObject inventoryItemPrefab;
    private int selectedSlot = -1;

    private void Start() {
        ChangeSelectedSlot(0);
    }

    public void ChangeSelectedSlot(int newValue) {
        int slot = selectedSlot + newValue;
        if (slot < 0 || slot >= inventorySlots.Length) return;

        if (itemObjects.ContainsKey(selectedSlot)) itemObjects[selectedSlot].gameObject.SetActive(false);
        if (selectedSlot >= 0) inventorySlots[selectedSlot].Deselect();

        inventorySlots[slot].Select();
        if (itemObjects.ContainsKey(slot)) itemObjects[slot].gameObject.SetActive(true);

        selectedSlot = slot;
    }

    public int AddItem(InventoryItem item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            ItemInSlot slotItem = inventorySlots[i].GetComponentInChildren<ItemInSlot>();
            if (slotItem == null) {
                SpawnNewItem(item, inventorySlots[i]);
                return i;
            }
        }

        return -1;
    }

    private void SpawnNewItem(InventoryItem item, InventorySlot slot)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        ItemInSlot inventoryItem = newItemGo.GetComponent<ItemInSlot>();
        inventoryItem.InitializeItem(item);
    }

    public void AddItemObject(int slot, NetworkObject itemObject) {
        itemObjects.Add(slot, itemObject);
        if (slot == selectedSlot) ChangeSelectedSlot(0);
    }
}
