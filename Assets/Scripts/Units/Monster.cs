using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class Monster : Unit
{
    [Header("Characteristic")]
    public int baseArmor;
    public int currentArmor;
    public int currentXP = 0;
    public HealthBar shieldBar;
    public int LVL = 1;
    public int maxLVL = 10;
    public int baseXP = 50; // Базовый опыт для перехода с 1 на 2 уровень
    [SerializeField] private int requiredXP;

    [Header("Level Up")]
    [SerializeField] private LevelUpMenu levelUpMenu;
    [SerializeField] private List<Ability> availableAbilities;

    [Header("Level Scaling")]
    [SerializeField] private float healthGrowthPercent = 0.1f;  // +10% здоровья
    [SerializeField] private float armorGrowthPercent = 0.1f;   // +10% брони
    [SerializeField] private float speedGrowthPercent = 0.05f;  // +5% скорости
    [SerializeField] private float actionPointGrowthChance = 0.3f; // 30% шанс дать +1 очко действия

    public override void Boot()
    {
        base.Boot(); // Вызов логики Unit

        // Инициализация уникальных параметров Monster
        requiredXP = Mathf.RoundToInt(baseXP * Mathf.Pow(1.5f, LVL - 1));

        if (shieldBar == null)
        {
            Transform healthBarTransform = transform.Find("ShieldBar");
            if (healthBarTransform == null)
            {
                Debug.LogError("Дочерний объект с именем 'ShieldBar' не найден у " + gameObject.name);
            }
            else
            {
                shieldBar = healthBarTransform.GetComponent<HealthBar>();
            }
        }

        currentArmor = baseArmor;
    }

    public void TakeXP(int xp)
    {
        currentXP += xp;

        if (debug) Debug.Log(gameObject.name + " получил: " + xp + " опыта. Текущий опыт: " + currentXP);

        // Повышаем уровень, пока хватает опыта
        while (currentXP >= requiredXP && LVL < maxLVL)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        LVL++;
        currentXP -= requiredXP;
        requiredXP = Mathf.RoundToInt(requiredXP * 1.5f);

        if (debug) Debug.Log("Уровень повышен до " + LVL + ". Остаток опыта: " + currentXP + ", требуется для следующего: " + requiredXP);

        // Усиление характеристик по проценту (но не лечим!)
        maxHealth = Mathf.RoundToInt(maxHealth * (1 + healthGrowthPercent));
        baseArmor = Mathf.RoundToInt(baseArmor * (1 + armorGrowthPercent));
        baseSpeed = Mathf.RoundToInt(baseSpeed * (1 + speedGrowthPercent));

        // Обновляем текущие значения, кроме здоровья и брони
        currentSpeed = baseSpeed;
        currentArmor = Mathf.Min(currentArmor, baseArmor); // Не увеличиваем текущую броню, если она уже меньше

        // Случайный шанс дать +1 Action Point
        if (Random.value < actionPointGrowthChance)
        {
            baseActionPoint += 1;
            if (debug) Debug.Log("Монстр получил дополнительное очко действия!");
        }

        if (debug)
        {
            Debug.Log($"Обновлённые характеристики: MaxHP={maxHealth}, BaseArmor={baseArmor}, Speed={baseSpeed}, ActionPoints={baseActionPoint}");
        }

        // Показать окно выбора способностей
        levelUpMenu.Show(this, GetAvailableAbilitiesForLevel());
    }


    private List<Ability> GetAvailableAbilitiesForLevel()
    {
        // Выбираем способности, которые требуют ТОЛЬКО текущий уровень
        return availableAbilities.FindAll(a => a.requiredLevel == LVL);
    }

    public void AddAbility(Ability newAbility)
    {
        var abilitiesList = new List<Ability>(abilities);
        abilitiesList.Add(newAbility);
        abilities = abilitiesList.ToArray();
    }

    public void GetLevelUpMenu(LevelUpMenu levelMenu)
    {
        levelUpMenu = levelMenu;
    }

}
