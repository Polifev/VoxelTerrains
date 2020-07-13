using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoxelTerrains.ScriptableObjects;


[ExecuteInEditMode]
public class DebugMarchingTetrahedrons : MonoBehaviour
{
    private static int[,] Tetrahedrons = new int[,]{
            { 0, 1, 2, 4 },
            { 0, 3, 2, 4 },
            { 3, 7, 4, 2 },
            { 6, 7, 4, 2 },
            { 5, 6, 2, 4 },
            { 5, 1, 2, 4 }
        };

    [SerializeField]
    private MeshConfigurations _configurations = null;
    [SerializeField]
    private bool[] _corners = new bool[8];

    private Mesh _currentMesh;
    private Mesh _invertedMesh;

    // Start is called before the first frame update
    void Update()
    {
        if (_configurations == null)
            return;

        _currentMesh = new Mesh();
        _invertedMesh = new Mesh();

        IList<Vector3> vertices = new List<Vector3>();
        IList<int> triangles = new List<int>();

        int[] configurations = ComputeIndices();

        for (int tetraIndex = 0; tetraIndex < Tetrahedrons.GetLength(0); tetraIndex++)
        {
            var caseIndex = configurations[tetraIndex];
            var configuration = _configurations.Configurations[tetraIndex * 16 + caseIndex];

            int zero = vertices.Count;
            for (int i = 0; i < configuration.Vertices.Length; i++)
            {
                var vertice = configuration.Vertices[i];
                vertices.Add(vertice);
            }
            for (int i = 0; i < configuration.Triangles.Length; i++)
            {
                triangles.Add(zero + configuration.Triangles[i]);
            }
        }

        _currentMesh.vertices = vertices.ToArray();
        _currentMesh.triangles = triangles.ToArray();
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
        
        var colors = new Color[_invertedMesh.vertexCount];
        for (int i = 0; i < _invertedMesh.colors.Length; i++)
        {
            colors[i] = Color.red;
        }
        _invertedMesh.colors = colors;


        GetComponent<MeshFilter>().mesh = _currentMesh;
        GetComponentsInChildren<MeshFilter>()[1].mesh = _invertedMesh;
    }
    private int[] ComputeIndices()
    {
        var result = new int[Tetrahedrons.GetLength(0)];

        for (int i = 0; i < Tetrahedrons.GetLength(0); i++)
        {
            result[i] = ComputeIndex(
                _corners[Tetrahedrons[i, 0]],
                _corners[Tetrahedrons[i, 1]],
                _corners[Tetrahedrons[i, 2]],
                _corners[Tetrahedrons[i, 3]]
            );
        }

        return result;
    }

    private int ComputeIndex(bool one, bool two, bool four, bool eight)
    {
        var result = 0;
        result += (one) ? 1 : 0;
        result += (two) ? 2 : 0;
        result += (four) ? 4 : 0;
        result += (eight) ? 8 : 0;
        return result;
    }


    private void OnDrawGizmos()
    {
        // Draw meshes
        if (_currentMesh.vertices.Length > 0)
        {
            //Gizmos.color = Color.blue;
            //Gizmos.DrawMesh(_currentMesh, transform.position);
            //Gizmos.color = Color.red;
            //Gizmos.DrawMesh(_invertedMesh, transform.position);
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
        for (int i = 0; i < 8; i++)
        {
            if (_corners[i])
            {
                Gizmos.DrawSphere(transform.position + vectors[i], 0.1f);
            }
        }
    }
    private static int Pow(int m, int n)
    {
        if (n < 0)
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