using UnityEngine;

namespace Astar.Grid
{
    public class Node : IHeapItem<Node>
    {
        public readonly bool walkable;
        public Vector3 worldPosition;
        public Node parent;

        public int GridX { get; }
        public int GridY { get; }

        public int gCost;
        public int hCost;

        int FCost => gCost + hCost;

        public int HeapIndex { get; set; }

        public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY)
        {
            this.walkable = walkable;
            this.worldPosition = worldPosition;
            GridX = gridX;
            GridY = gridY;
        }

        public int CompareTo(Node toCompare)
        {
            int compare = FCost.CompareTo((toCompare.FCost));
            if(compare == 0)
                compare = hCost.CompareTo(toCompare.hCost);
            return -compare;
        }

    }
}
