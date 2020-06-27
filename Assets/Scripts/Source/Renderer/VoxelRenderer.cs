using System;
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
    [RequireComponent(typeof(MeshCollider))]
    public class VoxelRenderer : MonoBehaviour
    {
        [SerializeField]
        private bool _debugMode = false;
        [SerializeField]
        private bool _realTime = false;
        
        [SerializeField]
        private CubeConfigurations _configurations = null;
        [SerializeField, Range(0.001f, 10.0f)]
        private float _tileSize = 1.0f;
        [SerializeField]
        private Vector3 _size = Vector3.one;
        [SerializeField]
        private AbstractScalarField _scalarField = null;

        public AbstractScalarField ScalarField
        {
            get => _scalarField;
            set => _scalarField = value;
        }

        public Vector3 Size
        {
            get => _size;
            set => _size = value;
        }

        private MeshFilter _meshFilter = null;
        private MeshCollider _meshCollider = null;

        public void Update()
        {
            if (_realTime)
            {
                RefreshMesh();
            }
        }

        public void RefreshMesh()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();

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
                            try
                            {
                                var correspondingVertice = new Vector3(x, y, z) + configuration.Vertices[configuration.Triangles[i]] * _tileSize;
                                triangles.Add(alreadyPresentVertices[correspondingVertice]);
                            }catch (Exception e)
                            {
                                Debug.LogError(e);
                            }
                            
                        }
                    }

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            _meshFilter.sharedMesh = mesh;
            _meshCollider.sharedMesh = mesh;
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

            if (_debugMode)
            {
                Handles.color = Color.red;
                for(float x = -_size.x / 2; x < _size.x/2+1; x++)
                    for (float y = -_size.y / 2; y < _size.y/2+1; y++)
                        for (float z = -_size.z / 2; z < _size.z/2+1; z++)
                        {
                            var style = GUIStyle.none;
                            style.richText = true;
                            var v = new Vector3(x, y, z) + transform.position;
                            //Handles.Label(v, $"<color=red>{}</color>", style);
                            if(_scalarField.ValueAt(v) > 0)
                            {
                                Gizmos.color = Color.blue;
                            }
                            else
                            {
                                Gizmos.color = Color.red;
                            }
                            
                            Gizmos.DrawSphere(v, 0.1f);
                        }
            }
            
        }
    }
}
