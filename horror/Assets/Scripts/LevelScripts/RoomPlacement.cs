using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Room Placement", menuName = "Level/Create New Room Placement")]
public class RoomPlacement : ScriptableObject
{
    public Vector3 placeLocation;
    public float rotation;
    public List<GameObject> roomPool;

    public RoomPlacement(Vector3 position) {

        placeLocation = position;
    }

    public void setPosition(Vector3 inPosition) {

        placeLocation = inPosition;
    }

    public void placeRoom(GameObject parentLevel) {

        GameObject placed = Instantiate(roomPool[Random.Range(0, roomPool.Count)], placeLocation, Quaternion.identity);
        placed.transform.Rotate(0f, rotation, 0f);
        placed.transform.SetParent(parentLevel.transform);
        placed.name = "Room";
    }

    public Vector3 getPosition() {

        return placeLocation;
    }
}
