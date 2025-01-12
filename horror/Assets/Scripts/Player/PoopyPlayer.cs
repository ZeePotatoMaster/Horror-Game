using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

[RequireComponent(typeof(CharacterController))]

public class PoopyPlayer : MonoBehaviour
{
    //Camera Vars
    public Camera playerCamera;
    [SerializeField] private GameObject cameraRoot;
    [SerializeField] private float lookSpeed = 2.0f;
    [SerializeField] private float lookXLimit = 90.0f;

    public float maxHorizontalTilt = 2.0f;
    public float tiltAcceleration = 5.0f;
    [HideInInspector] private float currentHorizontalTilt = 0f;

    //Character Controller
    private CharacterController characterController;
    [HideInInspector] public Vector3 moveDirection = Vector3.zero;

    //Headbob Vars
    [SerializeField] private bool canUseHeadBob = true;
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    private float defaultYPos = 0;
    private float timer;
    
    //Animation Vars
    public Animator playerAnimator;

    //Movement Vars
    private Vector2 movementInput = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;
    private bool jumped = false;
    [HideInInspector] public float currentSpeed;
    [HideInInspector] private float currentSprintMultiplier;

    //Movement Presets

    [SerializeField] private float walkingSpeed = 7.5f;
    [SerializeField] private float jumpSpeed = 10.0f;
    [SerializeField] private float gravity = 35.0f;
    [SerializeField] private float sprintingMultiplier = 1.5f;
    private float rotationX = 0;

    //Player States
    
    [SerializeField] private bool canMove = true;
    private bool isMoving = false;
    private bool isJumping = false;
    private bool isSprinting = false;
    [HideInInspector] public bool attacked = false;
    [HideInInspector] public bool reloaded = false; 

    private GameObject lootImage;

    //listener (to disable for multiplayer)
    [SerializeField] private AudioListener listener;
    private float animatedWalkSpeed = 2f;
    private int animatorX;
    private int animatorY;
    private int jumpAnim;
    private int groundedAnim;
    private int fallingAnim;
    private float changeTime;
   
    private Vector2 change;
    private float animateTimer = 1;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentSpeed = walkingSpeed;

        //headbob default camera pos
        defaultYPos = playerCamera.transform.localPosition.y;

        animatorX = Animator.StringToHash("X_Velocity");
        animatorY = Animator.StringToHash("Y_Velocity");
        jumpAnim = Animator.StringToHash("Jump");
        groundedAnim = Animator.StringToHash("Grounded");
        fallingAnim = Animator.StringToHash("Falling");
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

    public void OnSprint(InputAction.CallbackContext context) {
        isSprinting = context.action.triggered;
    }

    public void OnCrouch(InputAction.CallbackContext context) {

    }

    public void OnJump(InputAction.CallbackContext context) {
        jumped = context.action.triggered;
        isJumping = context.action.triggered;
    }

    public void OnLook(InputAction.CallbackContext context) {
        lookInput = context.ReadValue<Vector2>();
    }
    
    public void OnInteract(InputAction.CallbackContext context) {
    }

    // Update is called once per frame
    void Update()
    {

        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        //movespeed from input

        currentSprintMultiplier = 1f;

        if (isSprinting) {

            currentSprintMultiplier = sprintingMultiplier;
        }

        float curSpeedX = canMove ? (currentSpeed * currentSprintMultiplier) * movementInput.y : 0;
        float curSpeedY = canMove ? (currentSpeed * currentSprintMultiplier) * movementInput.x : 0;
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

        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f) {
            isMoving = true;
        } else {
            isMoving = false;
        }

        updateAnimations();

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
            updateCamera();
        }

        updateAnimatorParameters();
    }

    void updateCamera() {

        rotateCamera();

        if (canUseHeadBob) {
            handeHeadBob();
        }
    }

    void rotateCamera() {

        //Rotate player body
        rotationX += -lookInput.y * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        transform.rotation *= Quaternion.Euler(0, lookInput.x * lookSpeed, 0);
        
        //Detect any movement on the horizontal axis (A and D)
        //Constrain tilt to within the upper limit and apply the acceleration to get tilt smoothing
        float horizontalMovement = Input.GetAxisRaw("Horizontal");

        if (horizontalMovement != 0f) {

            float smoothTilt = 0f;
            smoothTilt = -horizontalMovement * maxHorizontalTilt;
            currentHorizontalTilt = Mathf.Lerp(currentHorizontalTilt, smoothTilt, Time.deltaTime * tiltAcceleration);
        } else {
            currentHorizontalTilt = Mathf.Lerp(currentHorizontalTilt, 0f, Time.deltaTime * tiltAcceleration);
        }

        //Combine camera tilt with the direction the players looking

        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, currentHorizontalTilt);
    }

    void handeHeadBob()
    {
        if (!characterController.isGrounded) return;

        if (isMoving)
        {
            timer += Time.deltaTime * (walkBobSpeed * currentSprintMultiplier);
            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, defaultYPos + Mathf.Sin(timer) * walkBobAmount, playerCamera.transform.localPosition.z);
        }
    }

    void updateAnimations() {

        changeTime = movementInput == Vector2.zero ? Time.deltaTime * 12 : Time.deltaTime * 4;

        change = Vector2.Lerp(change, isSprinting ? movementInput * 6 : movementInput * 2, changeTime);

        playerAnimator.SetFloat(animatorX, canMove ? change.x * 2 : 0);
        playerAnimator.SetFloat(animatorY, canMove ? change.y * 2 : 0);
    }

    void updateAnimatorParameters() {

        playerAnimator.SetBool(groundedAnim, characterController.isGrounded);
        playerAnimator.SetBool(jumpAnim, jumped);
        playerAnimator.SetBool(fallingAnim, !characterController.isGrounded);
    }
}
