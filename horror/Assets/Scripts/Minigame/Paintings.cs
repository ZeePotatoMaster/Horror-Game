using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Paintings : NetworkBehaviour
{
    public static Paintings instance;
    [HideInInspector] public NetworkVariable<int> totalPaintings = new NetworkVariable<int>(1);
    [HideInInspector] public int shotPaintings = 0;
    [SerializeField] private GameObject ui;
    private TMP_Text uit;

    void Awake()
    {
        if (instance == null) instance = this;
        uit = Instantiate(ui, GameObject.Find("Canvas").transform).GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        uit.text = shotPaintings + " / " + totalPaintings.Value + " paintings";
        
        //if (shotPaintings == totalPaintings) 
    }
}
