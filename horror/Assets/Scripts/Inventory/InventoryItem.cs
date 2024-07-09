using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Scriptable object/Item")]
public class InventoryItem : ScriptableObject
{
    public Sprite image;
    public bool stackable = false;
}
