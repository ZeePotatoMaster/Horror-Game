using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUnloader : MonoBehaviour
{

    public GameObject singleplayerPlayer;
    public int maxDistanceFromPlayer;

    public List<GameObject> levelRooms;

    public GameObject parentLevel;

    //UNUSED FOR NOW
    public List<GameObject> multiplayerPlayers;

    // Start is called before the first frame update
    void Start()
    {

        //note: revisit once random level loading is enabled and refactor as the third process in loading steps?

        populateFromScene();

        //default of 100, remove later
        maxDistanceFromPlayer = 100;
    }

    // Update is called once per frame
    void Update()
    {
        toggleRoomsLoaded();
    }

    void populateFromScene() {

        levelRooms.Clear();

        foreach (Transform child in parentLevel.transform) {

            levelRooms.Add(child.gameObject);
        }
    }

    void toggleRoomsLoaded() {

        foreach (GameObject room in levelRooms) {

            if (Vector3.Distance(singleplayerPlayer.transform.position, room.transform.position) > maxDistanceFromPlayer) {

                room.SetActive(false);
            } else {

                room.SetActive(true);
            }
        }
    }
}
