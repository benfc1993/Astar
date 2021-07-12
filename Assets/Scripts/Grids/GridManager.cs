using Astar.Grid;
using Astar.Pathfinding;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public AStarGrid aStarGrid;
    public PathRequestManager pathRequestManager;
    public WorldGrid worldGrid;

    public Transform[] buildingEntrances;
    void Awake()
    {
        worldGrid.Init();
        aStarGrid.Init();

    }

    void Start()
    {
        if (buildingEntrances.Length > 0) AssignEntrancesToGrid();
        worldGrid.Generate();
        pathRequestManager.Init();
    }

    void AssignEntrancesToGrid()
    {
        foreach (Transform entrance in buildingEntrances)
        {
            var position = entrance.position;

            var aStarNode = aStarGrid.NodeFromWorldPoint(position);
            aStarNode.walkable = true;

            var worldNode = worldGrid.NodeFromWorldPoint(position);
            worldNode.isBuildingEntrance = true;
            worldGrid.CreateRoad(worldNode.WorldPosition);
        }
    }
}
