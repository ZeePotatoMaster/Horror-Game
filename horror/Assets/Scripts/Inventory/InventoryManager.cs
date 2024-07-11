using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class InventoryManager : NetworkBehaviour
{
    private List<InventorySlot> inventorySlots = new List<InventorySlot>();
    private Dictionary<int, NetworkObject> itemObjects = new Dictionary<int, NetworkObject>();
    [SerializeField] private GameObject inventoryItemPrefab;
    private int selectedSlot = -1;

    public override void OnNetworkSpawn() {
        if (!IsOwner) return;
        GameObject canvas = GameObject.Find("Canvas");
        Transform hotbar = canvas.transform.Find("Toolbar");
        foreach (InventorySlot slot in hotbar.GetComponentsInChildren<InventorySlot>())
        {
            inventorySlots.Add(slot);
        }
        ChangeSelectedSlot(0);
    }

    public void ChangeSelectedSlot(int newValue) {
        if (!IsOwner) return;
        int slot = selectedSlot + newValue;
        if (slot < 0 || slot >= inventorySlots.Count) return;

        if (itemObjects.ContainsKey(selectedSlot)) EnableItemServerRpc(itemObjects[selectedSlot].gameObject, false);
        if (selectedSlot >= 0) inventorySlots[selectedSlot].Deselect();

        inventorySlots[slot].Select();
        if (itemObjects.ContainsKey(slot)) EnableItemServerRpc(itemObjects[slot].gameObject, true);

        selectedSlot = slot;
    }

    public int AddItem(InventoryItem item)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
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
        if (!IsOwner) return;
        Debug.Log("IT WORKED ! " + itemObject);
        itemObjects.Add(slot, itemObject);
        EnableItemServerRpc(itemObject, false);
        if (slot == selectedSlot) ChangeSelectedSlot(0);
    }

    [ServerRpc (RequireOwnership = false)]
    private void EnableItemServerRpc(NetworkObjectReference refItem, bool enable)
    {
        Debug.Log("trying to happen");
        if (refItem.TryGet(out NetworkObject item)) {
            item.gameObject.SetActive(enable);
            Debug.Log("something happened " + item);
        } 
        EnableItemClientRpc(refItem, enable);
    }

    [ClientRpc]
    private void EnableItemClientRpc(NetworkObjectReference refItem, bool enable)
    {
        if (refItem.TryGet(out NetworkObject item)) item.gameObject.SetActive(enable);
    }
}
