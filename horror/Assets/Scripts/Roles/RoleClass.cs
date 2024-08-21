using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RoleClass : NetworkBehaviour
{
    [HideInInspector] public string roleName = "Base Role";
    [HideInInspector] public bool isHuman;
    [SerializeField] private GameObject text;
    public PlayerBase pb;
    [SerializeField] private float energyEatTime = 1f;
    public GameObject energyIcon;
    public NetworkVariable<float> curseEnergy = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    virtual public void Start()
    {
        if (!IsOwner) return;
        GameObject canvas = GameObject.Find("Canvas");
        GameObject roleText = Instantiate(text, canvas.transform, false);
        roleText.GetComponent<TextMeshProUGUI>().SetText("You are: " + roleName);
        StartCoroutine(DestroyObject(roleText));
        energyIcon = Instantiate(energyIcon, canvas.transform, false);
    }

    private IEnumerator DestroyObject(GameObject o)
    {
        yield return new WaitForSeconds(5);

        Destroy(o);
    }

    public virtual void Update()
    {
        if (!IsOwner) return;

        if (isHuman) return;

        if (pb.interactObject != null) {
            if (pb.interactObject.tag == "Energy") {
                pb.isInteracting = true;

                pb.interactTick += Time.deltaTime;
                energyIcon.GetComponent<Image>().fillAmount = pb.interactTick/energyEatTime;
                if (pb.interactTick >= energyEatTime) {
                    pb.EndInteract();
                }
            }
        }
        if (energyIcon.GetComponent<Image>().fillAmount > 0f && !pb.isInteracting) energyIcon.GetComponent<Image>().fillAmount = 0f;
    }

}
