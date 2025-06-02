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

    // Update is called once per frame
    void Update()
    {
        if (ps.power == 0)
        {
            if (monitor.enabled) monitor.enabled = false;
            return;
        }
    }

    public void ChangeSelection(ConsoleButton cs)
    {
        if (activeButton == cs) return;
        cs.Select(true);

        if (activeButton != null) activeButton.Select(false);

        activeButton = cs;
    }

    public override void FinishInteract(GameObject player)
    {
        activeButton.Activate();
        this.GetComponent<MeshRenderer>().material = selectColor;
        Invoke(nameof(Reset), 2f);
    }

    void Reset()
    {
        this.GetComponent<MeshRenderer>().material = deselectColor;
    } 
}
