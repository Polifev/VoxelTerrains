using System;
using UnityEngine;

namespace VoxelTerrains
{
    public class LinearChunk
    {
        public Vector3 Dimensions => new Vector3(_data.GetLength(0), _data.GetLength(1), _data.GetLength(2));

        private float[,,] _data;

        public LinearChunk(float[,,] data)
        {
            _data = data;
        }

        public LinearChunk(Vector3Int size)
        {
            _data = new float[size.x, size.y, size.z];
            for (int x = 0; x < size.x; x++)
                for (int y = 0; y < size.y; y++)
                    for (int z = 0; z < size.z; z++)
                        _data[x, y, z] = -1.0f;
        }

        public void SetNearestValueAt(Vector3 relativeLocation, float value)
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

            var floored = Vector3Int.FloorToInt(relativeLocation);
            _data[floored.x, floored.y, floored.z] = value;
        }

        public float ValueAt(Vector3 relativeLocation)
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

            var floored = Vector3Int.FloorToInt(relativeLocation);
            var ceiled = Vector3Int.CeilToInt(relativeLocation);

            if (floored == ceiled)
            {
                return _data[floored.x, floored.y, floored.z];
            }
            else
            {
                ceiled = floored + Vector3Int.one;
            }

            float[] xInterpolations =
            {
                Mathf.Lerp(
                    _data[floored.x, floored.x, floored.x],
                    _data[ceiled.x, floored.x, floored.x],
                    (relativeLocation.x - floored.x)),
                Mathf.Lerp(
                    _data[floored.x, ceiled.x, floored.x],
                    _data[ceiled.x, ceiled.x, floored.x],
                    (relativeLocation.x - floored.x)),
                Mathf.Lerp(
                    _data[floored.x, ceiled.x, ceiled.x],
                    _data[ceiled.x, ceiled.x, ceiled.x],
                    (relativeLocation.x - floored.x)),
                Mathf.Lerp(
                    _data[floored.x, floored.x, ceiled.x],
                    _data[ceiled.x, floored.x, ceiled.x],
                    (relativeLocation.x - floored.x))
            };

            float[] yInterpolations =
            {
                Mathf.Lerp(xInterpolations[0], xInterpolations[1], (relativeLocation.y - floored.y)),
                Mathf.Lerp(xInterpolations[3], xInterpolations[2], (relativeLocation.y - floored.y))
            };

            float zInterpolation = Mathf.Lerp(yInterpolations[0], yInterpolations[1], (relativeLocation.z - floored.z));
            return zInterpolation;
        }
    }
}
