using UnityEngine;

namespace Astar.Grid
{
    public interface IGridNode
    {
        public int X { get; }
        public int Y { get; }
        public Vector3 WorldPosition { get; }

    }
}
