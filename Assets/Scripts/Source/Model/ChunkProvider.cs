using System.Collections.Generic;
using UnityEngine;

namespace VoxelTerrains.Model
{
    public class ChunkProvider
    {
        private WorldGenerator _worldGenerator;
        private IChunkRepository _chunkRepository;
        private IDictionary<Vector3Int, Chunk> _world;

        public ChunkProvider(WorldGenerator worldGenerator, IChunkRepository chunkRepository)
        {
            _world = new Dictionary<Vector3Int, Chunk>();
            _worldGenerator = worldGenerator;
            _chunkRepository = chunkRepository;
        }

        public ComputeBuffer FillBuffer(Vector3Int chunkIndex, ComputeBuffer buffer)
        {
            if (_world.ContainsKey(chunkIndex))
            {
                buffer.SetData(_world[chunkIndex].Data);
            }
            else if (_chunkRepository.HasChunk(chunkIndex))
            {
                buffer.SetData(_chunkRepository.LoadChunk(chunkIndex).Data);
            }
            else
            {
                var chunk = _worldGenerator.GenerateChunk(chunkIndex);
                AutoSaver.Instance.ChunksToSave.Enqueue(new LocatedChunk() {
                    ChunkIndex = chunkIndex,
                    Chunk = chunk 
                });
                buffer.SetData(chunk.Data);
                _world.Add(chunkIndex, chunk);
            }
            return buffer;
        }
    }
}
