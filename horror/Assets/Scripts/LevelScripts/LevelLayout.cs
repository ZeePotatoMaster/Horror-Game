using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLayout : ScriptableObject
{
    public List<RoomPlacement> rooms;

    public void addRoomPlacement(RoomPlacement placement) {

        rooms.Add(placement);
    }
}
