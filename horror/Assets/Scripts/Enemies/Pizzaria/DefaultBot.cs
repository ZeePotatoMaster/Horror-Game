using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultBot : MonoBehaviour
{
    [HideInInspector] public BotPosition currentPosition;
    public BotPosition startingPosition;

    private float timeUntilMove;
    private float spotTimer;
    private float timeUntilSpotKill = -1f;

    // Start is called before the first frame update
    void Start()
    {
        if (startingPosition != null) Move(startingPosition);
    }

    // Update is called once per frame
    void Update()
    {
        timeUntilMove -= Time.deltaTime;

        if (timeUntilMove <= 0f)
        {
            BotPosition bp = GetRandomPosition();
            if (bp == null) Move(currentPosition);

            else if (!bp.killSpot) Move(bp);
            else if (!bp.Occupied) Pizzaria.instance.GetComponent<Pizzaria>().Kill(this.transform);
            else Move(startingPosition);
        }

        if (currentPosition.spotted)
        {
            if (timeUntilSpotKill == -1f) timeUntilSpotKill = Random.Range(0f, 5f) + 5f;
            if (!GetRandomPosition().Occupied) spotTimer += Time.deltaTime;
            if (spotTimer >= timeUntilSpotKill) Pizzaria.instance.GetComponent<Pizzaria>().Kill(this.transform);
        }
        else if (spotTimer > 0f)
        {
            timeUntilSpotKill = -1f;
            spotTimer = 0f;
        }
    }

    BotPosition GetRandomPosition()
    {
        float roll = Random.Range(0f, 1f);
        float chance = 0f;
        int positionToMove = -1;

        float chanceMultipler = 1f;
        List<int> occupiedValues = new List<int>();

        for (int i = 0; i < currentPosition.chances.Length; i++)
        {
            if (occupiedValues.Contains(i)) continue;

            chance += currentPosition.chances[i] / chanceMultipler;
            if (roll <= chance && currentPosition.positions[i] != null)
            {
                if (currentPosition.positions[i].killSpot) positionToMove = i;
                else if (!currentPosition.positions[i].Occupied) positionToMove = i;

                if (positionToMove != -1) break;
                else
                {
                    chanceMultipler -= currentPosition.chances[i];
                    chance = 0f;

                    occupiedValues.Add(i);
                    i = 0;
                }
            }
        }

        if (positionToMove == -1) return null;
        return currentPosition.positions[positionToMove];
    }

    public void Move(BotPosition b)
    {
        if (currentPosition != null) currentPosition.Occupied = false;
        b.Occupied = true;

        this.transform.SetPositionAndRotation(b.transform.position, b.transform.rotation);
        //if (currentAnimation != b.animation) play(b.animation)

        float rand = Random.Range(-b.randomMoveTime, b.randomMoveTime);
        timeUntilMove = b.moveTime + rand;

        currentPosition = b;
    }
}
