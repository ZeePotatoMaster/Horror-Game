using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Color selectColor, notSelectColor;
    private InventoryManager inventoryManager;

    private void Awake() {
        inventoryManager = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
    }

    public void Select() {
        image.color = selectColor;
    }

    public void Deselect() {
        image.color = notSelectColor;
    }
}
