using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{

    public GameObject parentLevel;
    public string LevelUUID;
    public GameObject roomPrefab;

    public int roomCount;
    public int roomUpperLimit;

    // Start is called before the first frame update
    void Start()
    {

        roomCount = 0;
        roomUpperLimit = 20;

        LevelUUID = System.Guid.NewGuid().ToString();

        parentLevel.name = "Level-" + LevelUUID;

        destroyLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) {

            destroyLevel();
            createLevel();
        }
    }

    void destroyLevel() {

        foreach (Transform child in parentLevel.transform) {

            Destroy(child.gameObject); 
        }

        roomCount = 0;
    } 

    void createLevel() {

        placeRooms(null);
    }

    void placeRooms(Room previousRoom) {

        Room newRoom = (Room) ScriptableObject.CreateInstance("Room");

        GameObject previousRoomObject = null;

        if (previousRoom != null) {

            previousRoomObject = previousRoom.roomObject;
        } else {

            previousRoomObject = parentLevel;
        }

        GameObject room = Instantiate(roomPrefab, previousRoomObject.transform);

        createRoom(newRoom, room);

        if (previousRoom != null) {

            GameObject clonedConnector = Instantiate(newRoom.northConnector, newRoom.northConnector.transform.position, newRoom.northConnector.transform.rotation);

            newRoom.roomObject.transform.SetParent(clonedConnector.transform);

            clonedConnector.transform.position = previousRoom.southConnector.transform.position;
        }

        roomCount++;

        if (roomCount <= 20) {

            placeRooms(newRoom);
        }
    }

    void createRoom(Room room, GameObject worldRoom) {

        room.northConnector = worldRoom.transform.Find("NorthConnector").gameObject;
        room.eastConnector = worldRoom.transform.Find("EastConnector").gameObject;
        room.southConnector = worldRoom.transform.Find("SouthConnector").gameObject;
        room.westConnector = worldRoom.transform.Find("WestConnector").gameObject;

        room.roomObject = worldRoom;

        room.roomObject.transform.SetParent(parentLevel.transform);

        room.roomObject.name = "Room " + roomCount;
    }

    void determineRotation() {

    }

    void storeDataSync() {


    }

    void matchConnectors(Room currentRoom, Room connectingRoom) {

        
    }
}
