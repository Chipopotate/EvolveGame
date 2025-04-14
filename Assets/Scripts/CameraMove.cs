using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [Header("Настройки перемещения")]
    [Tooltip("Скорость перемещения камеры при нажатии клавиш WASD")]
    public float keyboardSpeed = 10f;
    [Tooltip("Скорость перетаскивания камеры мышью")]
    public float mouseDragSpeed = 10f;
    [Header("Настройки зума")]
    [Tooltip("Скорость приближения/отдаления камеры при использовании колесика мыши")]
    public float zoomSpeed = 5f;
    [Tooltip("Минимальное и максимальное значение зума")]
    public float minZoom = 3f, maxZoom = 15f;

    private Vector3 dragOrigin;
    private bool isDragging = false;

    public void Boot(TurnManager turnManager)
    {
        turnManager.Boot(this);
    }

    void Update()
    {
        HandleKeyboardMovement();
        HandleMouseDrag();
        HandleZoom();
    }

    private void HandleKeyboardMovement()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            move.y += 1;
        if (Input.GetKey(KeyCode.S))
            move.y -= 1;
        if (Input.GetKey(KeyCode.A))
            move.x -= 1;
        if (Input.GetKey(KeyCode.D))
            move.x += 1;

        transform.Translate(move * keyboardSpeed * Time.deltaTime, Space.World);
    }

    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 difference = Camera.main.ScreenToWorldPoint(dragOrigin) - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position += difference * mouseDragSpeed;
            dragOrigin = Input.mousePosition;
        }
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Camera.main.orthographicSize -= scroll * zoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);
        }
    }

    public void CenterCameraOnObject(GameObject target)
    {
        if (target != null)
        {
            Vector3 targetPos = target.transform.position;
            targetPos.z = transform.position.z;
            transform.position = targetPos;
        }
        else
        {
            Debug.LogWarning("Переданный объект для центрирования камеры равен null!");
        }
    }
}
