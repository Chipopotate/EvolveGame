using UnityEngine;

public abstract class BaseManager : MonoBehaviour
{
    // Виртуальный метод для переопределения в дочерних классах
    public virtual void Boot()
    {

    }

    // Универсальная проверка на зависимости компонентов
    protected void ValidateComponent<T>(T component) where T : Component
    {
        if (component == null)
        {
            string dependencyType = typeof(T).Name;
            Debug.LogError($"Ошибка в {GetType().Name}: Отсутствует компонент типа {dependencyType} на объекте {gameObject.name}!", gameObject);

            HandleCriticalError();
        }
    }

    // Для проверки любых Unity-объектов (не только компонентов)
    protected void ValidateAsset<T>(T asset) where T : Object
    {
        if (asset == null)
        {
            string assetType = typeof(T).Name;
            Debug.LogError($"Ошибка в {GetType().Name}: Отсутствует {assetType}!", gameObject);

            HandleCriticalError();
        }
    }

    // Статический метод для валидации зависимостей в любом классе
    public static bool ValidateDependencies(MonoBehaviour context, params Component[] dependencies)
    {
        bool isValid = true;
        foreach (var dependency in dependencies)
        {
            if (dependency == null)
            {
                Debug.LogError($"Отсутствует зависимость в {context.GetType().Name}!", context.gameObject);
                isValid = false;
            }
        }
        return isValid;
    }

    private void HandleCriticalError()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit(0);
        #endif
    }
}