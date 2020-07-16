using System;
using UnityEngine;

namespace VoxelTerrains
{
    public class Chunk
    {
        public static int SIZE = 64;
        public float[] Data;

        public Chunk(float[] data)
        {
            Data = data;
        }
    }
}
