using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLevel : MonoBehaviour
{

    public string LevelUUID;
    public GameObject parentLevel;
    public List<LevelLayout> layouts;

    void Start() {
        LevelUUID = System.Guid.NewGuid().ToString();

        parentLevel.name = "Level-" + LevelUUID;
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.G)) {

            destroyLevel();
            //createLevel();

            LevelLayout chosenLayout = layouts[Random.Range(0, layouts.Count)];
        }
    }

    void destroyLevel() {

        foreach (Transform child in parentLevel.transform) {

            Destroy(child.gameObject); 
        }
    } 
}
