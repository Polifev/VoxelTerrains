using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VoxelTerrains.ScalarField;
using VoxelTerrains.ScriptableObjects;

namespace VoxelTerrains.Renderer
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    public class VoxelRenderer : MonoBehaviour
    {
        [SerializeField]
        private CubeConfigurations _configurations;
        [SerializeField, Range(0.001f, 10.0f)]
        private float _tileSize = 0.1f;
        [SerializeField]
        private Vector3 _size = Vector3.one;
        [SerializeField]
        private AbstractScalarField _scalarField;

        private MeshFilter _meshFilter = null;

        public void RefreshMesh()
        {
            _meshFilter = GetComponent<MeshFilter>();

            IList<Vector3> vertices = new List<Vector3>();
            IList<int> triangles = new List<int>();

            IDictionary<Vector3, int> alreadyPresentVertices = new Dictionary<Vector3, int>();

            for(float x = (-_size/2).x; x < (_size / 2).x; x += _tileSize)
                for(float y = (-_size / 2).y; y < (_size / 2).y; y += _tileSize)
                    for (float z = (-_size / 2).z; z < (_size / 2).z; z += _tileSize)
                    {
                        int configurationIndex = ComputeIndex(x + transform.position.x, y + transform.position.y, z + transform.position.z);
                        var configuration = _configurations.Configurations[configurationIndex];
                        for(int i = 0; i < configuration.Vertices.Length; i++)
                        {
                            var vertice = new Vector3(x, y, z) + (configuration.Vertices[i]) * _tileSize;
                            if (!vertices.Contains(vertice))
                            {
                                var index = vertices.Count;
                                vertices.Add(vertice);
                                alreadyPresentVertices.Add(vertice, index);
                            }
                        }
                        for (int i = 0; i < configuration.Triangles.Length; i++)
                        {
                            var correspondingVertice = new Vector3(x, y, z) + configuration.Vertices[configuration.Triangles[i]] * _tileSize;
                            triangles.Add(alreadyPresentVertices[correspondingVertice]);
                        }
                    }

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            _meshFilter.mesh = mesh;
        }

        private int ComputeIndex(float x, float y, float z)
        {
            int index = 0;
            if (_scalarField.ValueAt(new Vector3(x, y, z)) > 0f)
            {
                index += 1;
            }
            if (_scalarField.ValueAt(new Vector3(x + _tileSize, y, z)) > 0f)
            {
                index += 2;
            }
            if (_scalarField.ValueAt(new Vector3(x + _tileSize, y, z + _tileSize)) > 0f)
            {
                index += 4;
            }
            if (_scalarField.ValueAt(new Vector3(x, y, z + _tileSize)) > 0f)
            {
                index += 8;
            }
            if (_scalarField.ValueAt(new Vector3(x, y + _tileSize, z)) > 0f)
            {
                index += 16;
            }
            if (_scalarField.ValueAt(new Vector3(x + _tileSize, y + _tileSize, z)) > 0f)
            {
                index += 32;
            }
            if (_scalarField.ValueAt(new Vector3(x + _tileSize, y + _tileSize, z + _tileSize)) > 0f)
            {
                index += 64;
            }
            if (_scalarField.ValueAt(new Vector3(x, y + _tileSize, z + _tileSize)) > 0f)
            {
                index += 128;
            }
            return index;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, _size);
        }
    }
}
