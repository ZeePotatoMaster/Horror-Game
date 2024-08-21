using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

[CreateAssetMenu(menuName = "Scriptable object/Curse")]
public class CurseObject : ScriptableObject
{
    public Texture image;
    public string curseName;
    public float cost;
    public string abilityType;
}
