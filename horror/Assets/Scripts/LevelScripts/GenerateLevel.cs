using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{

    public GameObject parentLevel;
    public string LevelUUID;

    public int roomCount;
    public int roomUpperLimit;

    public List<GameObject> roomPrefabs;

    // Start is called before the first frame update
    void Start()
    {

        roomCount = 1;
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

        roomCount = 1;
    } 

    void createLevel() {

        placeRooms(null);
    }

    void placeRooms(Room previousRoom) {

        //create starting room

        if (roomCount <= 1) {

            Room newRoom = createRoomVar();

            roomCount++;

            checkConnectors(newRoom);
        }

        GameObject previousRoomObject = null;

        if (previousRoom != null) {

            previousRoomObject = previousRoom.roomObject;
        } else {

            previousRoomObject = parentLevel;
        }

        if (previousRoom != null) {

            checkConnectors(previousRoom);
        }
    }

    //go through each possible connector and spawn an adjacent room if possible

    void checkConnectors(Room previousRoom) {

        if (previousRoom.hasSouthConnector() && !previousRoom.southConnectorUsed) {

            if (roomCount < roomUpperLimit) {

                GameObject clonedConnector = null;

                Room newRoom = createRoomVar();

                if (newRoom.hasNorthConnector() && !newRoom.northConnectorUsed) {

                    instantiateRoom(previousRoom, newRoom, 0.0f, "north", "south", clonedConnector);
                } else if (newRoom.hasEastConnector() && !newRoom.eastConnectorUsed) {

                    instantiateRoom(previousRoom, newRoom, 90.0f, "east", "south", clonedConnector);
                } else if (newRoom.hasSouthConnector() && !newRoom.southConnectorUsed) {

                    instantiateRoom(previousRoom, newRoom, -180.0f, "south", "south", clonedConnector);
                } else if (newRoom.hasWestConnector() && !newRoom.westConnectorUsed) {

                    instantiateRoom(previousRoom, newRoom, -90.0f, "west", "south", clonedConnector);
                }

                roomCount++;

                placeRooms(newRoom);
            }
        }
        
        if (previousRoom.hasEastConnector() && !previousRoom.eastConnectorUsed) {

            if (roomCount < roomUpperLimit) {

                GameObject clonedConnector = null;

                Room newRoom = createRoomVar();

                if (newRoom.hasNorthConnector() && !newRoom.northConnectorUsed) {

                    instantiateRoom(previousRoom, newRoom, -90.0f, "north", "east", clonedConnector);
                } else if (newRoom.hasEastConnector() && !newRoom.eastConnectorUsed) {

                    instantiateRoom(previousRoom, newRoom, -180.0f, "east", "east", clonedConnector);
                } else if (newRoom.hasSouthConnector() && !newRoom.southConnectorUsed) {

                    instantiateRoom(previousRoom, newRoom, 90.0f, "south", "east", clonedConnector);
                } else if (newRoom.hasWestConnector() && !newRoom.westConnectorUsed) {

                    instantiateRoom(previousRoom, newRoom, 0.0f, "west", "east", clonedConnector);
                }

                roomCount++;

                placeRooms(newRoom);
            }
        }
        
        if (previousRoom.hasNorthConnector() && !previousRoom.northConnectorUsed) {

            if (roomCount < roomUpperLimit) {

                GameObject clonedConnector = null;

                Room newRoom = createRoomVar();

                if (newRoom.hasNorthConnector() && !newRoom.northConnectorUsed) {

                    instantiateRoom(previousRoom, newRoom, -180.0f, "north", "north", clonedConnector);
                } else if (newRoom.hasEastConnector() && !newRoom.eastConnectorUsed) {

                    instantiateRoom(previousRoom, newRoom, -90.0f, "east", "north", clonedConnector);
                } else if (newRoom.hasSouthConnector() && !newRoom.southConnectorUsed) {

                    instantiateRoom(previousRoom, newRoom, 0.0f, "south", "north", clonedConnector);
                } else if (newRoom.hasWestConnector() && !newRoom.westConnectorUsed) {

                    instantiateRoom(previousRoom, newRoom, 90.0f, "west", "north", clonedConnector);
                }

                roomCount++;

                placeRooms(newRoom);
            }
        }
        
        if (previousRoom.hasWestConnector() && !previousRoom.westConnectorUsed) {

            if (roomCount < roomUpperLimit) {

                GameObject clonedConnector = null;

                Room newRoom = createRoomVar();

                if (newRoom.hasNorthConnector() && !newRoom.northConnectorUsed) {

                    instantiateRoom(previousRoom, newRoom, 90.0f, "north", "west", clonedConnector);
                } else if (newRoom.hasEastConnector() && !newRoom.eastConnectorUsed) {

                    instantiateRoom(previousRoom, newRoom, 0.0f, "east", "west", clonedConnector);
                } else if (newRoom.hasSouthConnector() && !newRoom.southConnectorUsed) {

                    instantiateRoom(previousRoom, newRoom, -90.0f, "south", "west", clonedConnector);
                } else if (newRoom.hasWestConnector() && !newRoom.westConnectorUsed) {

                    instantiateRoom(previousRoom, newRoom, -180.0f, "west", "west", clonedConnector);
                }

                roomCount++;

                placeRooms(newRoom);
            }
        }
    }

    //create a new room object and prefab

    Room createRoomVar() {

        Room newRoom = (Room) ScriptableObject.CreateInstance("Room");

        GameObject room = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Count)], parentLevel.transform);
                
        determineRoomConnectors(newRoom, room);

        return newRoom;
    }

    //connect the rooms

    void instantiateRoom(Room previousRoom, Room newRoom, float rotation, string targetConnector, string previousConnector, GameObject clonedConnector) {

        //clone the connector of this new room and disable connector for future use

        if (targetConnector == "north") {

            newRoom.setNorthConnectorStatus(true);

            clonedConnector = Instantiate(newRoom.northConnector, newRoom.northConnector.transform.position, newRoom.northConnector.transform.rotation);

            newRoom.roomObject.transform.SetParent(clonedConnector.transform);
        } else if (targetConnector == "east") {

            newRoom.setEastConnectorStatus(true);

            clonedConnector = Instantiate(newRoom.eastConnector, newRoom.eastConnector.transform.position, newRoom.eastConnector.transform.rotation);

            newRoom.roomObject.transform.SetParent(clonedConnector.transform);
        } else if (targetConnector == "south") {

            newRoom.setSouthConnectorStatus(true);

            clonedConnector = Instantiate(newRoom.southConnector, newRoom.southConnector.transform.position, newRoom.southConnector.transform.rotation);

            newRoom.roomObject.transform.SetParent(clonedConnector.transform);
        } else if (targetConnector == "west") {

            newRoom.setWestConnectorStatus(true);

            clonedConnector = Instantiate(newRoom.westConnector, newRoom.westConnector.transform.position, newRoom.westConnector.transform.rotation);

            newRoom.roomObject.transform.SetParent(clonedConnector.transform);
        }

        //teleport the cloned connector to the previous rooms connector and disable that connector too

        if (previousConnector == "north") {

            clonedConnector.transform.position = previousRoom.northConnector.transform.position;

            previousRoom.setNorthConnectorStatus(true);
        } else if (previousConnector == "east") {

            clonedConnector.transform.position = previousRoom.eastConnector.transform.position;

            previousRoom.setEastConnectorStatus(true);
        } else if (previousConnector == "south") {

            clonedConnector.transform.position = previousRoom.southConnector.transform.position;

            previousRoom.setSouthConnectorStatus(true);
        } else if (previousConnector == "west") {

            clonedConnector.transform.position = previousRoom.westConnector.transform.position;

            previousRoom.setWestConnectorStatus(true);
        }

        clonedConnector.transform.Rotate(0.0f, rotation, 0.0f);

        newRoom.roomObject.transform.SetParent(parentLevel.transform);

        Destroy(clonedConnector);
    }

    //set the room object variables and other necessary data

    void determineRoomConnectors(Room room, GameObject worldRoom) {

        if (worldRoom.transform.Find("NorthConnector") != null) {

            room.northConnector = worldRoom.transform.Find("NorthConnector").gameObject;
        }

        if (worldRoom.transform.Find("EastConnector") != null) {

            room.eastConnector = worldRoom.transform.Find("EastConnector").gameObject;
        }

        if (worldRoom.transform.Find("SouthConnector") != null) {

            room.southConnector = worldRoom.transform.Find("SouthConnector").gameObject;
        }

        if (worldRoom.transform.Find("WestConnector") != null) {

            room.westConnector = worldRoom.transform.Find("WestConnector").gameObject;
        }

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
