using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorpseEater : RoleClass
{
    [SerializeField] private float eatTime;
    private bool isEating = false;
    private PlayerBase pb;
    private float eatTick = 0;
    [SerializeField] private GameObject eatIcon;
    private bool canEat;

    private void Awake()
    {
        roleName = "Corpse Eater";
        isHuman = false;
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        pb = GetComponent<PlayerBase>();
        GameObject canvas = GameObject.Find("Canvas");
        eatIcon = Instantiate(eatIcon, canvas.transform, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (pb.interacted && canEat) Eat();
        if (isEating && !pb.interacted) EndEat();
        if (!canEat && !pb.interacted) EndEat();
    }

    private void EndEat()
    {
        eatTick = 0;
        eatIcon.GetComponent<Image>().fillAmount = 0f;
        isEating = false;
        canEat = true;
    }

    private void Eat()
    {
        RaycastHit looty;
        if (Physics.Raycast(pb.playerCamera.transform.position, pb.playerCamera.transform.forward, out looty))
        {
            float dist = Vector3.Distance(looty.transform.position, transform.position);
            if (dist <= 2.5) {
                if (looty.transform.tag == "Corpse")
                {
                    isEating = true;

                    eatTick += Time.deltaTime;
                    eatIcon.GetComponent<Image>().fillAmount = eatTick/eatTime;

                    if (eatTick >= eatTime)
                    {
                        EndEat();
                    }
                }
                else {
                    Debug.Log("nope eat");
                    canEat = false;
                    return;
                }
            }

            else if (dist >= 2.5 && isEating)
            {
                EndEat();
            }
            else
            {
                Debug.Log("nope eat");
                canEat = false;
                return;
            }
        }
        else
        {
            if (isEating) EndEat();
            Debug.Log("nope eat");
            canEat = false;
        }
    }
}
