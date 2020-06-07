using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelTerrains
{
    [ExecuteInEditMode]
    public class DebugMarchingCube : MonoBehaviour
    {
        [SerializeField, Range(0, 255)]
        private int _caseIndex = 0;

        private Mesh _currentMesh;

        private void Start()
        {
            var configuration = VoxelManager.Instance.Configuration(_caseIndex);
            _currentMesh = new Mesh();
            _currentMesh.vertices = configuration.Vertices;
            _currentMesh.triangles = configuration.Triangles;
            _currentMesh.RecalculateNormals();
        }

        // Start is called before the first frame update
        void OnValidate()
        {
            var configuration = VoxelManager.Instance.Configuration(_caseIndex);
            _currentMesh = new Mesh();
            _currentMesh.vertices = configuration.Vertices;
            _currentMesh.triangles = configuration.Triangles;
            _currentMesh.RecalculateNormals();
        }

        private void OnDrawGizmos()
        {
            if (_currentMesh.vertices.Length > 0)
            {
                Mesh copy = new Mesh();

                var vertices = new Vector3[_currentMesh.vertices.Length];
                for(int i = 0; i < _currentMesh.vertices.Length; i++)
                {
                    vertices[i] = _currentMesh.vertices[i];
                }
                copy.vertices = vertices;

                var triangles = new int[_currentMesh.triangles.Length];
                for(int i = 0; i < _currentMesh.triangles.Length; i += 3)
                {
                    triangles[i] = _currentMesh.triangles[i];
                    triangles[i + 1] = _currentMesh.triangles[i + 2];
                    triangles[i + 2] = _currentMesh.triangles[i + 1];
                }
                copy.triangles = triangles;
                copy.RecalculateNormals();

                Gizmos.color = Color.blue;
                Gizmos.DrawMesh(_currentMesh, transform.position);
                Gizmos.color = Color.red;
                Gizmos.DrawMesh(copy, transform.position);
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
