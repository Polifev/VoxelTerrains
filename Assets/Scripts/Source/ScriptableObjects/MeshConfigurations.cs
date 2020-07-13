using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelTerrains.ScriptableObjects
{
    [Serializable]
    public struct MeshConfiguration
    {
        public Vector3[] Vertices;
        public int[] Triangles;
    }

    [CreateAssetMenu(fileName = "DefaultConfigurations", menuName = "ScriptableObjects/MeshConfigurations", order = 1)]
    public class MeshConfigurations : ScriptableObject
    {
        public MeshConfiguration[] Configurations;
    }
}
