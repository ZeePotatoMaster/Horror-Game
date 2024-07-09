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
    private GameObject eatIcon;

    private void Awake()
    {
        roleName = "Corpse Eater";
        isHuman = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        pb = GetComponent<PlayerBase>();
        GameObject canvas = GameObject.Find("Canvas");
        eatIcon = canvas.transform.Find("EatIcon").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (pb.lootObject.gameObject.tag == "corpse")
        {
            isEating = true;

            eatTick += Time.deltaTime;
            eatIcon.GetComponent<Image>().fillAmount = eatTick/eatTime;

            if (eatTick >= eatTime)
            {
                EndEat();
            }
        }
        else if (pb.lootObject == null && isEating) EndEat();
    }

    private void EndEat()
    {
        eatTick = 0;
        eatIcon.GetComponent<Image>().fillAmount = 0f;
        isEating = false;
    }

    private void Transform()
    {
        
    }
}
