using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Gasoline : NetworkBehaviour
{
    [SerializeField] private float totalGas;
    [HideInInspector] public float currentGas;
    [SerializeField] private string genTag;
    [SerializeField] private GameObject gasBar;
    private PlayerBase pb;
    // Start is called before the first frame update
    void Start()
    {
        pb = transform.parent.GetComponent<PlayerBase>();
        gasBar = Instantiate(gasBar, GameObject.Find("Canvas").transform);
    }

    void OnDisable()
    {
        gasBar.SetActive(false);
    }

    void OnEnable()
    {
        gasBar.SetActive(true);
    }

    void OnNetworkDestroy()
    {
        Destroy(gasBar);
    }

    // Update is called once per frame
    void Update()
    {
        if (pb.interactObject != null)
        {
            if (pb.interactObject.tag == genTag && pb.interacted) pb.interactObject.GetComponent<Generator>().FillGenerator(this);
        }

        gasBar.GetComponent<Image>().fillAmount = currentGas / totalGas;

        if (currentGas <= 0) transform.parent.GetComponent<InventoryManager>().DropItem();
    }
}
