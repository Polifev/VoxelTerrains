using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VoxelTerrains.Renderer
{
    public class MarchingTetrahedronsRenderer : VoxelRenderer
    {

        private static int[,] Tetrahedrons = new int[,]{
            { 0, 1, 2, 4 },
            { 0, 3, 2, 4 },
            { 3, 7, 4, 2 },
            { 6, 7, 4, 2 },
            { 5, 6, 2, 4 },
            { 5, 1, 2, 4 }
        };

        public override void RefreshMesh()
        {
            base.RefreshMesh();

            IList<Vector3> vertices = new List<Vector3>();
            IList<int> triangles = new List<int>();
            IDictionary<Vector3, int> alreadyPresentVertices = new Dictionary<Vector3, int>();

            for (float x = (-Size / 2).x; x < (Size / 2).x; x += TileSize)
                for (float y = (-Size / 2).y; y < (Size / 2).y; y += TileSize)
                    for (float z = (-Size / 2).z; z < (Size / 2).z; z += TileSize)
                    {
                        int[] configurations = ComputeIndices(x + transform.position.x, y + transform.position.y, z + transform.position.z);
                        
                        for(int tetraIndex = 0; tetraIndex < Tetrahedrons.GetLength(0); tetraIndex++)
                        {
                            var caseIndex = configurations[tetraIndex];
                            var configuration = MeshConfigurations.Configurations[tetraIndex * 16 + caseIndex];
                            
                            for (int i = 0; i < configuration.Vertices.Length; i++)
                            {
                                var vertice = new Vector3(x, y, z) + (configuration.Vertices[i]) * TileSize;
                                if (!alreadyPresentVertices.ContainsKey(vertice))
                                {
                                    var index = vertices.Count;
                                    vertices.Add(vertice);
                                    alreadyPresentVertices.Add(vertice, index);
                                }
                            }
                            for (int i = 0; i < configuration.Triangles.Length; i++)
                            {
                                var correspondingVertice = new Vector3(x, y, z) + configuration.Vertices[configuration.Triangles[i]] * TileSize;
                                triangles.Add(alreadyPresentVertices[correspondingVertice]);
                            }
                        }
                    }

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            MeshFilter.sharedMesh = mesh;
            MeshCollider.sharedMesh = mesh;
        }

        private int[] ComputeIndices(float x, float y, float z)
        {
            var result = new int[Tetrahedrons.GetLength(0)];
            var cornerValues = new float[8];
            cornerValues[0] = ScalarField.ValueAt(new Vector3(x, y, z));
            cornerValues[1] = ScalarField.ValueAt(new Vector3(x + TileSize, y, z));
            cornerValues[2] = ScalarField.ValueAt(new Vector3(x + TileSize, y, z + TileSize));
            cornerValues[3] = ScalarField.ValueAt(new Vector3(x, y, z + TileSize));
            cornerValues[4] = ScalarField.ValueAt(new Vector3(x, y + TileSize, z));
            cornerValues[5] = ScalarField.ValueAt(new Vector3(x + TileSize, y + TileSize, z));
            cornerValues[6] = ScalarField.ValueAt(new Vector3(x + TileSize, y + TileSize, z + TileSize));
            cornerValues[7] = ScalarField.ValueAt(new Vector3(x, y + TileSize, z + TileSize));

            for(int i = 0; i < Tetrahedrons.GetLength(0); i++)
            {
                result[i] = ComputeIndex(
                    cornerValues[Tetrahedrons[i, 0]] > 0,
                    cornerValues[Tetrahedrons[i, 1]] > 0,
                    cornerValues[Tetrahedrons[i, 2]] > 0,
                    cornerValues[Tetrahedrons[i, 3]] > 0
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
    }
}
