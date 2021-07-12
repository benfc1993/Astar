using System;
using System.Collections.Generic;
using UnityEngine;
using World;

namespace Astar
{
    [CreateAssetMenu(fileName = "New Terrain Type", menuName = "Grid/Terrain Type")]
    public class TerrainTypeSO : ScriptableObject
    {
        [SerializeField] TerrainCost[] terrainTypes;
        public readonly Dictionary<TerrainType, int> terrainTypesDictionary = new Dictionary<TerrainType, int>();

        void OnEnable()
        {
            foreach (TerrainCost terrainType in terrainTypes)
            {
                terrainTypesDictionary.Add(terrainType.type, terrainType.cost);
            }
        }
    }

    [Serializable]
    public struct TerrainCost
    {
        public TerrainType type;
        public int cost;
    }
}
