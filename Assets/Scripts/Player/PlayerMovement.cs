using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [Header("Config Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private Transform cameraTransform;

    [Header("Mobile Input")]
    public FixedJoystick movementJoystick; // Joystick izquierdo
    public FixedJoystick fire1Joystick;      // Joystick derecho

    private float xRotation = 0f;
    [SerializeField] private float aimDeadZone = 0.2f;

    [SerializeField] private PlayerShooting shootingScript;

    

    void Update()
    {
        // --- Movimiento con joystick izquierdo ---
        float horizontal = movementJoystick.Horizontal;
        float vertical = movementJoystick.Vertical;

        Vector3 direction = transform.right * horizontal + transform.forward * vertical;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // --- Obtener entrada del joystick derecho ---
        Vector2 aimInput = new Vector2(fire1Joystick.Horizontal, fire1Joystick.Vertical);

        bool usingfire1Joystick = aimInput.magnitude > aimDeadZone;

        // --- Rotación con joystick derecho si se está usando ---
        if (usingfire1Joystick)
        {
            float deltaX = fire1Joystick.Horizontal * lookSpeed;
            float deltaY = fire1Joystick.Vertical * lookSpeed;

            xRotation -= deltaY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * deltaX);

            shootingScript.ShootButtonPressed();
        }
        // --- Si no se usa el joystick, permitir control con touch ---
        else if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    if (touch.phase == TouchPhase.Moved)
                    {
                        Vector2 delta = touch.deltaPosition * lookSpeed * Time.deltaTime;

                        xRotation -= delta.y;
                        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

                        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                        transform.Rotate(Vector3.up * delta.x);
                    }
                }
            }
        }
    }
}
