using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    [SerializeField]
    private float amplitude = 0.0001f;
    [SerializeField]
    private float period = 2f;
    [SerializeField]
    private float speed = 0.00002f;
    [SerializeField]
    private float offset = 0f;
    [SerializeField]
    private int scale = 3;

    private void Awake() {


        offset = 0f;

        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(this);
        }
    }

    private void Update() {

        offset += Time.deltaTime * speed;
    }

    public float GetWaveHeight(float x, float z) {

        //return amplitude * Mathf.Sin(((x * scale) / period + offset) + ((z * scale) / period + offset))

        return amplitude * GerstnerWave(((x * scale) / period + offset), ((z * scale) / period + offset));
    }

    private float GerstnerWave(float x, float z)
    {
        float s = z;
        float k = (2 * Mathf.PI) / period;
        float c = Mathf.Sqrt(Mathf.Abs(Physics.gravity.y) / k);
        float a = s / k;

        Vector2 d = new Vector2(x, z);
        d.Normalize();

        float dot = Vector2.Dot(d, new Vector2(x, z));
        float f = k * (dot - c * Time.deltaTime * speed);

        return a * Mathf.Sin(f);
    }
}