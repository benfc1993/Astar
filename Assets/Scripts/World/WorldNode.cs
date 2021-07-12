using System;
using Astar;
using Astar.Grid;
using UnityEngine;

[Serializable]
public class WorldNode : IGridNode
{

    public WorldNode(int x, int y, Vector3 worldPosition)
    {
        X = x;
        Y = y;
        WorldPosition = worldPosition;
    }

    public int X { get; }
    public int Y { get; }
    public Vector3 WorldPosition { get; }

    public TerrainType terrainType = TerrainType.Grass;

    public void SetTerrainType(TerrainType value)
    {
        terrainType = value;
    }
}

public enum TerrainType
{
    Grass,
    Road,
    Forest,
    Alley,
    Water,
}
