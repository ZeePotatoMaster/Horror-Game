using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChicaScript : MonoBehaviour
{
    [SerializeField] private BotPosition eatPosition;
    [SerializeField] private float eatingTime;
    private float currentEat = 0f;
    
    //setup bigchica
    [SerializeField] private BotPosition doorPos;
    [SerializeField] private PowerScript powerManager;
    [SerializeField] private BotPosition bcPosition;

    [SerializeField] private GameObject bigChica;

    // Update is called once per frame
    void Update()
    {
        if (eatPosition.occupied) currentEat += Time.deltaTime;
        if (currentEat >= eatingTime) GetBig();
    }

    void GetBig()
    {
        GameObject bc = Instantiate(bigChica, transform.position, transform.rotation);
        bc.GetComponent<BigChica>().Setup(doorPos, powerManager, bcPosition);
        Destroy(this.gameObject);
    }
}
