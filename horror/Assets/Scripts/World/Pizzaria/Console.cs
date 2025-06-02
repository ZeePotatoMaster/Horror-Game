using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Console : Interactable
{
    private ConsoleButton activeButton;

    [SerializeField] private PowerScript ps;
    [SerializeField] private Canvas monitor;

    [SerializeField] private Material selectColor;
    [SerializeField] private Material deselectColor;

    private bool activating = false;
    private float activateTick = 0f;
    [SerializeField] private float activateTime;

    // Update is called once per frame
    void Update()
    {
        if (ps.power == 0)
        {
            if (monitor.enabled) monitor.enabled = false;
            return;
        }
        if (activating) activateTick += Time.deltaTime;
        if (activateTick >= activateTime)
        {
            activeButton.Activate();
            ResetActivate();
        }
    }

    public void ChangeSelection(ConsoleButton cs)
    {
        if (activeButton == cs) return;
        cs.Select(true);

        if (activeButton != null) activeButton.Select(false);

        activeButton = cs;
        ResetActivate();
    }

    public override void FinishInteract(GameObject player)
    {
        activating = true;
        this.GetComponent<MeshRenderer>().material = selectColor;
    }

    void ResetActivate()
    {
        this.GetComponent<MeshRenderer>().material = deselectColor;
        activating = false;
        activateTick = 0f;
    } 
}
