using System;
using Unity.Netcode;
using Unity.Netcode.Samples;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]
public class PlayerControlAuthorative : NetworkBehaviour {
    [Header("Initialization Variables")]
    [SerializeField] private Vector3 defaultInitialPosition;

    [Header("Movement Variables")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeedMultiplier;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float groundForce;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDistance;

    [Header("Rotation Variables")]
    [SerializeField] private float sensitivityX;
    [SerializeField] private float sensitivityY;
    [SerializeField] private Vector2 clamp;
    [SerializeField] private Transform cameraTransform;

    [Header("Animation Variables")]
    [SerializeField] private NetworkVariable<bool> networkPlayerJumping = new NetworkVariable<bool>();
    [SerializeField] private NetworkVariable<bool> networkPlayerSprinting = new NetworkVariable<bool>();
    [SerializeField] private NetworkVariable<float> networkPlayerHorizontal = new NetworkVariable<float>();
    [SerializeField] private NetworkVariable<float> networkPlayerVertical = new NetworkVariable<float>();

    private bool oldPlayerJumping = false;
    private bool oldPlayerSprinting = false;
    private float oldPlayerHorizontal = 0f;
    private float oldPlayerVertical = 0f;

    private Animator animator;
    private CharacterController characterController;
    private float currentRotation, previousRotation;

    private bool inputJump;
    private bool inputSprint;
    private float inputHorizontal;
    private float inputVertical;
    private Vector3 movement;
    private float ySpeed;

    protected MeshRenderer Renderer;

    private void Awake() {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Renderer = GetComponentInChildren<MeshRenderer>();
        if (IsClient && IsOwner) {
            transform.position = defaultInitialPosition;
            PlayerCameraFollow.Instance.FollowPlayer(transform.Find("CameraRotate").Find("PlayerCameraRoot"));
        }
    }

    private void Update() { 
        if (IsClient && IsOwner) {
            ClientMovement();
            ClientRotation();
        }

        ClientVisuals();
    }

    private void ClientVisuals() {
        if (oldPlayerJumping != networkPlayerJumping.Value || oldPlayerSprinting != networkPlayerSprinting.Value || oldPlayerHorizontal != networkPlayerHorizontal.Value || oldPlayerVertical != networkPlayerVertical.Value) {
            oldPlayerJumping = networkPlayerJumping.Value;
            oldPlayerSprinting = networkPlayerSprinting.Value;
            oldPlayerHorizontal = networkPlayerHorizontal.Value;
            oldPlayerVertical = networkPlayerVertical.Value;
            animator.SetBool("Jumping", networkPlayerJumping.Value);
            animator.SetBool("Sprinting", networkPlayerSprinting.Value);
            animator.SetFloat("Horizontal", networkPlayerHorizontal.Value);
            animator.SetFloat("Vertical", networkPlayerVertical.Value);
            inputJump = false;
        }
    }

    private void ClientMovement() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            inputJump = true;
        } else if (Input.GetKeyUp(KeyCode.Space)) {
            inputJump = false;
        }

        inputSprint = Input.GetKey(KeyCode.LeftShift);

        inputVertical = 0f;
        inputHorizontal = 0f;

        if (Input.GetKey(KeyCode.W)) {
            inputVertical = 1f;
        } else if (Input.GetKey(KeyCode.S)) {
            inputVertical = -1f;
        }

        if (Input.GetKey(KeyCode.A)) {
            inputHorizontal = -1f;
        } else if (Input.GetKey(KeyCode.D)) {
            inputHorizontal = 1f;
        }

        movement = (transform.TransformDirection(Vector3.forward) * inputVertical + transform.TransformDirection(Vector3.right) * inputHorizontal).normalized * walkSpeed;

        bool isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && ySpeed < 0f) {
            ySpeed = -groundForce;
        }

        if (isGrounded && inputJump) {
            ySpeed = jumpHeight;
        }
        
        ySpeed -= gravity * Time.deltaTime;

        if (inputSprint) {
            movement *= sprintSpeedMultiplier;
        }

        movement.y = ySpeed;

        animator.SetBool("Jumping", inputJump);
        animator.SetBool("Sprinting", inputSprint);
        animator.SetFloat("Horizontal", inputHorizontal);
        animator.SetFloat("Vertical", inputVertical);

        characterController.Move(movement * Time.deltaTime);
        UpdatePlayerVisualsServerRpc(inputJump, inputSprint, inputHorizontal, inputVertical);
    }

    private void ClientRotation() {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = -Input.GetAxisRaw("Mouse Y");

        Vector2 rotation = new Vector2(
            mouseX * sensitivityX * Time.deltaTime,
            mouseY * sensitivityY * Time.deltaTime
        );

        transform.Rotate(0f, rotation.x, 0f);

        currentRotation = previousRotation + rotation.y;
        currentRotation = Mathf.Clamp(currentRotation, clamp.x, clamp.y);
        cameraTransform.Rotate(currentRotation - previousRotation, 0f, 0f);

        previousRotation = currentRotation;
    }

    [ServerRpc]
    public void UpdatePlayerVisualsServerRpc(bool playerJumping, bool playerSprinting, float playerHorizontal, float playerVertical) {
        networkPlayerJumping.Value = playerJumping;
        networkPlayerSprinting.Value = playerSprinting;
        networkPlayerHorizontal.Value = playerHorizontal;
        networkPlayerVertical.Value = playerVertical;
    }
}