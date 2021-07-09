using System;
using System.Collections.Generic;
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
            FindPath(seeker.position, target.position);
        }

        private void FindPath(Vector3 startPoint, Vector3 targetPoint)
        {
            Node startNode = _grid.NodeFromWorldPoint(startPoint);
            Node targetNode = _grid.NodeFromWorldPoint(targetPoint);

            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            
            openSet.Add((startNode));

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 0; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost ||
                        openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
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
