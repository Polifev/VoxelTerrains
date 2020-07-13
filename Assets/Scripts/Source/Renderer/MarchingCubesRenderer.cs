using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoxelTerrains.ScriptableObjects;

namespace VoxelTerrains.Renderer
{
    public class MarchingCubesRenderer : VoxelRenderer
    {
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
                        int configurationIndex = ComputeIndex(x + transform.position.x, y + transform.position.y, z + transform.position.z);
                        var configuration = MeshConfigurations.Configurations[configurationIndex];
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

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            MeshFilter.sharedMesh = mesh;
            MeshCollider.sharedMesh = mesh;
        }

        private int ComputeIndex(float x, float y, float z)
        {
            int index = 0;
            if (ScalarField.ValueAt(new Vector3(x, y, z)) > 0f)
            {
                index += 1;
            }
            if (ScalarField.ValueAt(new Vector3(x + TileSize, y, z)) > 0f)
            {
                index += 2;
            }
            if (ScalarField.ValueAt(new Vector3(x + TileSize, y, z + TileSize)) > 0f)
            {
                index += 4;
            }
            if (ScalarField.ValueAt(new Vector3(x, y, z + TileSize)) > 0f)
            {
                index += 8;
            }
            if (ScalarField.ValueAt(new Vector3(x, y + TileSize, z)) > 0f)
            {
                index += 16;
            }
            if (ScalarField.ValueAt(new Vector3(x + TileSize, y + TileSize, z)) > 0f)
            {
                index += 32;
            }
            if (ScalarField.ValueAt(new Vector3(x + TileSize, y + TileSize, z + TileSize)) > 0f)
            {
                index += 64;
            }
            if (ScalarField.ValueAt(new Vector3(x, y + TileSize, z + TileSize)) > 0f)
            {
                index += 128;
            }
            return index;
        }
    }
}

