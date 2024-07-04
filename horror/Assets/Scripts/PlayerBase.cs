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
    [SerializeField] private float jumpSpeed = 8.0f;
    [SerializeField] private float gravity = 20.0f;
    public Camera playerCamera;
    [SerializeField] private float lookSpeed = 2.0f;
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

    [SerializeField] private bool canMove = true;

    private Vector2 movementInput = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;
    private bool jumped = false;
    [HideInInspector] public bool attacked = false;
    [HideInInspector] public bool reloaded = false; 

    //looting
    private bool interacted = false;
    private float lootTimer = 0f;
    private bool isLooting = false;
    [HideInInspector]
    public bool lootCompleted = false;

    private GameObject lootImage;

    //listener (to disable for multiplayer)
    [SerializeField] private AudioListener listener;

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
    }

    //player inputs

    public void OnMove(InputAction.CallbackContext context) {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context) {
        attacked = context.action.triggered;
    }

    public void OnReload(InputAction.CallbackContext context) {
        reloaded = context.action.triggered;
    }

    public void OnJump(InputAction.CallbackContext context) {
        jumped = context.action.triggered;
    }

    public void OnLook(InputAction.CallbackContext context) {
        lookInput = context.ReadValue<Vector2>();
    }
    
    public void OnInteract(InputAction.CallbackContext context) {
        context.action.performed += context => interacted = true;
        context.action.canceled += context => interacted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        //movespeed from input
        float curSpeedX = canMove ? (currentSpeed) * movementInput.y : 0;
        float curSpeedY = canMove ? (currentSpeed) * movementInput.x : 0;
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
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -lookInput.y * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, lookInput.x * lookSpeed, 0);
        }

        if (canUseHeadBob && canMove) HandleHeadBob();

        //loot
        if (interacted == true) Loot();
        if (isLooting && interacted == false) EndLoot();
    }

    void HandleHeadBob()
    {
        if (!characterController.isGrounded) return;

        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * walkBobSpeed;
            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, defaultYPos + Mathf.Sin(timer) * walkBobAmount, playerCamera.transform.localPosition.z);
        }
    }

    private void Loot()
    {
        RaycastHit looty;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out looty))
        {
            float dist = Vector3.Distance(looty.transform.position, transform.position);
            if (dist < 2.5)
            {
                if (looty.transform.GetComponent<Item>() != null)
                {
                    float lootTime = looty.transform.GetComponent<Item>().lootTime;

                    isLooting = true;

                    lootTimer += Time.deltaTime;
                    lootImage.GetComponent<Image>().fillAmount = lootTimer/lootTime;

                    if (lootTimer >= lootTime)
                    {
                        looty.transform.GetComponent<Item>().OnPickup();
                        EndLoot();
                    }
                }

                else if (looty.transform.GetComponent<Roles>() != null)
                {
                    looty.transform.GetComponent<Roles>().AssignRoles();
                }
            }

            else if (dist >= 2.5 && isLooting)
            {
                EndLoot();
            }
        }
    }

    private void EndLoot()
    {
        lootTimer = 0;
        lootImage.GetComponent<Image>().fillAmount = 0f;
        isLooting = false;
    }


}
