using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelTerrains.ScriptableObjects
{
    [Serializable]
    public struct CubeConfiguration
    {
        public Vector3[] Vertices;
        public int[] Triangles;
    }

    [CreateAssetMenu(fileName = "DefaultConfigurations", menuName = "ScriptableObjects/CubeConfigurations", order = 1)]
    public class CubeConfigurations : ScriptableObject
    {
        public CubeConfiguration[] Configurations;
    }
}
