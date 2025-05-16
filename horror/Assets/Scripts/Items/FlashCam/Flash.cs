using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flash : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<Image>().color.a > 0)
        {
            var color = this.GetComponent<Image>().color;
            color.a -= 0.001f;
            this.GetComponent<Image>().color = color;
        }
    }
}
