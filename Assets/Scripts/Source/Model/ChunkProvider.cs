using System.Collections.Generic;
using UnityEngine;

namespace VoxelTerrains.Model
{
    public class ChunkProvider
    {
        private WorldGenerator _worldGenerator;
        private IDictionary<Vector3Int, Chunk> _world;

        public ChunkProvider(WorldGenerator worldGenerator)
        {
            _world = new Dictionary<Vector3Int, Chunk>();
            _worldGenerator = worldGenerator;
        }

        public ComputeBuffer FillBuffer(Vector3Int chunkIndex, ComputeBuffer buffer)
        {
            if (_world.ContainsKey(chunkIndex))
            {
                buffer.SetData(_world[chunkIndex].Data);
            }
            else
            {
                var chunk = _worldGenerator.GenerateChunk(chunkIndex);
                buffer.SetData(chunk.Data);
                _world.Add(chunkIndex, chunk);
            }
            return buffer;
        }

        public Chunk this[Vector3Int chunkIndex] {
            get => _world[chunkIndex];
        }
    }
}
