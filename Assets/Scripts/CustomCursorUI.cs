using UnityEngine;
using UnityEngine.UI;

public class CustomCursorUI : MonoBehaviour
{
    [Header("UI  урсор")]
    [SerializeField] private Image cursorImage; // UI Image с изображением курсора
    [SerializeField] private Canvas canvas;       // Canvas, прив€занный к камере

    void Start()
    {
        // —крываем стандартный курсор
        Cursor.visible = false;
    }

    void Update()
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
        {
            // ѕреобразуем координаты мыши в локальные координаты Canvas
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out Vector2 localPoint
            );
            // ќбновл€ем позицию курсора
            cursorImage.rectTransform.localPosition = localPoint;
        }
        else if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            cursorImage.transform.position = Input.mousePosition;
        }
    }
}
