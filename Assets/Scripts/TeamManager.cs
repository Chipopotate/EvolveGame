using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class TeamManager : BaseManager
{
    [Header("Debug")]
    [SerializeField] private bool debug = false;

    private EffectBarManager _effectManager;
    private LevelUpMenu _levelUpMenu;

    [Header("Teams")] // Списки unit-ов для команд
    public List<Unit> allUnits;
    public List<Unit> monsterTeam;
    public List<Unit> hunterTeam;
    public List<Unit> neutralTeam;

    public List<GameObject> corpse;

    public enum TeamType
    {
        Monsters,
        Hunters,
        Neutrals
    }

    public TeamType СurrentTeam { get; private set; }

    public void Boot(EffectBarManager effectManager, LevelUpMenu levelMenu)
    {
        _effectManager = effectManager;
        _levelUpMenu = levelMenu;

        // Заменяем префабы
        {
            ReplacePrefabsWithInstances(ref monsterTeam);
            foreach (Monster monster in monsterTeam)
            {
                monster.GetLevelUpMenu(_levelUpMenu);
            }
            ReplacePrefabsWithInstances(ref hunterTeam);
            ReplacePrefabsWithInstances(ref neutralTeam);
        }
        // Добавляем в allUnits всех unit-ов
        {
            ListAddRange(allUnits, monsterTeam);
            ListAddRange(allUnits, hunterTeam);
            ListAddRange(allUnits, neutralTeam);
        }


        if (debug) Debug.Log(this.GetType().Name + " инициализирован!");
    }

    // Метод расширения списка
    private void ListAddRange(List<Unit> toList, List<Unit> fromList)
    {
        toList.AddRange(fromList);

        if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");
    }

    // Метод переключения текущей команды
    public TeamType GetNextTeam(TeamType current)
    {
        // Порядок: Players → Hunters → Neutrals → Players...
        switch (current)
        {
            case TeamType.Monsters:
                return TeamType.Hunters;
            case TeamType.Hunters:
                return TeamType.Neutrals;
            case TeamType.Neutrals:
                return TeamType.Monsters;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // Метод получения текущей команды
    public List<Unit> GetTeamByIndex(TeamType team)
    {
        switch (team)
        {
            case TeamType.Monsters:
                return monsterTeam;
            case TeamType.Hunters:
                return hunterTeam;
            case TeamType.Neutrals:
                return neutralTeam;
            default: 
                throw new ArgumentOutOfRangeException();
        }
    }

    // Метод для преобразования индекса в TeamType
    public TeamType GetTeamTypeByIndex(int index)
    {
        if (Enum.IsDefined(typeof(TeamType), index))
        {
            if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");
            return (TeamType)index;
        }
        else
        {
            Debug.LogError($"Недопустимый индекс команды: {index}");
            return TeamType.Monsters; // Возвращаем значение по умолчанию
        }
    }

    /// <summary>
    /// Общий метод для замены списка префабов на экземпляры объектов в сцене.
    /// </summary>
    /// <param name="prefabs">Список префабов для замены.</param>
    /// <param name="parent">Родительский объект, к которому будут прикреплены новые объекты (опционально).</param>
    /// <param name="spawnPositions">Список позиций, где должны быть созданы экземпляры (опционально).</param>
    /// <returns>Список созданных объектов в сцене.</returns>
    public void ReplacePrefabsWithInstances(ref List<Unit> prefabs)
    {
        // Создаем временный список для хранения инстансов
        List<Unit> instantiatedObjects = new List<Unit>();

        foreach (var prefab in prefabs)
        {
            if (prefab == null) continue;

            // Создаем экземпляр объекта
            GameObject instantiatedObject = UnityEngine.Object.Instantiate(prefab.gameObject);
            Unit unit = instantiatedObject.GetComponent<Unit>();

            if (unit != null)
            {
                unit.Boot();
                unit.OnDeath += (entity) => HandleUnitDeath(entity as Unit);
                _effectManager.RegisterEntity(unit);
                instantiatedObjects.Add(unit);
            }
        }

        // Очищаем оригинальный список и заменяем его содержимое
        prefabs.Clear();
        prefabs.AddRange(instantiatedObjects);
    }

    private void HandleUnitDeath(Unit deadUnit)
    {
        int reward = deadUnit.xpReward;

        // Удаляем из всех списков
        allUnits.Remove(deadUnit);
        monsterTeam.Remove(deadUnit);
        hunterTeam.Remove(deadUnit);
        neutralTeam.Remove(deadUnit);

        GameObject temp =  Instantiate(corpse[UnityEngine.Random.Range(0, corpse.Count)], deadUnit.transform.position, Quaternion.identity);
        Corpse corpseComponent = temp.GetComponent<Corpse>();
        corpseComponent.SetCorpseReward(reward);

        // Дополнительная логика при необходимости
        Debug.Log($"{deadUnit.name} удален из всех списков команд");
    }

    private void CleanNullReferences(List<Unit> list)
    {
        list.RemoveAll(unit => unit == null);
    }
}
