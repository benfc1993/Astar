using UnityEngine;

namespace Astar.Grid
{
    public class Node
    {
        public bool walkable;
        public Vector3 worldPosition;
        public Node parent;

        public int GridX { get; }
        public int GridY { get; }

        public int gCost;
        public int hCost;

        public int fCost => gCost + hCost;

        public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY)
        {
            this.walkable = walkable;
            this.worldPosition = worldPosition;
            this.GridX = gridX;
            this.GridY = gridY;
        }
    }
}
