using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
using static UnityEngine.EventSystems.EventTrigger;

public class MapGenerationController : BaseManager
{
    [Header("Debug")]
    [SerializeField] private bool debug = false;

    [Header("Tiles")]
    [SerializeField] private TileBase grass;
    [SerializeField] private TileBase dirty;
    [SerializeField] private TileBase road;
    [SerializeField] private TileBase sand;
    [SerializeField] private TileBase water;
    [SerializeField] private TileBase rockEdge;
    [SerializeField] private TileBase rockPlate;

    [Header("MapSettings")]
    [SerializeField] private int width = 100;
    [SerializeField] private int height = 100;
    int centerX;
    int centerY;
    float radius;
    float radiusSq;

    [Header("Dependencies")]
    [SerializeField] private TeamManager _team;
    [SerializeField] private MapManager _map;
    [SerializeField] private TileManager _tileManager;
    [SerializeField] private ObjectSpawnController objectSpawner;
    [SerializeField] private EntitySpawnController entitySpawner;
    [SerializeField] private NoiseManager noise;

    [Header("Components")]
    [SerializeField] private Tilemap tileMap;
    public List<Vector3Int> passableTiles {get; private set; }
    public List<Vector3Int> grassTiles {get; private set; }
    public List<Vector3Int> sandTiles {get; private set; }
    public List<Vector3Int> dirtTiles {get; private set; }
    public List<Vector3Int> impassableTiles { get; private set; }

    public void Boot(MapManager mapManager, TeamManager teamManager, TileManager tileManager)
    {
        passableTiles = new List<Vector3Int>();
        impassableTiles = new List<Vector3Int>();

        centerX = width / 2;          // Центр по X
        centerY = height / 2;         // Центр по Y
        radius = Mathf.Min(width, height) / 2f; // Радиус круга
        radiusSq = radius * radius; // Квадрат радиуса для оптимизации

        _team = teamManager;
        _map = mapManager;
        _tileManager = tileManager;

        tileMap = gameObject.GetComponent<Tilemap>();
        objectSpawner = GetComponent<ObjectSpawnController>();
        entitySpawner = GetComponent<EntitySpawnController>();
        noise = GetComponent<NoiseManager>();

        // Проверяем зависимостей
        {
            ValidateComponent(tileMap);
            ValidateComponent(objectSpawner);
            ValidateComponent(entitySpawner);
            ValidateComponent(noise);

            ValidateAsset(dirty);
            ValidateAsset(grass);
            ValidateAsset(water); 
            ValidateAsset(sand);
        }

        noise.Boot(grass, dirty, rockPlate, water, sand);
        objectSpawner.Boot(_map, _tileManager);
        entitySpawner.Boot(_team, _map, this, width, height);

        BaseMapGenerate();

        if (debug) Debug.Log(this.GetType().Name + " инициализирован");
    }

    public void BaseMapGenerate()
    {
        _map.mainTilemap.ClearAllTiles();
        passableTiles.Clear(); // Очищаем список перед генерацией
        objectSpawner.Boot(_map, _tileManager); // Очищаем предыдущие объекты

        TileBase tile;
        Vector3Int tilePosition = new Vector3Int(0, 0, 0);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Проверяем, находится ли точка внутри круга
                float dx = x - centerX;
                float dy = y - centerY;
                float distanceSq = dx * dx + dy * dy;

                if (distanceSq > radiusSq)
                {
                    //tile = water; // Устанавливаем воду вне круга
                    continue;
                }
                else
                {
                    float noiseValue = noise.GetNoiseValue(x, y); // Получение значения шума
                    tile = noise.GetTileForNoise(noiseValue); // Получаем тип тайла на основе шума
                    
                }

                tilePosition = new Vector3Int(x, y, 0); // Позиция тайла
                tileMap.SetTile(tilePosition, tile); // Заполняем карту тайлом

                if (tile != water && tile != rockPlate)
                {
                    passableTiles.Add(tilePosition); // Сохраняем тайлы земли
                }
                else if (tile == water)
                    impassableTiles.Add(tilePosition); // Сохраняем тайлы воды
            }
        }

        if (debug) Debug.Log($"Метод {MethodBase.GetCurrentMethod().Name} выполнен!");

        objectSpawner.PlaceObjects(passableTiles);
        entitySpawner.PlaceEntities();
    }
}
