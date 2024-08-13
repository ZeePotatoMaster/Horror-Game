using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingRandomizer : MonoBehaviour
{

    public List<Material> paintings;
    public Material corruptedPainting;
    public GameObject gameObject;
    public bool isCorrupted = false;
    public int corruptionChance = 10;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Renderer>().material = paintings[Random.Range(0, paintings.Count)];

        if (Random.Range(1, corruptionChance) == 1) {

            isCorrupted = true;
            gameObject.GetComponent<Renderer>().material = corruptedPainting;
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
