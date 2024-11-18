using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(menuName = "Scriptable object/Role")]
public class RoleObject : ScriptableObject
{
    public float health;
    public bool isHuman;
    public string roleName;
    public string scriptName;
    public InventoryItem[] startItems;
    public GameObject[] prefabs;
}