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
    public FixedJoystick aimJoystick;      // Joystick derecho (solo para disparo)
    private float xRotation = 0f;

    [SerializeField] private float aimDeadZone = 0.2f;
    [SerializeField] private PlayerShooting shootingScript;

    void Update()
    {
        // Movimiento con joystick izquierdo
        float horizontal = movementJoystick.Horizontal;
        float vertical = movementJoystick.Vertical;

        Vector3 direction = transform.right * horizontal + transform.forward * vertical;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // --- CONTROL DE CÁMARA (por touch tradicional, como antes) ---
        if (Input.touchCount > 0)
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

        // --- CONTROL DE DISPARO con aimJoystick ---
        Vector2 aimInput = new Vector2(aimJoystick.Horizontal, aimJoystick.Vertical);

        if (aimInput.magnitude > aimDeadZone)
        {
            shootingScript.ShootButtonPressed();
        }
    }
}
