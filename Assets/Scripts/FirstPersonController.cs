using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
     public float walkSpeed = 5f; // Скорость ходьбы
    public float runSpeed = 10f; // Скорость бега
    public float jumpForce = 7f; // Сила прыжка
    public float mouseSensitivity = 2f; // Чувствительность мыши для вращения камеры
    public Transform cameraTransform; // Ссылка на Transform камеры

    private Rigidbody rb; // Компонент Rigidbody для физики
    private float currentSpeed; // Текущая скорость движения

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody не найден на объекте!");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentSpeed = walkSpeed; // Изначально используем скорость ходьбы
    }

    void Update()
    {
        // Перемещение
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");
        Vector3 movement = transform.right * horizontalMovement + transform.forward * verticalMovement;

        // Определение скорости (ходьба или бег)
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed; // Игрок бежит
        }
        else
        {
            currentSpeed = walkSpeed; // Игрок идет
        }

        // Применяем скорость к Rigidbody
        rb.velocity = new Vector3(movement.x * currentSpeed, rb.velocity.y, movement.z * currentSpeed);

        // Прыжок
        if (Input.GetButtonDown("Jump") && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // Вращение головы (камеры)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Вращаем тело персонажа по горизонтали
        transform.Rotate(Vector3.up * mouseX);

        // Вращаем камеру по вертикали, ограничивая повороты
        // Проверяем текущий поворот камеры по оси X, чтобы ограничить движение
        float currentCameraRotationX = cameraTransform.localEulerAngles.x;
        // Если текущий поворот больше 180 (значит, смотрит вниз), приводим его к соответствующему отрицательному значению
        if (currentCameraRotationX > 180) 
        {
            currentCameraRotationX -= 360;
        }

        float newCameraRotationX = currentCameraRotationX - mouseY;
        newCameraRotationX = Mathf.Clamp(newCameraRotationX, -90f, 90f); // Ограничиваем наклон камеры
        cameraTransform.localEulerAngles = new Vector3(newCameraRotationX, 0f, 0f);

        // Активация/деактивация фонарика
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashlight();
        }
    }

    // Функция для переключения фонарика
    void ToggleFlashlight()
    {
        Light flashlight = GetComponentInChildren<Light>();
        if (flashlight != null)
        {
            flashlight.enabled = !flashlight.enabled;
        }
        else
        {
            Debug.LogWarning("Фонарик (компонент Light) не найден на дочерних объектах!");
        }
    }
}
