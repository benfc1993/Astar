using System.Collections.Generic;
using Astar.Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldGrid : MonoBehaviour
{
    public GridDataSO gridDataSO;
    GridManager _gridManager;

    int _gridSizeX;
    int _gridSizeY;
    float _nodeDiameter;
    public WorldNode[,] Grid { get; private set; }
    public bool drawGizmos;
    public int roadWidth;
    [SerializeField] Vector2 forestSize;
    [SerializeField] int forestCount;

    Vector3 _worldBottomLeft;
    [SerializeField] List<Vector3> exits = new List<Vector3>();

    public void Init()
    {
        _gridManager = GetComponentInParent<GridManager>();
        CreateGrid();
    }


    public void Generate()
    {
        print("generation");
    }


    private void CreateGrid()
    {

        _nodeDiameter = gridDataSO.nodeRadius * 2;
        _gridSizeX = Mathf.RoundToInt((gridDataSO.gridWorldSize.x / _nodeDiameter));
        _gridSizeY = Mathf.RoundToInt((gridDataSO.gridWorldSize.y / _nodeDiameter));

        if(gridDataSO.gridWorldSize.x <= 0 || gridDataSO.gridWorldSize.y <= 0 || gridDataSO.nodeRadius <= 0) return;
        Grid = new WorldNode[_gridSizeX, _gridSizeY];
        _worldBottomLeft = transform.position - Vector3.right * gridDataSO.gridWorldSize.x / 2 - Vector3.forward * gridDataSO.gridWorldSize.y / 2;

        for (int x = 0; x < _gridSizeX; x++)
        {
            for (int y = 0; y < _gridSizeY; y++)
            {
                Vector3 worldPoint = _worldBottomLeft + Vector3.right * (x * _nodeDiameter + gridDataSO.nodeRadius) + Vector3.forward * (y * _nodeDiameter + gridDataSO.nodeRadius);
                Grid[x, y] = new WorldNode(x, y, worldPoint);
            }
        }

        CreateForest();
        CreateMainRoads();
    }

    void CreateForest()
    {
        for (int i = 0; i < forestCount; i++)
        {
            float bottomX = Random.Range(0, gridDataSO.gridWorldSize.x - forestSize.x);
            float bottomY = Random.Range(0, gridDataSO.gridWorldSize.y - forestSize.y);

            for (int x = 0; x < forestSize.x; x++)
            {
                for (int y = 0; y < forestSize.y; y++)
                {
                    Vector3 worldPoint = _worldBottomLeft + Vector3.right * ((bottomX + x) * _nodeDiameter + gridDataSO.nodeRadius) + Vector3.forward * (
                        (bottomY + y) * _nodeDiameter + gridDataSO.nodeRadius);
                    NodeFromWorldPoint(new Vector3(worldPoint.x, 0, worldPoint.z)).SetTerrainType(TerrainType.Forest);
                }
            }

        }

    }

    void CreateMainRoads()
    {
        Vector2Int[] roadStarts = {new Vector2Int(_gridSizeX / 4, 0), new Vector2Int(0, _gridSizeY / 3)};

        for (int y = 0; y < _gridSizeY; y++)
        {
            int posX = _gridSizeX / 4;

            for (int i = 0; i < roadWidth; i++)
            {
                if(posX + i >= _gridSizeX) continue;
                Grid[ posX + i, y].SetTerrainType(TerrainType.Road);
                if ((y == 0 || y == _gridSizeY - 1) && (i == Mathf.RoundToInt(roadWidth / 2)))
                {
                    var node = Grid[posX, y];
                    node.isExit = true;
                    exits.Add(node.WorldPosition);
                }
            }
        }

        for (int x = 0; x < _gridSizeX; x++)
        {
            int posY = _gridSizeY / 4;

            for (int i = 0; i < roadWidth; i++)
            {
                if(posY + i >= _gridSizeY) continue;
                Grid[x, posY + i].SetTerrainType(TerrainType.Road);
                if ((x == 0 || x == _gridSizeX - 1) && (i == Mathf.RoundToInt(roadWidth / 2)))
                {
                    var node = Grid[x, posY];
                    node.isExit = true;
                    exits.Add(node.WorldPosition);
                }
            }
        }
    }


    public void CreateRoad(Vector3 worldNode)
    {
        var exit = exits[0];
        print($"create road to {exit}");
        PathRequestManager.RequestPath(worldNode, exit, TraceRoad, false);
    }

    void TraceRoad(Vector3[] newPath, bool pathSuccessful)
    {
        print("here: " + pathSuccessful);

        if (!pathSuccessful) return;
        print("creating road " + newPath.Length);
        foreach (Vector3 point in newPath)
        {
            var node = NodeFromWorldPoint(point);
            node.SetTerrainType(TerrainType.Road);
            _gridManager.aStarGrid.CalculateMovementCost(node.X, node.Y);
        }
    }

    //TODO: remove duplication of method
    public WorldNode NodeFromWorldPoint(Vector3 worldPosition)
    {
        var percentX = (worldPosition.x + gridDataSO.gridWorldSize.x / 2) / gridDataSO.gridWorldSize.x;
        var percentY = (worldPosition.z + gridDataSO.gridWorldSize.y / 2) / gridDataSO.gridWorldSize.y;
        percentX = Mathf.Clamp01((percentX));
        percentY = Mathf.Clamp01((percentY));

        int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);
        return Grid[x, y];
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridDataSO.gridWorldSize.x,1, gridDataSO.gridWorldSize.y));

        if (!drawGizmos) return;
        for (int x = 0; x < _gridSizeX; x += 1)
        {
            for (int y = 0; y < _gridSizeY; y += 1)
            {
                if(Grid[x,y].terrainType != TerrainType.Road && Grid[x,y].terrainType != TerrainType.Forest) continue;
                Gizmos.color = Grid[x, y].terrainType != TerrainType.Road ? Color.yellow : Color.green;
                Gizmos.DrawCube(new Vector3(Grid[x, y].WorldPosition.x, 0, Grid[x, y].WorldPosition.z), Vector3.one * (_nodeDiameter - 0.1f));
            }
        }
    }
}
