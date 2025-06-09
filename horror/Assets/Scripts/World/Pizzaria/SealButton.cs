using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SealButton : Interactable
{
    [HideInInspector] public bool sealing = false;
    private float sealTick = 0f;
    [SerializeField] private float sealTime;
    [HideInInspector] public CamButton sealCam;
    [SerializeField] private CamSystem camSystem;

    [SerializeField] private Material selectColor;
    [SerializeField] private Material activeColor;
    [SerializeField] private Material errorColor;
    private Material deselectColor;

    private bool seal = false;

    void Start()
    {
        deselectColor = GetComponent<MeshRenderer>().material;
    }

    public override void FinishInteract(GameObject player)
    {
        camSystem.SealCam();
    }

    void Update()
    {
        if (sealing)
        {
            sealTick += Time.deltaTime;
            if (sealCam.sealPosition.Occupied)
            {
                StopSeal();
                GetComponent<MeshRenderer>().material = errorColor;
            }
        }

        if (sealTick >= sealTime)
        {
            StopSeal();
            Seal();
        }
    }

    public void StartSeal()
    {
        sealing = true;
        GetComponent<MeshRenderer>().material = selectColor;
    }

    public void StopSeal()
    {
        if (!sealing) return;
        sealing = false;
        sealTick = 0f;
        ChangeColor(false);
        sealCam = null;
    }

    public void UnSeal()
    {
        if (sealCam == null) return;

        sealCam.sealPosition.Occupied = false;
        seal = false;
    }

    void Seal()
    {
        sealCam.sealPosition.Occupied = true;
        seal = true;
        ChangeColor(true);
    }

    public void ChangeColor(bool activeButton)
    {
        Material selectm = seal ? activeColor : deselectColor;
        Material m = activeButton ? selectm : deselectColor;

        GetComponent<MeshRenderer>().material = m;
    }
}
