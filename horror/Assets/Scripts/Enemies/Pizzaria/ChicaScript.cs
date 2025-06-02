using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ChicaScript : MonoBehaviour
{
    [SerializeField] private BotPosition eatPosition;
    [SerializeField] private float eatingTime;
    private float currentEat = 0f;

    //setup bigchica
    [SerializeField] private BotPosition cupcakeStart;

    [SerializeField] private GameObject cupcakePrefab;
    private Cupcake cupcake;
    private bool cupcakeSpawned = false;

    // Update is called once per frame
    void Update()
    {
        if (eatPosition.Occupied) currentEat += Time.deltaTime;

        if (currentEat >= eatingTime && !cupcakeSpawned) SpawnCupcake();
        else if (currentEat >= eatingTime && cupcakeSpawned) SpeedUpCupcake();
    }

    void SpawnCupcake()
    {
        DefaultBot db = this.GetComponent<DefaultBot>();

        GameObject c = Instantiate(cupcakePrefab, transform.position, transform.rotation);
        cupcake = c.GetComponent<Cupcake>();
        cupcake.Setup(db, cupcakeStart);

        db.Move(db.startingPosition);

        cupcakeSpawned = true;
        currentEat = 0f;
    }
    void SpeedUpCupcake()
    {
        DefaultBot db = this.GetComponent<DefaultBot>();
        db.Move(db.startingPosition);
        currentEat = 0f;

        cupcake.SpeedUp();
    }
}
