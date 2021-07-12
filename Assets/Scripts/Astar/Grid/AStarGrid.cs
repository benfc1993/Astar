using System.Collections.Generic;
using UnityEngine;

namespace Astar.Grid
{
    public class AStarGrid : MonoBehaviour
    {
        public GridDataSO gridDataSO;
        public TerrainTypeSO terrainType;
        WorldGrid _worldGrid;

        public float carveRadius;
        public LayerMask unwalkableLayerMask;
        private Node[,] _grid;

        private float _nodeDiameter;
        private int _gridSizeX, _gridSizeY;
        public bool displayGridGizmos;
        public int MaxSize => _gridSizeX * _gridSizeY;

        private void Awake()
        {
            _worldGrid = GetComponentInParent<GridManager>().worldGrid;

            _nodeDiameter = gridDataSO.nodeRadius * 2;
            _gridSizeX = Mathf.RoundToInt((gridDataSO.gridWorldSize.x / _nodeDiameter));
            _gridSizeY = Mathf.RoundToInt((gridDataSO.gridWorldSize.y / _nodeDiameter));
        }

        public void Init()
        {
            CreateGrid();
        }

        private void CreateGrid()
        {
            _grid = new Node[_gridSizeX, _gridSizeY];
            Vector3 worldBottomLeft = transform.position - Vector3.right * gridDataSO.gridWorldSize.x / 2 - Vector3.forward * gridDataSO.gridWorldSize.y / 2;

            for (int x = 0; x < _gridSizeX; x++)
            {
                for (int y = 0; y < _gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * _nodeDiameter + gridDataSO.nodeRadius) + Vector3.forward * (y * _nodeDiameter + gridDataSO.nodeRadius);
                    bool walkable = !(Physics.CheckSphere(worldPoint, 2 * gridDataSO.nodeRadius + carveRadius, unwalkableLayerMask));

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

                    if(checkX >= 0 && checkX < _gridSizeX && checkY >= 0 && checkY < _gridSizeY) neighbours.Add(_grid[checkX, checkY]);
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

            int x = Mathf.RoundToInt((_gridSizeX - 1) * percentX);
            int y = Mathf.RoundToInt((_gridSizeY - 1) * percentY);
            return _grid[x, y];
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridDataSO.gridWorldSize.x,1, gridDataSO.gridWorldSize.y));

            if (_grid == null || !displayGridGizmos) return;
            foreach (Node node in _grid)
            {
                Gizmos.color = node.walkable ? Color.white : Color.red;
                Gizmos.DrawCube(node.WorldPosition, Vector3.one * (_nodeDiameter - .1f));
            }
        }
    }
}
