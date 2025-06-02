using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Cupcake : MonoBehaviour
{
    private DefaultBot chicaBot;
    [SerializeField] private DefaultBot cupcakeBot;

    [SerializeField] private float switchTime;
    [SerializeField] private float switchRandomness;
    public float switchChance;

    private float currentSwitch = 0f;
    private float currentSwitchTime;

    private bool switched = false;

    void Start()
    {
        SetSwitchTime();
    }

    // Update is called once per frame
    void Update()
    {
        if (!switched) currentSwitch += Time.deltaTime;

        if (currentSwitch >= currentSwitchTime) CheckSwitch();
    }

    void CheckSwitch()
    {
        float roll = Random.Range(0f, 1f);
        if (roll <= switchChance) Switch();

        SetSwitchTime();
    }

    void SetSwitchTime()
    {
        float r = Random.Range(-switchRandomness, switchRandomness);
        currentSwitchTime = switchTime + r;

        currentSwitch = 0f;
    }

    void Switch()
    {
        BotPosition chicaPos = chicaBot.currentPosition;
        BotPosition cupcakePos = cupcakeBot.currentPosition.GetComponent<CupcakePosition>().chicaPosition;

        if (cupcakePos.Occupied) return;
        foreach (BotPosition pos in chicaPos.positions)
        {
            if (pos.killSpot) return;
        }

        chicaBot.Move(cupcakePos);
        cupcakeBot.Move(chicaPos);

        switched = true;
        cupcakeBot.enabled = false;

        Invoke(nameof(ResetSwitch), 20);
    }

    void ResetSwitch()
    {
        cupcakeBot.enabled = true;
        cupcakeBot.Move(cupcakeBot.startingPosition);
        switched = false;
    }

    public void Setup(DefaultBot db, BotPosition cupcakeStart)
    {
        chicaBot = db;

        cupcakeBot.startingPosition = cupcakeStart;
        cupcakeBot.Move(cupcakeStart);
    }

    public void SpeedUp()
    {
        switchChance = Mathf.Clamp(switchChance + 0.15f, 0f, 1f);
        switchTime = Mathf.Clamp(switchTime - 5f, switchRandomness, 999f);
    }
}
