using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public InventoryItem[] inventoryItems;
    public static ItemHolder instance;

    void Awake()
    {
        instance = this;
    }
}
