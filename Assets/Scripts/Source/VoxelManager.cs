using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelTerrains.Renderer;
using VoxelTerrains.ScalarField;

namespace VoxelTerrains
{
    public class VoxelManager : MonoBehaviour
    {
        [SerializeField]
        private AbstractScalarField _terrain;
        [SerializeField]
        private int _rendererSize = 4;
        [SerializeField]
        private GameObject _voxelRendererPrefab;

        private Dictionary<Vector3Int, VoxelRenderer> _renderers = new Dictionary<Vector3Int, VoxelRenderer>();

        private void Start()
        {
            for(int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    for (int z = 0; z < 4; z++)
                    {
                        InstantiateRenderer(new Vector3Int(x, y, z)).RefreshMesh();
                    }
                }
            }

            _terrain.OnTerrainChanged += RenderChunksAround;
        }

        private void RenderChunksAround(Vector3 location)
        {
            var temp = new Vector3();
            temp.x = location.x / _rendererSize;
            temp.y = location.y / _rendererSize;
            temp.z = location.z / _rendererSize;
            Vector3Int rendererIndex = Vector3Int.FloorToInt(temp);

            Vector3Int[] updateIndexes = new Vector3Int[27];
            updateIndexes[0] = rendererIndex;
            // faces
            updateIndexes[1] = rendererIndex + new Vector3Int(1, 0, 0);
            updateIndexes[2] = rendererIndex + new Vector3Int(-1, 0, 0);
            updateIndexes[3] = rendererIndex + new Vector3Int(0, 1, 0);
            updateIndexes[4] = rendererIndex + new Vector3Int(0, -1, 0);
            updateIndexes[5] = rendererIndex + new Vector3Int(0, 0, 1);
            updateIndexes[6] = rendererIndex + new Vector3Int(0, 0, -1);

            // edges
            updateIndexes[7] = rendererIndex + new Vector3Int(1, 1, 0);
            updateIndexes[8] = rendererIndex + new Vector3Int(1, -1, 0);
            updateIndexes[9] = rendererIndex + new Vector3Int(-1, 1, 0);
            updateIndexes[9] = rendererIndex + new Vector3Int(-1, -1, 0);

            updateIndexes[7] = rendererIndex + new Vector3Int(1, 0, 1);
            updateIndexes[8] = rendererIndex + new Vector3Int(1, 0, -1);
            updateIndexes[9] = rendererIndex + new Vector3Int(-1, 0, 1);
            updateIndexes[9] = rendererIndex + new Vector3Int(-1, 0, -1);

            updateIndexes[7] = rendererIndex + new Vector3Int(0, 1, 1);
            updateIndexes[8] = rendererIndex + new Vector3Int(0, 1, -1);
            updateIndexes[9] = rendererIndex + new Vector3Int(0, -1, 1);
            updateIndexes[9] = rendererIndex + new Vector3Int(0, -1, -1);

            // vertices
            updateIndexes[9] = rendererIndex + new Vector3Int(1, 1, 1);
            updateIndexes[9] = rendererIndex + new Vector3Int(1, 1, -1);
            updateIndexes[9] = rendererIndex + new Vector3Int(1, -1, 1);
            updateIndexes[9] = rendererIndex + new Vector3Int(1, -1, -1);
            updateIndexes[9] = rendererIndex + new Vector3Int(-1, 1, 1);
            updateIndexes[9] = rendererIndex + new Vector3Int(-1, 1, -1);
            updateIndexes[9] = rendererIndex + new Vector3Int(-1, -1, 1);
            updateIndexes[9] = rendererIndex + new Vector3Int(-1, -1, -1);


            foreach (Vector3Int index in updateIndexes)
            {
                if (!_renderers.ContainsKey(index))
                {
                    InstantiateRenderer(index);
                }

                _renderers[index].RefreshMesh();
            }
            
        }

        private VoxelRenderer InstantiateRenderer(Vector3Int index)
        {
            var instance = Instantiate(_voxelRendererPrefab, this.transform);
            instance.transform.Translate(index * _rendererSize);

            var renderer = instance.GetComponent<VoxelRenderer>();
            renderer.ScalarField = _terrain;
            renderer.Size = Vector3.one * _rendererSize;
            _renderers.Add(index, renderer);
            return renderer;
        }
    }
}

