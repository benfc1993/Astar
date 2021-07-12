using System.Collections.Generic;
using Grids;
using UnityEngine;
using World;

namespace Astar.Grid
{
    public class AStarGrid : MonoBehaviour
    {
        public GridDataSO gridDataSO;
        public TerrainTypeSO terrainType;
        WorldGrid _worldGrid;

        public float carveRadius;
        public LayerMask unWalkableLayerMask;
        Node[,] _grid;
        Vector3 _worldBottomLeft;
        
        float _nodeDiameter;
        int _nodeCountX, _nodeCountY;
        public bool displayGridGizmos;
        public int MaxSize => _nodeCountX * _nodeCountY;
        static AStarGrid _instance;

        void Awake()
        {
            _instance = this;
            _worldGrid = GetComponentInParent<GridManager>().worldGrid;

            _nodeDiameter = gridDataSO.nodeRadius * 2;
            _nodeCountX = Mathf.RoundToInt((gridDataSO.gridWorldSize.x / _nodeDiameter));
            _nodeCountY = Mathf.RoundToInt((gridDataSO.gridWorldSize.y / _nodeDiameter));
        }

        public void Init()
        {
            CreateGrid();
        }

        void CreateGrid()
        {
            _grid = new Node[_nodeCountX, _nodeCountY];
            _worldBottomLeft = transform.position - Vector3.right * gridDataSO.gridWorldSize.x / 2 - Vector3.forward * gridDataSO.gridWorldSize.y / 2;

            for (int x = 0; x < _nodeCountX; x++)
            {
                for (int y = 0; y < _nodeCountY; y++)
                {
                    Vector3 worldPoint = _worldBottomLeft + Vector3.right * (x * _nodeDiameter + gridDataSO.nodeRadius) + Vector3.forward * (y * _nodeDiameter + gridDataSO.nodeRadius);
                    bool walkable = !(Physics.CheckSphere(worldPoint, 2 * gridDataSO.nodeRadius + carveRadius, unWalkableLayerMask));

                    terrainType.terrainTypesDictionary.TryGetValue(_worldGrid.Grid[x, y].terrainType,
                        out int movementPenalty);
                    _grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
                }
            }
        }

        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if(x == 0 && y == 0) continue;

                    int checkX = node.X + x;
                    int checkY = node.Y + y;

                    if(checkX >= 0 && checkX < _nodeCountX && checkY >= 0 && checkY < _nodeCountY) neighbours.Add(_grid[checkX, checkY]);
                }
            }

            return neighbours;
        }

        public Node NodeFromWorldPoint(Vector3 worldPosition)
        {
            var percentX = (worldPosition.x + gridDataSO.gridWorldSize.x / 2) / gridDataSO.gridWorldSize.x;
            var percentY = (worldPosition.z + gridDataSO.gridWorldSize.y / 2) / gridDataSO.gridWorldSize.y;
            percentX = Mathf.Clamp01((percentX));
            percentY = Mathf.Clamp01((percentY));

            int x = Mathf.RoundToInt((_nodeCountX - 1) * percentX);
            int y = Mathf.RoundToInt((_nodeCountY - 1) * percentY);
            return _grid[x, y];
        }

        public void CalculateMovementCost(int x, int y)
        {
            terrainType.terrainTypesDictionary.TryGetValue(_worldGrid.Grid[x, y].terrainType,
                out int movementPenalty);
            _grid[x,y].SetMovementPenalty(movementPenalty);
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridDataSO.gridWorldSize.x,1, gridDataSO.gridWorldSize.y));

            if (_grid == null || !displayGridGizmos) return;
            foreach (Node node in _grid)
            {
                Gizmos.color = node.walkable ? Color.white : Color.red;
                Gizmos.DrawCube(node.WorldPosition, Vector3.one * (_nodeDiameter - .1f));
            }
        }

        public static void SamplePosition(Vector3 origin, float radius, out Node point)
        {
            while (true)
            {
                var angle = Random.Range(0, 360);
                var radiusSample = Random.Range(radius / 2, radius);
                Vector3 samplePoint = Quaternion.AngleAxis(angle, Vector3.up) * new Vector3(1,0,1) * radiusSample + origin;
                print(origin);
                print(angle);
                print(samplePoint);
                if (_instance._worldBottomLeft.x + samplePoint.x < _instance._worldBottomLeft.y + _instance.gridDataSO.gridWorldSize.x && _instance._worldBottomLeft.y + samplePoint.y < _instance._worldBottomLeft.y + _instance.gridDataSO.gridWorldSize.y )
                {
                    point = _instance.NodeFromWorldPoint(samplePoint);
                    return;
                }
            }
            
        }
    }
}
