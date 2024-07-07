using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Layout", menuName = "Level/Create New Level Layout")]
public class LevelLayout : ScriptableObject
{
    public List<RoomPlacement> rooms;

    public void addRoomPlacement(RoomPlacement placement) {

        rooms.Add(placement);
    }

    public void placeRooms(GameObject parentLevel) {

        foreach (RoomPlacement room in rooms) {

            room.placeRoom(parentLevel);
        }
    }
}
