using System;
using Astar.Grid;
using Astar.Pathfinding;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public AStarGrid aStarGrid;
    public PathRequestManager pathRequestManager;
    public WorldGrid worldGrid;

    void Awake()
    {
        worldGrid.Init();
        aStarGrid.Init();
        worldGrid.Generate();
        pathRequestManager.Init();
    }
}
