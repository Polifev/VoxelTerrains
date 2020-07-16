using UnityEngine;

namespace VoxelTerrains.Model
{
    public class WorldGenerator
    {
        public ComputeShader GeneratorShader { get; set; }

        public Chunk GenerateChunk(Vector3Int chunkIndex)
        {
            var bufferSize = Chunk.SIZE * Chunk.SIZE * Chunk.SIZE;

            ComputeBuffer chunkBuffer = new ComputeBuffer(bufferSize, sizeof(float));

            GeneratorShader.SetBuffer(0, "chunk", chunkBuffer);
            GeneratorShader.SetVector("chunkPosition", (Vector3)chunkIndex * (Chunk.SIZE));
            GeneratorShader.Dispatch(0, 8, 8, 8);

            var data = new float[bufferSize];
            chunkBuffer.GetData(data);

            chunkBuffer.Release();
            return new Chunk(data);
        }
    }
}
