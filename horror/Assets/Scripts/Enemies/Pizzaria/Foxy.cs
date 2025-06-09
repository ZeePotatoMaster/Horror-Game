using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foxy : MonoBehaviour
{
    [SerializeField] private float phaseChangeTime;
    [SerializeField] private float changeRandom;
    private float changeTime;
    private float currentchange;
    private int currentPhase = 0;

    [SerializeField] private Camera camWatching;
    [SerializeField] private Camera hallCam;
    [SerializeField] private Camera mainHallCam;
    [SerializeField] private LadderDoor ld;
    [SerializeField] private PowerDoor pd;

    private bool attacking = false;

    [SerializeField] LayerMask foxyMask;
    LayerMask normalMask;

    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {
        if (!camWatching.enabled) currentchange += Time.deltaTime;
        if (currentchange >= changeTime) ChangePhases();

        if (!attacking) return;

        if (ld.open && (mainHallCam.enabled || camWatching.enabled)) AttackMain();
        if (!ld.open && (hallCam.enabled || camWatching.enabled)) AttackHall();
    }

    void ChangePhases()
    {
        currentPhase++;
        if (currentPhase < 6) this.GetComponent<Animator>().Play("Phase" + currentPhase);

        currentchange = 0f;
        float r = Random.Range(-changeRandom, changeRandom);
        changeTime = phaseChangeTime + r;

        if (currentPhase == 5) StartAttack();
        if (currentPhase == 6)
        {
            if (ld.open) AttackMain();
            else AttackHall();
        }
    }

    void Reset()
    {
        currentchange = 0f;
        float r = Random.Range(-changeRandom, changeRandom);
        changeTime = phaseChangeTime + r;

        currentPhase = 0;
        this.GetComponent<Animator>().Play("Phase" + currentPhase);
    } 

    void StartAttack()
    {
        attacking = true;
    }

    void AttackHall()
    {
        attacking = false;

        hallCam.cullingMask = foxyMask;
        this.GetComponent<Animator>().Play("AttackHall");
        Invoke(nameof(CheckKill), 3f);
    }

    void AttackMain()
    {
        attacking = false;

        mainHallCam.cullingMask = foxyMask;
        this.GetComponent<Animator>().Play("AttackMain");
        Invoke(nameof(CheckKill), 3f);
    }

    void CheckKill()
    {
        if (currentPhase == 0) return;

        hallCam.cullingMask = normalMask;

        if (pd.isOpen) Pizzaria.instance.GetComponent<Pizzaria>().Kill(this.transform);
        else Reset();
    }

    public void OnShot()
    {
        mainHallCam.cullingMask = normalMask;
        Reset();
    }
}