using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TilePropertiesCollection", menuName = "Tiles/TileProperties")]
public class TileProperties : ScriptableObject
{
    public TileBase tile;  // Какой тайл описываем
    public int movementCost; // Стоимость движения по этому тайлу
    public bool isWalkable; // Можно ли по нему ходить
    // public bool isOccupied = false; // Занят ли тайл
}