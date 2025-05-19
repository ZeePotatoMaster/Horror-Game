using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.VisualScripting;

[RequireComponent(typeof(CharacterController))]

public class PlayerBase : NetworkBehaviour
{

    //movement vars
    [SerializeField] private float walkingSpeed = 7.5f;
    [SerializeField] private float sprintSpeed = 15f;
    [SerializeField] private float jumpSpeed = 8.0f;
    [SerializeField] private float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    [SerializeField] private float lookXLimit = 90.0f;

    private CharacterController characterController;
    [HideInInspector] public Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    [HideInInspector] public float currentSpeed;

    //headbob stuff
    [SerializeField] private bool canUseHeadBob = true;
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.5f;
    private float defaultYPos = 0;
    private float timer;
    private float knockbackY;
    private float knockbackZ;
    private float knockbackTimer;

    public bool canMove = true;
    public Vector3 lockPosition = Vector3.zero;

    private Vector2 movementInput = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;
    private bool jumped = false;
    private bool isSprinting = false;
    [HideInInspector] public bool attacked = false;
    [HideInInspector] public bool altAttacked = false;
    [HideInInspector] public bool reloaded = false;

    //looting
    [HideInInspector] public bool interacted = false;
    [HideInInspector] public float interactTick = 0f;
    [HideInInspector] public bool isInteracting = false;
    [HideInInspector] public bool canInteract = true;
    [HideInInspector] public Transform interactObject;

    [HideInInspector] public GameObject lootImage;

    //listener (to disable for multiplayer)
    [SerializeField] private AudioListener listener;

    //inventory
    private bool swappedWeapons;
    public bool canSwapWeapons = true;
    private int swapDirection = 0;
    private InventoryManager inventoryManager;
    private bool dropped;

    //spells
    [HideInInspector] public bool picking;

    //animator
    private Vector2 change;
    [SerializeField] private GameObject playerModel;
    private float changeTime;
    public Transform RHandTarget, LHandTarget;

    //camera shake
    private Vector3 originalPos;
    private float elapsed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner)
        {
            playerCamera.enabled = false;
            listener.enabled = false;
            return;
        }

        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentSpeed = walkingSpeed;

        //headbob default camera pos
        defaultYPos = playerCamera.transform.localPosition.y;

        GameObject canvas = GameObject.Find("Canvas");
        lootImage = canvas.transform.Find("LootIcon").gameObject;

        inventoryManager = GetComponent<InventoryManager>();

        //invisible playermodel
        int invisLayer = LayerMask.NameToLayer("Invisible");
        var children = playerModel.GetComponentsInChildren<Transform>();
        foreach (var child in children) child.gameObject.layer = invisLayer;

        //camera shake
        originalPos = playerCamera.transform.localPosition;
    }

    //player inputs

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.action.triggered;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        attacked = context.action.triggered;
    }

    public void OnAltAttack(InputAction.CallbackContext context)
    {
        altAttacked = context.action.triggered;
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        reloaded = context.action.triggered;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jumped = context.action.triggered;
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        dropped = context.action.triggered;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnPickCurse(InputAction.CallbackContext context)
    {
        picking = context.action.triggered;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        context.action.performed += context => interacted = true;
        context.action.canceled += context => interacted = false;
    }

    public void OnWeaponChange(InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<float>());
        if (context.ReadValue<float>() > 0)
        {
            swapDirection = 1;
        }

        else if (context.ReadValue<float>() == -120)
        {
            swapDirection = -1;
        }

        else if (context.ReadValue<float>() == 0)
        {
            swappedWeapons = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        currentSpeed = isSprinting ? sprintSpeed : walkingSpeed;

        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        //movespeed from input
        float curSpeedX = canMove ? currentSpeed * movementInput.y : 0;
        float curSpeedY = canMove ? currentSpeed * movementInput.x : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (jumped && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        if (characterController.enabled) characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -lookInput.y * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
            transform.rotation *= Quaternion.Euler(0, lookInput.x * lookSpeed, 0);
        }
        if (lockPosition != Vector3.zero && transform.position != lockPosition)
        {
            transform.position = lockPosition;
        }

        if (canUseHeadBob && canMove) HandleHeadBob();

        //loot
        if (interacted && canInteract) interactObject = Interact();

        if (interactObject == null)
        {
            canInteract = false;
            if (isInteracting) EndInteract();
        }
        else if (interactObject.GetComponent<Interactable>() != null) interactObject.GetComponent<Interactable>().OnInteract(this.gameObject);

        if (!interacted && (!canInteract || isInteracting)) EndInteract();

        //inventory
        if (swappedWeapons && canSwapWeapons)
        {
            inventoryManager.ChangeSelectedSlot(swapDirection);
            swapDirection = 0;
            swappedWeapons = false;
        }

        if (dropped) inventoryManager.DropItem();

        //anims
        updateAnimations();
        updateAnimatorParameters();
    }

    void HandleHeadBob()
    {
        if (!characterController.isGrounded) return;

        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            float bobSpeed = isSprinting ? walkBobSpeed * 1.4f : walkBobSpeed;
            timer += Time.deltaTime * bobSpeed;
            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, defaultYPos + Mathf.Sin(timer) * walkBobAmount, playerCamera.transform.localPosition.z);
        }

        Quaternion normal = Quaternion.Euler(rotationX, 0, 0);
        Quaternion knock = Quaternion.Euler(rotationX, knockbackY, knockbackZ);
        if (knockbackTimer > 0)
        {
            playerCamera.transform.localRotation = Quaternion.Lerp(normal, knock, knockbackTimer);
            knockbackTimer = Mathf.Clamp(knockbackTimer - Time.deltaTime, 0f, 1f);
        }
    }

    void updateAnimations()
    {

        changeTime = movementInput == Vector2.zero ? Time.deltaTime * 12 : Time.deltaTime * 4;

        change = Vector2.Lerp(change, isSprinting ? movementInput * 6 : movementInput * 2, changeTime);

        Animator animator = playerModel.GetComponent<Animator>();
        PlayerModel modelScript = playerModel.GetComponent<PlayerModel>();

        animator.SetFloat(modelScript.GetAnimInt(0), canMove ? change.x * 2 : 0);
        animator.SetFloat(modelScript.GetAnimInt(1), canMove ? change.y * 2 : 0);
    }

    void updateAnimatorParameters()
    {

        Animator animator = playerModel.GetComponent<Animator>();
        PlayerModel modelScript = playerModel.GetComponent<PlayerModel>();

        if (!jumped && animator.GetBool(modelScript.GetAnimInt(2))) animator.ResetTrigger(modelScript.GetAnimInt(2));
        if (jumped) animator.SetTrigger(modelScript.GetAnimInt(2));
        if (characterController.isGrounded && animator.GetBool(modelScript.GetAnimInt(3))) animator.ResetTrigger(modelScript.GetAnimInt(3));
        if (!characterController.isGrounded) animator.SetTrigger(modelScript.GetAnimInt(3));
    }

    /*public void AddCameraKnockback(Transform direction, float yAmount, float zAmount)
    {
        Vector3 localDirection = this.transform.InverseTransformPoint(direction.position);
        knockbackTimer = 1f;

        if (localDirection.x < 0 && Mathf.Abs(localDirection.z) < 1) knockbackY = 45f;
        else if (localDirection.x > 0 && Mathf.Abs(localDirection.z) < 1) knockbackY = -45f;

        else if (localDirection.z >= 1) {
            knockbackY = yAmount;
            knockbackZ = zAmount;
        }
        else if (localDirection.z <= -1) {
            knockbackY = -yAmount;
            knockbackZ = -zAmount;
        }
    }*/

    [ServerRpc(RequireOwnership = false)]
    public void CamKnockbackServerRpc(int directionY, int directionZ, float intensity, ulong id)
    {
        CamKnockbackClientRpc(directionY, directionZ, intensity, new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { id } } });
    }

    [ClientRpc]
    private void CamKnockbackClientRpc(int directionY, int directionZ, float intensity, ClientRpcParams clientRpcParams)
    {
        knockbackY = directionY * intensity;
        knockbackZ = directionZ * intensity;
        knockbackTimer = 1f;
    }

    private Transform Interact()
    {
        Debug.Log("started");
        RaycastHit looty;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out looty))
        {
            float dist = Vector3.Distance(looty.transform.position, transform.position);
            if (dist <= 2.5)
            {
                Debug.Log(looty.transform);
                return looty.transform;
            }

            else if (dist >= 2.5 && isInteracting)
            {
                EndInteract();
            }
        }
        return null;
    }

    public void EndInteract()
    {
        interactTick = 0;
        lootImage.GetComponent<Image>().fillAmount = 0f;
        isInteracting = false;
        canInteract = true;
        interactObject = null;
    }

    public void StartShake(float dur, float mag)
    {
        StartCoroutine(Shake(dur, mag));
    }

    //camera shake
    private IEnumerator Shake(float duration, float magnitude)
    {
        elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            playerCamera.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null; //before we continue to the next iteration of the while loop, we want to wait until the next frame is drawn
        }

        playerCamera.transform.localPosition = originalPos;
    }

    public void removeLockPosition()
    {
        lockPosition = Vector3.zero;
    }

    //when touching elevator
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.collider.transform.CompareTag("Elevator")) return;
        if (MinigameManager.instance == null) return;
        if (hit.collider.GetComponent<Elevator>().ownerid == OwnerClientId) MinigameManager.instance.OnPlayerEnterElevator(OwnerClientId);
    }
}
