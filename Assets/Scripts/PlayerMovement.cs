using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction jumpAction;
    private InputAction crouchAction;


    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float accel = 25f;
    [SerializeField] private float deccel = 35f;
    [SerializeField] private Transform visual;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float gravity = -25f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float crouchHeight = 1.1f;
    [SerializeField] private float crouchTransitionSpeed = 5f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float standingCameraHeight = 1.5f;
    [SerializeField] private float crouchCameraHeight = 0.75f;
    [SerializeField] private float cameraHeightSpeed = 4f;


    private Vector3 currentVelocity;
    private float verticalVelocity;
    private float standingHeight;
    private Vector3 standingCenter;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
        sprintAction = playerInput.actions.FindAction("Sprint");
        jumpAction = playerInput.actions.FindAction("Jump");
        crouchAction = playerInput.actions.FindAction("Crouch");
        standingHeight = controller.height;
        standingCenter = controller.center;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        Vector3 direction = forward * input.y + right * input.x;
        direction = Vector3.ClampMagnitude(direction, 1f);
        Vector3 targetVelocity;
        float targetSpeed;
        if (crouchAction.IsPressed())
        {
            targetSpeed = crouchSpeed;
        }else if (sprintAction.IsPressed())
        {
            targetSpeed = sprintSpeed;
        }
        else
        {
            targetSpeed = moveSpeed;
        }
        // if (sprintAction.IsPressed())
        // {
        //     targetVelocity= direction * sprintSpeed;
        // } else
        // {
        //     targetVelocity = direction * moveSpeed;
        // }
        targetVelocity = direction * targetSpeed;

        float rate = direction.sqrMagnitude > 0.001f ? accel : deccel;
        currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, rate * Time.deltaTime);
        float speed = new Vector2(currentVelocity.x, currentVelocity.z).magnitude;
        animator.SetFloat("Speed", speed);
        if (crouchAction.IsPressed())
        {
            animator.SetBool("Crouching", true);
        }else animator.SetBool("Crouching", false);
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            visual.rotation = Quaternion.RotateTowards(visual.rotation, targetRotation, rotationSpeed*Time.deltaTime);
        }
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        if (controller.isGrounded && jumpAction.WasPressedThisFrame())
        {
            verticalVelocity = Mathf.Sqrt(-2f * gravity * jumpHeight);
            animator.SetBool("Grounded", false);
            animator.SetTrigger("Jump");
        }
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 totalVelocity = currentVelocity;
        totalVelocity.y = verticalVelocity;
        float targetHeight = standingHeight;
        float targetCameraHeight;
        if (crouchAction.IsPressed())
        {
            targetHeight = crouchHeight;
            targetCameraHeight = crouchCameraHeight;
        } else targetCameraHeight = standingCameraHeight;

        Vector3 cameraPosition = cameraTarget.localPosition;

        cameraPosition.y = Mathf.MoveTowards(
            cameraPosition.y,
            targetCameraHeight,
            cameraHeightSpeed * Time.deltaTime
        );

        cameraTarget.localPosition = cameraPosition;
        
        controller.height = Mathf.MoveTowards(controller.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
        float heightDifference = standingHeight - controller.height;
        controller.center = standingCenter - Vector3.up * (heightDifference / 2f);
        controller.Move(totalVelocity*Time.deltaTime);
        animator.SetBool("Grounded", controller.isGrounded);
    }
}
