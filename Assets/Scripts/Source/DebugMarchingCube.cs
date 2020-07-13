using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelTerrains.ScriptableObjects;

namespace VoxelTerrains
{
    [ExecuteInEditMode]
    public class DebugMarchingCube : MonoBehaviour
    {
        [SerializeField]
        private MeshConfigurations _configurations = null;
        [SerializeField, Range(0, 255)]
        private int _caseIndex = 0;

        private Mesh _currentMesh;
        private Mesh _invertedMesh;

        private void Start()
        {
            
        }

        // Start is called before the first frame update
        void OnValidate()
        {
            if (_configurations == null)
                return;

            var configuration = _configurations.Configurations[_caseIndex];
            _currentMesh = new Mesh();
            _invertedMesh = new Mesh();

            _currentMesh.vertices = configuration.Vertices;
            _currentMesh.triangles = configuration.Triangles;
            _currentMesh.RecalculateNormals();

            
            _invertedMesh.vertices = _currentMesh.vertices;
            var invertedTriangles = new int[_currentMesh.triangles.Length];
            for (int i = 0; i < _currentMesh.triangles.Length; i += 3)
            {
                invertedTriangles[i] = _currentMesh.triangles[i];
                invertedTriangles[i + 1] = _currentMesh.triangles[i + 2];
                invertedTriangles[i + 2] = _currentMesh.triangles[i + 1];
            }
            _invertedMesh.triangles = invertedTriangles;
            _invertedMesh.RecalculateNormals();
        }

        private void OnDrawGizmos()
        {
            // Draw meshes
            if (_currentMesh.vertices.Length > 0)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawMesh(_currentMesh, transform.position);
                Gizmos.color = Color.red;
                Gizmos.DrawMesh(_invertedMesh, transform.position);
            }

            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position + new Vector3(0.5f, 0.5f, 0.5f), Vector3.one);
            Vector3[] vectors = new Vector3[]
            {
                new Vector3(0,0,0),
                new Vector3(1,0,0),
                new Vector3(1,0,1),
                new Vector3(0,0,1),
                new Vector3(0,1,0),
                new Vector3(1,1,0),
                new Vector3(1,1,1),
                new Vector3(0,1,1),
            };

            Gizmos.color = Color.green;
            int temp = _caseIndex;
            for(int i = 7; i >= 0; i--)
            {
                int pow = Pow(2, i);
                if (temp >= pow)
                {
                    temp -= pow;
                    Gizmos.DrawSphere(transform.position + vectors[i], 0.1f);
                }
            }
        }
        private static int Pow(int m, int n)
        {
            if(n < 0)
            {
                return -1;
            }
            else if (n == 0)
            {
                return 1;
            }
            else
            {
                return m * Pow(m, n - 1);
            }
        }
    }
}
