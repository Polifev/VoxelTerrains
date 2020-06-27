using System;
using UnityEngine;

namespace VoxelTerrains
{
    public class Chunk
    {
        public Vector3 Dimensions => new Vector3(_data.GetLength(0), _data.GetLength(1), _data.GetLength(2));

        private float[,,] _data;

        public Chunk(float[,,] data)
        {
            _data = data;
        }

        public Chunk(Vector3Int size)
        {
            _data = new float[size.x, size.y, size.z];
            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                    for (int z = 0; z < size.z; z++)
                        _data[x, y, z] = -1.0f;
        }

        public void SetValueAt(Vector3Int relativeLocation, float value)
        {
            if (relativeLocation.x >= Dimensions.x
                || relativeLocation.x < 0
                || relativeLocation.y >= Dimensions.y
                || relativeLocation.y < 0
                || relativeLocation.z >= Dimensions.z
                || relativeLocation.z < 0)
            {
                throw new ArgumentOutOfRangeException("Vector parameter is out of range");
            }

            _data[relativeLocation.x, relativeLocation.y, relativeLocation.z] = value;
        }

        public float ValueAt(Vector3Int relativeLocation)
        {
            return _data[relativeLocation.x, relativeLocation.y, relativeLocation.z];
        }
    }
}
