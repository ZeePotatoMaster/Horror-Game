 using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class FlashCam : NetworkBehaviour
{
    [SerializeField] private float resetSpeed = 1f;
    [SerializeField] private float chargeTime = 1f;
    private float currentCharge = 0f;

    private bool canAttack = true;
    private bool charging = false;
    private PlayerBase pb;

    const string IDLE = "Idle";
    const string WALK = "Walk";
    private string currentAnimationState;

    //flash
    [SerializeField] private GameObject flashScreen;

    [SerializeField] private Transform rightIKTarget, leftIKTarget, viewmodel, worldmodel;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner) {
            viewmodel.gameObject.SetActive(false);
            return;
        }
        pb = this.transform.parent.GetComponent<PlayerBase>();
        Debug.Log(pb);
        viewmodel.SetParent(pb.playerCamera.transform, false);
        worldmodel.GetComponentInChildren<MeshRenderer>().enabled = false;
        flashScreen = Instantiate(flashScreen, GameObject.Find("Canvas").transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        if (pb.attacked) Attack();
        else if (!pb.attacked && charging) {
            currentCharge = 0f;
            charging = false;
        }

        SetAnimations();
    }

    private void Attack()
    {
        if (!canAttack) return;

        ChangeAnimationState("Charging");
        charging = true;

        currentCharge += Time.deltaTime;
        if (currentCharge >= chargeTime)
        {
            AttackCollider();
            currentCharge = 0f;
            charging = false;
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
    }

    private void AttackCollider()
    {
        canAttack = false;
        Invoke(nameof(ResetAttack), resetSpeed);

        var color = flashScreen.GetComponent<Image>().color;
        color.a = 1f;
        flashScreen.GetComponent<Image>().color = color;
        
        RaycastHit hit;
        if (Physics.SphereCast(pb.playerCamera.transform.position, 0.5f, pb.playerCamera.transform.forward, out hit))
        {
            if (hit.distance > 7) return;

            GameObject p = hit.transform.gameObject;

            if (hit.transform.tag != "Painting") return;

            Paintings.instance.GetComponent<Paintings>().shotPaintings++;
            hit.transform.tag = "Untagged";
        }
    }

    private void SetAnimations()
    {
        if (pb != null) {
            pb.RHandTarget.SetPositionAndRotation(rightIKTarget.position, rightIKTarget.rotation);
            pb.LHandTarget.SetPositionAndRotation(leftIKTarget.position, leftIKTarget.rotation);
        }

        if (charging) return;
        
        if (Mathf.Abs(pb.moveDirection.x) > 0.1f || Mathf.Abs(pb.moveDirection.z) > 0.1f) ChangeAnimationState(WALK);
        else ChangeAnimationState(IDLE);
    }

    private void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return;

        Debug.Log(newState);
        currentAnimationState = newState;
        worldmodel.GetComponent<Animator>().CrossFadeInFixedTime(newState, 0.2f);
        viewmodel.GetComponentInChildren<Animator>().CrossFadeInFixedTime(newState + " View", 0.2f);
    }

    void OnEnable(){
        if (!IsOwner) return;
        viewmodel.gameObject.SetActive(true);
        if (!viewmodel.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName(currentAnimationState + " View")) viewmodel.GetComponentInChildren<Animator>().CrossFadeInFixedTime(currentAnimationState + " View", 0.2f);
    }

    void OnDisable(){
        if (!IsOwner) return;
        viewmodel.gameObject.SetActive(false);
    }

    void OnNetworkDestroy ()
    {
        Destroy(viewmodel.gameObject);
    }
}
