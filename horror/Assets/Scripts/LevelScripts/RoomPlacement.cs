using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPlacement : ScriptableObject
{
    public Vector3 placeLocation;
    public List<GameObject> roomPool;

    public void setPosition(Vector3 inPosition) {

        placeLocation = inPosition;
    }
}
