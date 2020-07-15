using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VoxelTerrains.ScalarField
{
    [ExecuteInEditMode]
    public class ChunkBasedScalarField : AbstractEditableScalarField
    {
        // Private fields
        [SerializeField]
        private ComputeShader _worldGenerator = null;
        
        private static Vector3Int ChunkSize = Vector3Int.one * 64;

        private IDictionary<Vector3Int, Chunk> _chunks = new ConcurrentDictionary<Vector3Int, Chunk>();
        
        // Events
        public override event TerrainChangedEventHandler OnTerrainChanged;

        // Public methods
        public void ResetChunks()
        {
            _chunks = new Dictionary<Vector3Int, Chunk>();
        }

        public override float ValueAt(Vector3 location)
        {
            var floored = Vector3Int.FloorToInt(location);
            var ceiled = Vector3Int.CeilToInt(location);

            if (floored == ceiled)
            {
                return ValueFromChunk(floored);
            }
            else
            {
                ceiled = floored + Vector3Int.one;
            }

            float[] xInterpolations =
            {
                Mathf.Lerp(
                    ValueFromChunk(new Vector3Int(floored.x, floored.y, floored.z)),
                    ValueFromChunk(new Vector3Int(ceiled.x, floored.y, floored.z)),
                    (location.x - floored.x)),
                Mathf.Lerp(
                    ValueFromChunk(new Vector3Int(floored.x, ceiled.y, floored.z)),
                    ValueFromChunk(new Vector3Int(ceiled.x, ceiled.y, floored.z)),
                    (location.x - floored.x)),
                Mathf.Lerp(
                    ValueFromChunk(new Vector3Int(floored.x, ceiled.y, ceiled.z)),
                    ValueFromChunk(new Vector3Int(floored.x, ceiled.y, ceiled.z)),
                    (location.x - floored.x)),
                Mathf.Lerp(
                    ValueFromChunk(new Vector3Int(floored.x, floored.y, ceiled.z)),
                    ValueFromChunk(new Vector3Int(floored.x, floored.y, ceiled.z)),
                    (location.x - floored.x))
            };

            float[] yInterpolations =
            {
                Mathf.Lerp(xInterpolations[0], xInterpolations[1], (location.y - floored.y)),
                Mathf.Lerp(xInterpolations[3], xInterpolations[2], (location.y - floored.y))
            };

            float zInterpolation = Mathf.Lerp(yInterpolations[0], yInterpolations[1], (location.z - floored.z));
            return zInterpolation;
        }

        public override void AddValueAt(Vector3 location, float value)
        {
            var floored = Vector3Int.FloorToInt(location);
            var ceiled = Vector3Int.CeilToInt(location);

            if (floored == ceiled)
            {
                AddValueInChunk(floored, value);
            }
            else
            {
                ceiled = floored + Vector3Int.one;
                AddValueInChunk(new Vector3Int(floored.x, floored.y, floored.z), value);
                AddValueInChunk(new Vector3Int(ceiled.x, floored.y, floored.z), value);
                AddValueInChunk(new Vector3Int(ceiled.x, floored.y, ceiled.z), value);
                AddValueInChunk(new Vector3Int(floored.x, floored.y, ceiled.z), value);
                AddValueInChunk(new Vector3Int(floored.x, ceiled.y, floored.z), value);
                AddValueInChunk(new Vector3Int(ceiled.x, ceiled.y, floored.z), value);
                AddValueInChunk(new Vector3Int(ceiled.x, ceiled.y, ceiled.z), value);
                AddValueInChunk(new Vector3Int(floored.x, ceiled.y, ceiled.z), value);
            }
            OnTerrainChanged?.Invoke(location);
        }

        // Private methods
        private float ValueFromChunk(Vector3Int vector)
        {
            var chunkIndex = Util.GetChunkIndex(vector, ChunkSize);
            var chunkPosition = Util.MultiplyCoordsInt(chunkIndex, ChunkSize);
            var localVector = vector - chunkPosition;
            if (!_chunks.ContainsKey(chunkIndex))
            {
                _chunks[chunkIndex] = GenerateChunk(chunkPosition);
            }
            return _chunks[chunkIndex].ValueAt(localVector);
        }

        private void AddValueInChunk(Vector3Int vector, float value)
        {
            var chunkIndex = Util.GetChunkIndex(vector, ChunkSize);
            var chunkPosition = Util.MultiplyCoordsInt(chunkIndex, ChunkSize);
            var localVector = vector - chunkPosition;
            if (!_chunks.ContainsKey(chunkIndex))
            {
                _chunks[chunkIndex] = GenerateChunk(chunkPosition);
            }
            _chunks[chunkIndex].SetValueAt(localVector, Mathf.Clamp(_chunks[chunkIndex].ValueAt(localVector) + value, -1.0f, 1.0f));
        }

        private Chunk GenerateChunk(Vector3Int chunkPosition)
        {
            var bufferSize = ChunkSize.x * ChunkSize.y * ChunkSize.z;

            ComputeBuffer cornersBuffer = new ComputeBuffer(bufferSize, sizeof(float) * 4);

            _worldGenerator.SetBuffer(0, "corners", cornersBuffer);
            _worldGenerator.SetVector("offset", new Vector4(chunkPosition.x, chunkPosition.y, chunkPosition.z, 0));
            _worldGenerator.SetInts("size", new int[]{ ChunkSize.x, ChunkSize.y, ChunkSize.z});
            _worldGenerator.Dispatch(0, 8, 8, 8);

            var data = new Vector4[ChunkSize.x * ChunkSize.y * ChunkSize.z];
            cornersBuffer.GetData(data);

            var chunkData = new float[ChunkSize.x, ChunkSize.y, ChunkSize.z];
            for (int x = 0; x < ChunkSize.x; x++)
                for (int y = 0; y < ChunkSize.y; y++)
                    for (int z = 0; z < ChunkSize.z; z++)
                    {
                        chunkData[x, y, z] = data[x * 64 * 64 + y * 64 + z].w;
                    }

            cornersBuffer.Release();

            return new Chunk(chunkData);
        }
    }
}
