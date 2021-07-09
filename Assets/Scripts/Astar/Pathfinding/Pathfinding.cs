using System.Collections.Generic;
using System.Diagnostics;
using Astar.Grid;
using UnityEngine;

namespace Astar.Pathfinding
{
    public class Pathfinding : MonoBehaviour
    {
        public Transform seeker;
        public Transform target;
        private AStarGrid _grid;

        private void Awake() => _grid = GetComponent<AStarGrid>();

        private void Update()
        {
            if(Input.GetButtonDown("Jump"))
                FindPath(seeker.position, target.position);
        }

        private void FindPath(Vector3 startPoint, Vector3 targetPoint)
        {
            //TODO: remove for production
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Node startNode = _grid.NodeFromWorldPoint(startPoint);
            Node targetNode = _grid.NodeFromWorldPoint(targetPoint);

            Heap<Node> openSet = new Heap<Node>(_grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add((startNode));

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    print($"path found in {sw.ElapsedMilliseconds}ms");
                    RetracePath(startNode, targetNode);
                    return;
                }

                foreach (Node neighbour in _grid.GetNeighbours(currentNode))
                {
                    if(!neighbour.walkable || closedSet.Contains(neighbour)) continue;
                    int newMovementCost = GetDistance(currentNode, neighbour) + currentNode.gCost;
                    if (newMovementCost < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCost;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if(!openSet.Contains(neighbour)) openSet.Add(neighbour);
                    }
                }
            }

        }

        void RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            path.Reverse();
            _grid.path = path;
        }

        int GetDistance(Node nodeA, Node nodeB)
        {
            int distX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            int distY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

            if (distY < distX)
                return 14 * distY + 10 * (distX - distY);
            return 14 * distX + 10 * (distY - distX);
        }
    }
}
