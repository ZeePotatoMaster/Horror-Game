using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class ConsoleButton : Interactable
{
    [SerializeField] private Console console;

    [SerializeField] private Material selectColor;
    [SerializeField] private Material deselectColor;

    [SerializeField] private TMP_Text consoleText;
    [SerializeField] private string buttonName;

    protected virtual void Start()
    {
        consoleText.text = buttonName;
    }

    public void Select(bool select)
    {
        Material m = select ? selectColor : deselectColor;
        this.GetComponent<MeshRenderer>().material = m;

        string s = select ? ">" : "";
        consoleText.text = buttonName + s;
    }

    public override void FinishInteract(GameObject player)
    {
        console.ChangeSelection(this);
    }

    public abstract void Activate();
}
