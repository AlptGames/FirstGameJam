using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 7f;
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;

    [Header("Звуки шагов (Петли/Loops)")]
    public AudioSource footstepSource;    // Сюда перетащи AudioSource
    public AudioClip walkLoop;            // Дорожка с ходьбой
    public AudioClip runLoop;             // Дорожка с бегом

    private Rigidbody rb;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentSpeed = walkSpeed;

        // Важные настройки для зацикливания
        if (footstepSource != null)
        {
            footstepSource.loop = true;
            footstepSource.playOnAwake = false;
        }
    }

    void Update()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");
        Vector3 movement = transform.right * horizontalMovement + transform.forward * verticalMovement;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        currentSpeed = isRunning ? runSpeed : walkSpeed;

        rb.velocity = new Vector3(movement.x * currentSpeed, rb.velocity.y, movement.z * currentSpeed);

        // Управление звуковыми дорожками
        HandleFootstepLoops(movement, isRunning);

        // Прыжок
        if (Input.GetButtonDown("Jump") && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Вращение (осталось без изменений)
        RotateCamera();

        if (Input.GetKeyDown(KeyCode.F)) ToggleFlashlight();
    }

    void HandleFootstepLoops(Vector3 moveDir, bool isRunning)
    {
        // Условие: мы жмем кнопки движения И мы на земле
        bool isMoving = moveDir.magnitude > 0.1f && Mathf.Abs(rb.velocity.y) < 0.1f;

        if (isMoving)
        {
            AudioClip targetClip = isRunning ? runLoop : walkLoop;

            // Если звук еще не играет ИЛИ сменился режим (ходьба/бег)
            if (!footstepSource.isPlaying || footstepSource.clip != targetClip)
            {
                footstepSource.clip = targetClip;
                footstepSource.Play();
            }
        }
        else
        {
            // Если остановились или прыгнули — выключаем звук
            if (footstepSource.isPlaying)
            {
                footstepSource.Stop();
            }
        }
    }

    // Вынес вращение в отдельный метод для чистоты
    void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);

        float currentCameraRotationX = cameraTransform.localEulerAngles.x;
        if (currentCameraRotationX > 180) currentCameraRotationX -= 360;
        float newCameraRotationX = Mathf.Clamp(currentCameraRotationX - mouseY, -90f, 90f);
        cameraTransform.localEulerAngles = new Vector3(newCameraRotationX, 0f, 0f);
    }

    void ToggleFlashlight()
    {
        Light flashlight = GetComponentInChildren<Light>();
        if (flashlight != null) flashlight.enabled = !flashlight.enabled;
    }
}
