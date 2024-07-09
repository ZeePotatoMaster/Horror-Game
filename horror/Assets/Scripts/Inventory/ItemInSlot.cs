using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInSlot : MonoBehaviour
{
    [HideInInspector] public InventoryItem item;
    private Image image;

    private void Awake() {
        image = GetComponent<Image>();
    }

    public void InitializeItem(InventoryItem i)
    {
        item = i;
        image.sprite = i.image;
    }
}
