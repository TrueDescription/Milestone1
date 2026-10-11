using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraTarget;

    [SerializeField] private float mouseSens = 0.15f;
    [SerializeField] private float gamepadSens = 150f;

    [SerializeField] private float minPitch = -35f;
    [SerializeField] private float maxPitch = 65f;

    private PlayerInput playerInput;
    private InputAction lookAction;

    private float yaw;
    private float pitch;

    // private void Start()
    // {
    //     playerInput = GetComponent<PlayerInput>();
    //     // lookAction = playerInput.actions.FindAction("Look");
    //     lookAction = playerInput.actions.FindAction("Player/Look", true);

    //     yaw = cameraTarget.eulerAngles.y;
    //     pitch = cameraTarget.eulerAngles.x;

    //     Cursor.lockState = CursorLockMode.Locked;
    //     Cursor.visible = false;
    // }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        lookAction = playerInput.actions.FindAction("Player/Look");

        yaw = cameraTarget.eulerAngles.y;
        pitch = cameraTarget.eulerAngles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        bool usingMouse = lookAction.activeControl?.device is Mouse;
        float sens;
        if (usingMouse)
        {
            sens = mouseSens;
        } else sens = gamepadSens * Time.deltaTime;
        yaw += lookInput.x * sens;
        pitch -=lookInput.y * sens;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraTarget.rotation = Quaternion.Euler(pitch, yaw, 0f);

    }
}