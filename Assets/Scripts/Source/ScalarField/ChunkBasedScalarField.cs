﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VoxelTerrains.ScalarField
{
    [ExecuteInEditMode]
    public class ChunkBasedScalarField : AbstractScalarField
    {
        // Private fields
        [SerializeField]
        private AbstractScalarField _worldGenerator = null;
        [SerializeField]
        private Vector3Int _chunkSize = Vector3Int.one * 16;
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

        public void AddValueAt(Vector3 location, float value)
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
            var chunkIndex = Util.GetChunkIndex(vector, _chunkSize);
            var chunkPosition = Util.MultiplyCoordsInt(chunkIndex, _chunkSize);
            var localVector = vector - chunkPosition;
            if (!_chunks.ContainsKey(chunkIndex))
            {
                _chunks[chunkIndex] = GenerateChunk(chunkPosition);
            }
            return _chunks[chunkIndex].ValueAt(localVector);
        }

        private void AddValueInChunk(Vector3Int vector, float value)
        {
            var chunkIndex = Util.GetChunkIndex(vector, _chunkSize);
            var chunkPosition = Util.MultiplyCoordsInt(chunkIndex, _chunkSize);
            var localVector = vector - chunkPosition;
            if (!_chunks.ContainsKey(chunkIndex))
            {
                _chunks[chunkIndex] = GenerateChunk(chunkPosition);
            }
            _chunks[chunkIndex].SetValueAt(localVector, Mathf.Clamp(_chunks[chunkIndex].ValueAt(localVector) + value, -1.0f, 1.0f));
        }

        private Chunk GenerateChunk(Vector3Int chunkPosition)
        {
            var data = new float[_chunkSize.x, _chunkSize.y, _chunkSize.z];
            for(int x = 0; x < _chunkSize.x; x++)
                for (int y = 0; y < _chunkSize.y; y++)
                    for (int z = 0; z < _chunkSize.z; z++)
                    {
                        data[x, y, z] = _worldGenerator.ValueAt(chunkPosition + new Vector3(x, y, z));
                    }
            return new Chunk(data);
        }
    }
}
