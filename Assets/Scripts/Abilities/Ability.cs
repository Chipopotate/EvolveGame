using UnityEngine;

// Перечисление, которое определяет тип цели способности: юнит или плитка.
public enum AbilityTargetMode { Self, Target, Tile, Trigger }

// Абстрактный класс для создания способностей. Используется как шаблон.
public abstract class Ability : ScriptableObject
{
    [Header("Base Settings")]
    public string abilityName; // Название способности.
    public int requiredLevel = 1; // Уровень, необходимый для использования способности.
    public int range; // Дальность действия.
    public int scope; // Область действия.
    public int staminaCost; // Стоимость в очках действий.
    public Sprite icon; // Иконка для отображения в интерфейсе.
    public AbilityTargetMode targetMode = AbilityTargetMode.Target; // Тип цели способности.

    // Проверка возможности применения способности (по целям типа юнит). 
    public virtual bool CanExecute(Unit caster, Entity target, ReachableTilesController reachableTiles, HighlightAreaController highlightArea)
    {
        if (target == null) return false;
        return targetMode == AbilityTargetMode.Target &&
               caster.currentActionPoint >= staminaCost &&
               reachableTiles.IsTargetInRange(caster.transform.position, target.transform.position, range);
    }

    // Проверка возможности применения способности (по целям типа плитка).
    public virtual bool CanExecute(Unit caster, Vector3Int targetPosition, ReachableTilesController reachableTiles, HighlightAreaController highlightArea)
    {
        return targetMode == AbilityTargetMode.Tile &&
               caster.currentActionPoint >= staminaCost &&
               reachableTiles.IsTargetInRange(caster.transform.position, targetPosition, range);
    }

    // Абстрактный метод для выполнения способности (цель - юнит).
    public virtual void Execute(Unit caster, Entity target, HighlightAreaController highlightArea) { }

    // Абстрактный метод для выполнения способности (цель - плитка).
    public virtual void Execute(Unit caster, Vector3 targetPosition, HighlightAreaController highlightArea) { }

    public virtual void PreviewArea(Unit caster, HighlightAreaController highlightArea, ReachableTilesController reachableTiles)
    {
        // Если действия недостаточно — не подсвечиваем
        if (caster.currentActionPoint < staminaCost)
            return;

        // Подсветка зависит от типа цели
        if (targetMode == AbilityTargetMode.Tile)
        {
            // Просто подсветим радиус вокруг юнита
            highlightArea.HighlightArea(caster.transform.position, range, Color.yellow); // Или любой другой цвет
        }
        else if (targetMode == AbilityTargetMode.Target)
        {
            // Тут можно было бы подсветить клетки, на которых стоят враги в радиусе действия
            // или просто подсветить область вокруг, как для тайлов

            highlightArea.HighlightArea(caster.transform.position, range, Color.red); // Подсветка "юнитных" целей
        }
    }
}
