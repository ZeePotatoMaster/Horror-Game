using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Scriptable object/Item")]
public class InventoryItem : ScriptableObject
{
    public Sprite image;
    public NetworkObject itemObject;
    public NetworkObject worldItemObject;
    public int itemId;
}
