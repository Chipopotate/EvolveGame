using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NoiseManager : BaseManager
{
    [Header("Debug")]
    [SerializeField] private bool debug = false;

    [Header("PerlinSettings")]
    [SerializeField] private float noiseScale = 0.1f;
    [SerializeField] private Vector2 offset;

    [Header("Levels")]
    [SerializeField] private float waterLevel = 0.3f;
    [SerializeField] private float sandLevel = 0.35f;
    [SerializeField] private float rockLevel = 0.9f;

    [Header("Dependencies")]
    [SerializeField] private TileBase _ground;
    [SerializeField] private TileBase _dirty;
    [SerializeField] private TileBase _rock;
    [SerializeField] private TileBase _water;
    [SerializeField] private TileBase _sand;

    public void Boot(TileBase ground, TileBase dirty, TileBase rock, TileBase water, TileBase sand)
    {
        _ground = ground;
        _dirty = dirty;
        _rock = rock;
        _water = water;
        _sand = sand;
    }

    public TileBase GetTileForNoise(float noise)
    {
        if (noise < waterLevel) 
            return _water;
        if (noise < sandLevel)
            return _sand;
        if (noise > rockLevel)
            return _rock;
        else
            return _ground;
    }

    public float GetNoiseValue(int x, int y)
    {
        float noiseValue = Mathf.PerlinNoise((x + offset.x) * noiseScale, (y + offset.y) * noiseScale);
        return noiseValue;
    }
}
