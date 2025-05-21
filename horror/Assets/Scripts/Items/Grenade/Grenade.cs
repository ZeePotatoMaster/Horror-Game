using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Grenade : NetworkBehaviour
{
    private ItemInSlot slotItem;

    [SerializeField] private Transform rightIKTarget, leftIKTarget, viewmodel, worldmodel;
    private PlayerBase pb;
    [SerializeField] private NetworkObject grenadePrefab;
    [SerializeField] private float startingForce = 1f;
    [SerializeField] private float forceIncreaseMultiplier = 5f;
    [SerializeField] private float maxForce = 20f;
    private float force;

    private bool readied = false;

    private bool canAttack = true;
    [SerializeField] private float attackResetTime;

    
    private bool canLaunch = false;
    [SerializeField] private float launchReadyTime;
    private bool launch = false;

    const string IDLE = "Idle";
    const string WALK = "Walk";
    const string READY = "Ready";
    const string LAUNCH = "Throw";
    const string START = "Start";
    private string currentAnimationState;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner)
        {
            viewmodel.gameObject.SetActive(false);
            return;
        }

        //find base
        pb = transform.parent.gameObject.GetComponent<PlayerBase>();

        //viewmodel
        viewmodel.SetParent(pb.playerCamera.transform, false);

        if (worldmodel.GetComponent<MeshRenderer>() != null) worldmodel.GetComponent<MeshRenderer>().enabled = false;
        foreach (MeshRenderer e in worldmodel.GetComponentsInChildren<MeshRenderer>())
        {
            e.enabled = false;
        }

        force = startingForce;
    }

    public void SetSlotItem(ItemInSlot s)
    {
        slotItem = s;
    }

    public int GetGrenadeAmount()
    {
        return slotItem.number;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        if (readied) force += Time.deltaTime * forceIncreaseMultiplier;
        if (force > maxForce) force = maxForce;

        if (pb.attacked && !readied && canAttack) ReadyGrenade();
        if (!pb.attacked && readied) launch = true;
        if (launch && canLaunch) LaunchGrenade();

        SetAnimations();
    }

    void ReadyGrenade()
    {
        Debug.Log("readying");
        readied = true;
        ChangeAnimationState(READY);
        Invoke(nameof(ReadyLaunch), launchReadyTime);
    }

    void ReadyLaunch()
    {
        Debug.Log("ready");
        canLaunch = true;
    }

    void LaunchGrenade()
    {
        Debug.Log("launching");
        ChangeAnimationState(LAUNCH);
        Transform c = pb.playerCamera.transform;
        LaunchGrenadeRpc(c.position, c.rotation, c.forward, force);

        slotItem.number--;

        //end
        readied = false;
        canAttack = false;
        launch = false;
        canLaunch = false;
        force = startingForce;
        Invoke(nameof(ResetAttack), attackResetTime);
    }

    [Rpc(SendTo.Server)]
    void LaunchGrenadeRpc(Vector3 pos, Quaternion rot, Vector3 forward, float force)
    {
        NetworkObject grenade = Instantiate(grenadePrefab, pos, rot);
        grenade.Spawn();
        grenade.GetComponent<Rigidbody>().AddForce(grenade.transform.forward * force, ForceMode.Impulse);
    }

    void ResetAttack()
    {
        if (slotItem.number <= 0) transform.parent.GetComponent<InventoryManager>().DropItem();
        Debug.Log("ready to attack again");
        canAttack = true;
        ChangeAnimationState(START);
    }

    private void SetAnimations()
    {
        pb.RHandTarget.SetPositionAndRotation(rightIKTarget.position, rightIKTarget.rotation);
        pb.LHandTarget.SetPositionAndRotation(leftIKTarget.position, leftIKTarget.rotation);

        if (worldmodel.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName(START)) return;
        if (currentAnimationState == LAUNCH || readied) return;

        if (Mathf.Abs(pb.moveDirection.x) > 0.1f || Mathf.Abs(pb.moveDirection.z) > 0.1f) ChangeAnimationState(WALK);
        else ChangeAnimationState(IDLE);
    }
    
    private void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return;

        Debug.Log(newState);
        currentAnimationState = newState;
        worldmodel.GetComponent<Animator>().CrossFadeInFixedTime(newState, 0.2f);
        viewmodel.GetComponent<Animator>().CrossFadeInFixedTime(newState + "view", 0.2f);
    }

    void OnEnable()
    {
        if (!IsOwner) return;
        viewmodel.gameObject.SetActive(true);
        readied = false;
        canAttack = true;
        launch = false;
        canLaunch = false;
        force = startingForce;
        if (slotItem == null) return;
        if (slotItem.number <= 0) transform.parent.GetComponent<InventoryManager>().DropItem();
    }

    void OnDisable(){
        if (!IsOwner) return;
        viewmodel.gameObject.SetActive(false);
    }
}
