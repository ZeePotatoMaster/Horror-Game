using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class RoleClass : NetworkBehaviour
{
    [HideInInspector] public string roleName = "Base Role";
    [HideInInspector] public bool isHuman;
    [SerializeField] private GameObject text;
    public PlayerBase pb;
    [SerializeField] private float energyEatTime = 1f;
    public GameObject energyIcon;
    public NetworkVariable<float> curseEnergy = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private GameObject radialMenu;
    private bool picking;
    private bool casted;
    [HideInInspector] public Curse currentAbility;
    [SerializeField] private CurseObject[] curseObjects;

    virtual public void Start()
    {
        if (!IsOwner) return;
        GameObject canvas = GameObject.Find("Canvas");
        GameObject roleText = Instantiate(text, canvas.transform, false);
        roleText.GetComponent<TextMeshProUGUI>().SetText("You are: " + roleName);
        StartCoroutine(DestroyObject(roleText));
        energyIcon = Instantiate(energyIcon, canvas.transform, false);

        radialMenu = Instantiate(radialMenu, canvas.transform, false);
        radialMenu.GetComponent<RadialMenu>().curseObjects = curseObjects;
        radialMenu.GetComponent<RadialMenu>().Setup(this.gameObject);
    }

    public void OnPickCurse(InputAction.CallbackContext context) {
        picking = context.action.triggered;
    }
    public void OnCast(InputAction.CallbackContext context) {
        casted = context.action.triggered;
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

        if (picking && !radialMenu.activeSelf) {
            radialMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pb.lookSpeed = pb.lookSpeed / 4;
        }
        else if (!picking && radialMenu.activeSelf) {
            radialMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            pb.lookSpeed = pb.lookSpeed * 4;
        } 

        if (currentAbility != null) {
            if (casted && !currentAbility.activated && !this.GetComponent<InventoryManager>().HoldingSomething()) currentAbility.OnActivate(this.gameObject);
        }

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
