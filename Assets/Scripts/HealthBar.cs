using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool debug = false;

    [Header("HealthBar settings")]
    [SerializeField] private Image foregroundImage; // Image дл€ заливки (зелена€)

    // ћетод дл€ обновлени€ полоски здоровь€
    public void SetHealth(float currentHealth, float maxHealth) // currentHealth - текущее здоровье // maxHealth - максимальное здоровье
    {
        if (maxHealth <= 0f)
        {
            Debug.LogWarning("ћаксимальное количество здоровь€ должно быть больше нул€!");
            return;
        }

        // ¬ычисл€ем долю здоровь€ и обновл€ем fillAmount (от 0 до 1)
        float fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        
        // ќбновл€ем полоску здоровь€
        if (foregroundImage == null)
        {
            Debug.LogWarning("ќтсутсвует ссылка на компонент!");
            return;
        }
        else foregroundImage.fillAmount = fillAmount;
    }
}
