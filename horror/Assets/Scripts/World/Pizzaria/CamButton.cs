using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamButton : Interactable
{
    [SerializeField] private Camera cam;
    [SerializeField] private CamSystem camSystem;
    
    
    [SerializeField] private Material selectColor;
    [SerializeField] private Material deselectColor;
    [SerializeField] private Canvas usedCanvas;

    public bool canSeal;
    public BotPosition sealPosition;

    public void Select(bool select)
    {
        Material m = select ? selectColor : deselectColor;
        this.GetComponent<MeshRenderer>().material = m;
        cam.enabled = select;
        usedCanvas.enabled = select;
    }

    public override void FinishInteract(GameObject player)
    {
        camSystem.ChangeCams(this);
    }
}
