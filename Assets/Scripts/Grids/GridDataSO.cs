using UnityEngine;

namespace Grids
{
    [CreateAssetMenu(fileName = "New Grid Data", menuName = "Grid/Grid Data")]
    public class GridDataSO : ScriptableObject
    {
        public Vector2 gridWorldSize;
        public float nodeRadius;
    }
}
