using System;
using System.IO;
using UnityEngine;

namespace VoxelTerrains.Model
{
    public class FileSystemChunkRepository : IChunkRepository
    {
        private static readonly int FLOAT_SIZE = 4;

        private string _chunksDirectory;

        public FileSystemChunkRepository(string chunkDirectory)
        {
            _chunksDirectory = chunkDirectory;
        }

        public bool HasChunk(Vector3Int chunkIndex)
        {
            return File.Exists(GetFilePath(chunkIndex));
        }

        public Chunk LoadChunk(Vector3Int chunkIndex)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(GetFilePath(chunkIndex))))
            {
                long length = reader.BaseStream.Length / FLOAT_SIZE;
                float[] data = new float[length];
                for(var i = 0; i < length; i++)
                {
                    data[i] = reader.ReadSingle();
                }
                return new Chunk(data);
            }
        }

        public void SaveChunk(Vector3Int chunkIndex, Chunk chunk)
        {
            using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(GetFilePath(chunkIndex))))
            {
                for (var i = 0; i < chunk.Data.Length; i++)
                {
                    writer.Write(chunk.Data[i]);
                }
            }
        }

        public string GetFilePath(Vector3Int chunkIndex)
        {
            var filename =  $"x{chunkIndex.x}y{chunkIndex.y}z{chunkIndex.z}.cnk";
            return Path.Combine(_chunksDirectory, filename);
        }
    }
}
