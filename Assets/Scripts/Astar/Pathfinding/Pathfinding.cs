using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Astar.Grid;
using UnityEngine;
using System;

namespace Astar.Pathfinding
{
    public class Pathfinding : MonoBehaviour
    {
        private AStarGrid _grid;
        PathRequestManager _pathRequestManager;

        private void Awake()
        {
            _grid = GetComponent<AStarGrid>();
            _pathRequestManager = GetComponent<PathRequestManager>();
        }


        public void StartFindPath(Vector3 pathStart, Vector3 pathEnd)
        {
            StartCoroutine(FindPath(pathStart, pathEnd));
        }

        IEnumerator FindPath(Vector3 startPoint, Vector3 targetPoint)
        {
            //TODO: remove for production
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Vector3[] waypoints = new Vector3[0];
            bool pathSucess = false;

            Node startNode = _grid.NodeFromWorldPoint(startPoint);
            Node targetNode = _grid.NodeFromWorldPoint(targetPoint);

            if (startNode.walkable && targetNode.walkable)
            {
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
                        pathSucess = true;
                        break;
                    }

                    foreach (Node neighbour in _grid.GetNeighbours(currentNode))
                    {
                        if(!neighbour.walkable || closedSet.Contains(neighbour)) continue;
                        int newMovementCost = GetDistance(currentNode, neighbour) + currentNode.gCost + currentNode.movementCost;
                        if (newMovementCost < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newMovementCost;
                            neighbour.hCost = GetDistance(neighbour, targetNode);
                            neighbour.parent = currentNode;

                            if(!openSet.Contains(neighbour)) openSet.Add(neighbour);
                            else
                                openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }

            yield return null;
            if(pathSucess)
                waypoints = RetracePath(startNode, targetNode);
            _pathRequestManager.FinishedProcessingPath(waypoints, pathSucess);
        }

        Vector3[] RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parent;
            }

            Vector3[] waypoints = SimplifyPath(path);
            Array.Reverse(waypoints);
            return waypoints;
        }

        Vector3[] SimplifyPath(List<Node> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;

            for (int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew =
                    new Vector2(path[i - 1].X - path[i].X, path[i - 1].Y - path[i].Y);
                if (directionOld != directionNew)
                    waypoints.Add(path[i].WorldPosition);
                directionOld = directionNew;
            }

            return waypoints.ToArray();
        }

        int GetDistance(Node nodeA, Node nodeB)
        {
            int distX = Mathf.Abs(nodeA.X - nodeB.X);
            int distY = Mathf.Abs(nodeA.Y - nodeB.Y);

            if (distY < distX)
                return 14 * distY + 10 * (distX - distY);
            return 14 * distX + 10 * (distY - distX);
        }

    }
}
