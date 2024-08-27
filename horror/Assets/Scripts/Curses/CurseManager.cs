using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CurseManager : NetworkBehaviour
{
    public PlayerBase pb;
    public GameObject energyIcon;
    public NetworkVariable<float> curseEnergy = new NetworkVariable<float>(100f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private GameObject radialMenu;
    [HideInInspector] public Curse currentAbility;

    virtual public void Start()
    {
        if (!IsOwner) return;
        pb = this.GetComponent<PlayerBase>();
    }

    public void SetupCurses(CurseObject[] pCurseObjects, GameObject menuPrefab, GameObject energyPrefab)
    {
        GameObject canvas = GameObject.Find("Canvas");
        radialMenu = Instantiate(menuPrefab, canvas.transform, false);
        energyIcon = Instantiate(energyPrefab, canvas.transform, false);

        RadialMenu menuScript = radialMenu.GetComponent<RadialMenu>();
        menuScript.curseObjects = pCurseObjects;
        menuScript.Setup(this.gameObject);
    }

    public virtual void Update()
    {
        if (!IsOwner) return;

        if (pb.picking && !radialMenu.activeSelf) {
            radialMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            pb.lookSpeed = pb.lookSpeed / 4;
        }
        else if (!pb.picking && radialMenu.activeSelf) {
            radialMenu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            pb.lookSpeed = pb.lookSpeed * 4;
        } 

        if (currentAbility != null) {
            if (pb.casted && !currentAbility.activated && !this.GetComponent<InventoryManager>().HoldingSomething()) currentAbility.OnActivate(this.gameObject);
        }
    }

}
