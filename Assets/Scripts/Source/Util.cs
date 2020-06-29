using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VoxelTerrains
{
    public class Util
    {
        public static Vector3Int GetChunkIndex(Vector3 position, Vector3Int chunkSize)
        {
            int x = Mathf.FloorToInt(position.x / chunkSize.x);
            int y = Mathf.FloorToInt(position.y / chunkSize.y);
            int z = Mathf.FloorToInt(position.z / chunkSize.z);
            return new Vector3Int(x, y, z);
        }

        public static IEnumerable<Vector2Int> GetSquaredSpiral(int total)
        {
            var lastPosition = Vector2Int.zero;
            var currentOrientation = 0;
            var orientations = new Vector2Int[]
            {
                        new Vector2Int(0, 1),
                        new Vector2Int(1, 0),
                        new Vector2Int(0, -1),
                        new Vector2Int(-1, 0)
            };

            for (int i = 0; i < total; i++)
            {
                for (int twice = 0; twice < 2; twice++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        yield return lastPosition;
                        lastPosition = lastPosition + orientations[currentOrientation];
                    }
                    currentOrientation = (currentOrientation + 1) % 4;
                }
            }
        }

        public static Vector3Int MultiplyCoordsInt(Vector3Int u, Vector3Int v)
        {
            Vector3Int result = u;
            result.x = u.x * v.x;
            result.y = u.y * v.y;
            result.z = u.z * v.z;
            return result;
        }
    }
}
