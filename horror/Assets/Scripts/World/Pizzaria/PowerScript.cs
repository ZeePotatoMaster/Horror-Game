using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PowerScript : MonoBehaviour
{
    [SerializeField] private TMP_Text powerText;
    [HideInInspector] public float power;
    [SerializeField] private float startingPower;
    private int drainLevel = 1;

    // Start is called before the first frame update
    void Start()
    {
        power = startingPower;
    }

    // Update is called once per frame
    void Update()
    {
        if (power > 0) power -= Time.deltaTime * drainLevel;
        if (power < 0) power = 0;

        powerText.text = "Power: " + Mathf.Round(power / startingPower * 100) + "%";
    }

    public void ChangeDrain(int amount)
    {
        drainLevel += amount;
    }
}
