using UnityEngine;

namespace VoxelTerrains.Model
{
    public interface IChunkRepository
    {
        bool HasChunk(Vector3Int chunkIndex);
        Chunk LoadChunk(Vector3Int chunkIndex);
        void SaveChunk(Vector3Int chunkIndex, Chunk chunk);
    }
}
