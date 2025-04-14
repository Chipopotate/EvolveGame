using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TilePropertiesCollection", menuName = "Tiles/TilePropertiesCollection")]
public class TilePropertiesCollection : ScriptableObject
{
    public List<TileProperties> tiles = new List<TileProperties>();

    public TileProperties GetTileProperties(TileBase tile)
    {
        return tiles.Find(tp => tp.tile == tile);
    }

    // Метод проверки проходимости tile-а
    public bool IsTileWalkable(TileBase tile)
    {
        foreach (TileProperties tp in tiles)
        {
            if (tp.tile == tile) return tp.isWalkable;
        }
        return false; // Если тайл не найден, считаем его непроходимым
    }

    // Метод получения стоимости перемещения по tile-у
    public int GetTileCost(TileBase tile)
    {
        foreach (TileProperties tp in tiles)
        {
            if (tp.tile == tile) return tp.movementCost;
        }
        return 0; // Если тайл не найден, назначаем минимальную стоимость
    }
}
