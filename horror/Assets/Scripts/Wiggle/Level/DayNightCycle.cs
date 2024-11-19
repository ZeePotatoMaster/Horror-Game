using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{

    [SerializeField]
    private int ticks;
    [SerializeField]
    private Material skyBox;

    public float blendProgress = 0f;

    [SerializeField]
    private int sunriseTriggerTick = 100;
    [SerializeField]
    private int dayTriggerTick = 11000;
    [SerializeField]
    private int sunsetTriggerTick = 31000;
    [SerializeField]
    private int nightTriggerTick = 41000;

    [SerializeField]
    private float blendIncrement = 0.01f;

    [SerializeField]
    private float starsThreshold = 33.2f;

    [SerializeField]
    private Color nightPrimary;
    [SerializeField]
    private Color nightSecondary;

    [SerializeField]
    private Color sunrisePrimary;
    [SerializeField]
    private Color sunriseSecondary;

    [SerializeField]
    private Color dayPrimary;
    [SerializeField]
    private Color daySecondary;
    

    // Start is called before the first frame update
    void Start()
    {
        ticks = 0;
        blendProgress = 0f;

        skyBox.SetColor("_PrimaryColor", nightPrimary);
        skyBox.SetColor("_SecondaryColor", nightSecondary);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ticks < 31000) {

            ticks++;
        } else {
            ticks = 0;
        }

        if (ticks == sunriseTriggerTick || ticks == dayTriggerTick || ticks == sunsetTriggerTick || ticks == nightTriggerTick) {

            blendProgress = 0f;
        }
        
        //TODO: light levels decrease at night
        //TODO: fog increases at night

        //NO STARS: 160
        //PLENTY: 33.2

        //blend between night to sunrise
        if (ticks >= sunriseTriggerTick && ticks < dayTriggerTick) {

            BlendColors(nightPrimary, sunrisePrimary, nightSecondary, sunriseSecondary);

            starsThreshold = 33.2f + (blendProgress * 100);
            skyBox.SetFloat("_StarThreshold", starsThreshold);
        }

        //blend between sunrise to day
        if (ticks >= dayTriggerTick && ticks < sunsetTriggerTick) {

            BlendColors(sunrisePrimary, dayPrimary, sunriseSecondary, daySecondary);
        }

        //blend between day to sunset
        if (ticks >= sunsetTriggerTick && ticks < nightTriggerTick) {

            BlendColors(dayPrimary, sunrisePrimary, daySecondary, sunriseSecondary);
        }

        //blend between sunset to night
        if (ticks >= nightTriggerTick) {

            BlendColors(sunrisePrimary, nightPrimary, sunriseSecondary, nightSecondary);

            if (starsThreshold > 1f) {
                starsThreshold = 160f - (blendProgress * 100);
                skyBox.SetFloat("_StarThreshold", starsThreshold);
            }
        }
    }

    void BlendColors(Color initialPrimary, Color resultPrimary, Color initialSecondary, Color resultResultSecondary) {

        skyBox.SetColor("_PrimaryColor", Color.Lerp(initialPrimary, resultPrimary, blendProgress));
        skyBox.SetColor("_SecondaryColor", Color.Lerp(initialSecondary, resultResultSecondary, blendProgress));

        if (blendProgress < 1.0f) {
            blendProgress += blendIncrement;
        }
    }
}
