using UnityEngine;

namespace Grids
{
    public interface IGridNode
    {
        public int X { get; }
        public int Y { get; }
        public Vector3 WorldPosition { get; }

    }
}
