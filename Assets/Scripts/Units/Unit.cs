using System.Collections.Generic;
using UnityEngine;
using static TeamManager;

public class Unit : Entity
{
    [Header("Settings")]
    public int baseActionPoint = 2;
    public int currentActionPoint;
    public int baseSpeed = 5;
    public int currentSpeed;
    public int sightRange = 3;
    public int attackBoost = 0;
    public int xpReward = 0;

    [Header("Resistance")]
    //

    public TeamManager.TeamType _teamType; // Индекс команды
    // Свойство для совместимости
    public int TeamIndex => (int)_teamType;

    [Header("Abilities")]
    public Ability[] abilities; // Ссылки на способности из ScriptableObject

    [Header("Health & Effects")]
    [SerializeField] private HealthBar healthBar;

    [Header("Dependencies")]
    private HealthManager healthManager;

    public override void Boot()
    {
        base.Boot(); // Вызов базовой логики (Entity)

        // Инициализация здоровья
        healthManager = new HealthManager(maxHealth, healthBar);

        if (healthBar == null)
        {
            Transform healthBarTransform = transform.Find("HealthBar");
            if (healthBarTransform != null)
            {
                healthBar = healthBarTransform.GetComponent<HealthBar>();
            }
        }

        currentHealth = maxHealth;
        currentSpeed = baseSpeed;
    }

    public void ResetActionPoint()
    {
        currentActionPoint = baseActionPoint;
    }

    public void TakeDamage(int damage)
    {
        healthManager.TakeDamage(damage);
        if (healthManager.IsDead())
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        healthManager.Heal(amount);
    }
}
