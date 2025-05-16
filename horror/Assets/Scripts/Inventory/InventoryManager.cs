using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class InventoryManager : NetworkBehaviour
{
    [HideInInspector] public List<InventorySlot> inventorySlots = new List<InventorySlot>();
    private InventoryItem[] inventoryItems;
    private Dictionary<int, NetworkObject> itemObjects = new Dictionary<int, NetworkObject>();
    [SerializeField] private GameObject inventoryItemPrefab;
    [HideInInspector] public int selectedSlot = -1;

    public override void OnNetworkSpawn() {
        if (!IsOwner) return;

        inventoryItems = GameObject.Find("ItemHolder").GetComponent<ItemHolder>().inventoryItems;

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

    public void DropItem() {
        if (!IsOwner) return;
        if (!itemObjects.ContainsKey(selectedSlot)) return;
        if (!inventorySlots[selectedSlot].GetComponentInChildren<ItemInSlot>().canDrop) return;

        SpawnWorldItemServerRpc(NetworkManager.LocalClientId, inventorySlots[selectedSlot].GetComponentInChildren<ItemInSlot>().item.itemId, itemObjects[selectedSlot]);
        DestroyItem(selectedSlot);
    }

    public void DestroyItem(int slot)
    {
        inventorySlots[slot].GetComponentInChildren<ItemInSlot>().DestroySelf();
        DestroyItemServerRpc(itemObjects[slot]);
        itemObjects.Remove(slot);
    }

    public bool HoldingSomething() 
    {
        if (!itemObjects.ContainsKey(selectedSlot)) return false;
        else return true;
    }

    public ItemInSlot GetItemInSlot(int i)
    {
        return inventorySlots[i].GetComponentInChildren<ItemInSlot>();
    }

    public int AddItem(InventoryItem item)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            ItemInSlot slotItem = inventorySlots[i].GetComponentInChildren<ItemInSlot>();
            if (slotItem == null) {
                SpawnNewItem(item, inventorySlots[i], i);
                return i;
            }
        }

        return -1;
    }

    private void SpawnNewItem(InventoryItem item, InventorySlot slot, int slotNumber)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        ItemInSlot inventoryItem = newItemGo.GetComponent<ItemInSlot>();
        inventoryItem.InitializeItem(item, this, slotNumber);
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

    [ServerRpc (RequireOwnership = false)]
    private void SpawnWorldItemServerRpc(ulong id, int itemId, NetworkObjectReference reference)
    {
        InventoryItem item = null;
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i].itemId == itemId) item = inventoryItems[i];
        }

        NetworkObject worldItem = Instantiate(item.worldItemObject, NetworkManager.ConnectedClients[id].PlayerObject.gameObject.transform.position, Quaternion.identity);
        worldItem.SpawnWithOwnership(id, true);
        SetupWorldItemClientRpc(worldItem, new ClientRpcParams { Send = new ClientRpcSendParams {TargetClientIds = new List<ulong> {id}}});
    }

    [ClientRpc]
    private void SetupWorldItemClientRpc(NetworkObjectReference reference, ClientRpcParams clientRpcParams)
    {
        if (reference.TryGet(out NetworkObject worldItem)) worldItem.GetComponent<WorldItem>().SetupWorldItem(itemObjects[selectedSlot]);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyItemServerRpc(NetworkObjectReference reference)
    {
        if (reference.TryGet(out NetworkObject itemObject)) itemObject.Despawn(true);
    }
}
