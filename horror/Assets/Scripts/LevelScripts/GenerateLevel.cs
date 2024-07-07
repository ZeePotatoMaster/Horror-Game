using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{

    public string LevelUUID;
    public GameObject parentLevel;
    public List<LevelLayout> layouts;
    public LevelLayout chosenLayout;

    void Start() {
        LevelUUID = System.Guid.NewGuid().ToString();

        parentLevel.name = "Level-" + LevelUUID;
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.G)) {

            destroyLevel();
            createLevel();
        }
    }

    void destroyLevel() {

        foreach (Transform child in parentLevel.transform) {

            Destroy(child.gameObject); 
        }
    } 

    void createLevel() {

        chosenLayout = layouts[Random.Range(0, layouts.Count)];

        chosenLayout.placeRooms(parentLevel);
    }
}
