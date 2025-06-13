using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLureButton : Interactable
{
    [SerializeField] private CamSystem cs;

    private int luresLeft;
    [SerializeField] private int lureAmount;
    [SerializeField] private int lureRandom;
    private bool canLure = true;
    private bool lureCooldown = false;

    [SerializeField] private Material errorColor;
    [SerializeField] private Material activateColor;
    private Material goodColor;

    void Start()
    {
        goodColor = this.GetComponent<MeshRenderer>().material;
        RebootLures();
    }

    public override void FinishInteract(GameObject player)
    {
        if (!canLure || lureCooldown) return;

        luresLeft--;
        cs.StartLure();

        this.GetComponent<MeshRenderer>().material = activateColor;

        lureCooldown = true;
        Invoke(nameof(ResetCooldown), 1f);
    }

    public void BreakLures()
    {
        canLure = false;
        this.GetComponent<MeshRenderer>().material = errorColor;
    }

    public void RebootLures()
    {
        canLure = true;

        int r = Random.Range(-lureRandom, lureRandom);
        luresLeft = r + lureAmount;

        this.GetComponent<MeshRenderer>().material = goodColor;
    }

    void ResetCooldown()
    {
        lureCooldown = false;
        this.GetComponent<MeshRenderer>().material = goodColor;

        if (luresLeft <= 0) BreakLures();
    }
}
