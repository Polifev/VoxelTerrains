using System;
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
        [SerializeField]
        private AbstractScalarField _worldGenerator = null;
        [SerializeField]
        private Vector3Int _chunkSize = Vector3Int.one * 16;

        private Dictionary<Vector3Int, LinearChunk> _chunks = new Dictionary<Vector3Int, LinearChunk>();

        private void OnValidate()
        {
            _chunks = new Dictionary<Vector3Int, LinearChunk>();
        }

        public override float ValueAt(Vector3 vector)
        {
            var divided = new Vector3();
            divided.x = vector.x / _chunkSize.x;
            divided.y = vector.y / _chunkSize.y;
            divided.z = vector.z / _chunkSize.z;
            var rounded = Vector3Int.FloorToInt(divided);
            rounded.x *= _chunkSize.x;
            rounded.y *= _chunkSize.y;
            rounded.z *= _chunkSize.z;
            var localVector = vector - rounded;

            if (!_chunks.ContainsKey(rounded))
            {
                _chunks[rounded] = GenerateChunk(rounded);
            }
            return _chunks[rounded].ValueAt(localVector);
        }

        private LinearChunk GenerateChunk(Vector3Int chunkPosition)
        {
            var data = new float[_chunkSize.x, _chunkSize.y, _chunkSize.z];
            for(int x = 0; x < _chunkSize.x; x++)
                for (int y = 0; y < _chunkSize.y; y++)
                    for (int z = 0; z < _chunkSize.z; z++)
                    {
                        data[x, y, z] = _worldGenerator.ValueAt(chunkPosition + new Vector3(x, y, z));
                    }

            return new LinearChunk(data);
        }
    }
}
