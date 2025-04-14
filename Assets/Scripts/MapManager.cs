using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class MapManager : BaseManager
{
    [Header("Debug")]
    [SerializeField]
    private bool debug = false;

    public Grid grid { get; private set; }
    public Tilemap mainTilemap { get; set; }
    public Tilemap fogTilemap { get; set; }

    [SerializeField] private TilePropertiesCollection tileProperties; // Коллекция свойств тайлов
    public TilePropertiesCollection tilePropertiesCollection { get; private set; }

    public Dictionary<Vector3Int, bool> occupationMap = new Dictionary<Vector3Int, bool>();

    Transform childTransform;

    public override void Boot()
    {
        grid = GetComponent<Grid>();

        childTransform = transform.Find("MainTilemap");
        ValidateComponent(childTransform);
        mainTilemap = childTransform.gameObject.GetComponent<Tilemap>();

        childTransform = transform.Find("FogTilemap");
        ValidateComponent(childTransform);
        fogTilemap = childTransform.gameObject.GetComponent<Tilemap>();

        tilePropertiesCollection = tileProperties;

        // Проверяем зависимостей
        {
            ValidateComponent(grid);
            ValidateComponent(mainTilemap);
            ValidateComponent(fogTilemap);
            ValidateAsset(tilePropertiesCollection);
        }
    }

    /// <summary>
    /// Преобразует мировые координаты в координаты клетки на сетке.
    /// </summary>
    /// <param name="worldPosition">Мировые координаты (Vector3).</param>
    /// <returns>Координаты клетки на сетке (Vector3Int).</returns>
    public Vector3Int GetTilePositionFromWorldPosition(Vector3 worldPosition)
    {
        // Используем метод WorldToCell, чтобы преобразовать координаты
        Vector3Int cellPosition = grid.WorldToCell(worldPosition);

        return cellPosition; // Возвращаем координаты клетки
    }

    /// <summary>
    /// Возвращает центр указанной клетки в мировых координатах.
    /// </summary>
    /// <param name="tilePosition">Координаты клетки (Vector3Int).</param>
    /// <returns>Мировые координаты центра клетки (Vector3).</returns>
    public Vector3 GetTileCenterWorld(Vector3Int tilePosition)
    {
        Vector3 centerPosition = grid.GetCellCenterWorld(tilePosition); // Получаем центр клетки
        centerPosition.z = 0; // Устанавливаем Z-координату на 0 для работы в 2D
        return centerPosition;
    }
}