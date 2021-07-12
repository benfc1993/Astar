using Grids;
using UnityEngine;
using Utils;

namespace Astar.Grid
{
    public class Node : IHeapItem<Node>, IGridNode
    {
        public bool walkable;
        public Vector3 WorldPosition { get; }
        public Node parent;

        public int X { get; }
        public int Y { get; }

        public int gCost;
        public int hCost;

        public int movementCost;

        int FCost => gCost + hCost;

        public int HeapIndex { get; set; }

        public Node(bool walkable, Vector3 worldPosition, int x, int y, int movementCost)
        {
            this.walkable = walkable;
            this.WorldPosition = worldPosition;
            this.X = x;
            this.Y = y;
            this.movementCost = movementCost;
        }

        public int CompareTo(Node toCompare)
        {
            int compare = FCost.CompareTo((toCompare.FCost));
            if(compare == 0)
                compare = hCost.CompareTo(toCompare.hCost);
            return -compare;
        }

        public void SetMovementPenalty(int movementPenalty)
        {
            movementCost = movementPenalty;
        }
    }
}
