using UnityEngine;

namespace VoxelTerrains.Model
{
    public class LocatedChunk
    {
        public Vector3Int ChunkIndex {get; set;}
        public Chunk Chunk { get; set; }
    }
}
