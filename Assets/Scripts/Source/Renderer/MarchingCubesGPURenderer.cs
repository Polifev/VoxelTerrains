using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace VoxelTerrains.Renderer
{
    // notice me
    struct Triangle
    {
#pragma warning disable 649
        public Vector3 one;
        public Vector3 two;
        public Vector3 three;
    }

    public class MarchingCubesGPURenderer : VoxelRenderer
    {
        [SerializeField]
        private ComputeShader _marchingCubesShader = null;

        private ComputeBuffer _cornersBuffer;
        private ComputeBuffer _trianglesBuffer;
        private ComputeBuffer _trianglesCountBuffer;

        public override void RefreshMesh()
        {
            base.RefreshMesh();
            CreateBuffers();

            var numberOfCornersPerAxis = Vector3Int.FloorToInt(Size / TileSize) + Vector3Int.one;
            int numberOfCorners = numberOfCornersPerAxis.x * numberOfCornersPerAxis.y * numberOfCornersPerAxis.z;
            FillCornersBuffer(_cornersBuffer, numberOfCorners);
            
            _trianglesBuffer.SetCounterValue(0);
            
            _marchingCubesShader.SetBuffer(0, "triangles", _trianglesBuffer);
            _marchingCubesShader.SetBuffer(0, "corners", _cornersBuffer);
            
            _marchingCubesShader.SetInt("numberCornersX", numberOfCornersPerAxis.x);
            _marchingCubesShader.SetInt("numberCornersY", numberOfCornersPerAxis.y);
            _marchingCubesShader.SetInt("numberCornersZ", numberOfCornersPerAxis.z);

            _marchingCubesShader.SetFloat("tileSize", TileSize);
            _marchingCubesShader.SetVector("rendererSize", Size);
            _marchingCubesShader.SetVector("offset", transform.position);

            _marchingCubesShader.Dispatch(0, 2, 2, 2);

            ComputeBuffer.CopyCount(_trianglesBuffer, _trianglesCountBuffer, 0);
            int[] triCountArr = new int[1];
            _trianglesCountBuffer.GetData(triCountArr);
            int triCount = triCountArr[0];

            Triangle[] triangles = new Triangle[triCount];
            _trianglesBuffer.GetData(triangles, 0, 0, triCount);

            Mesh mesh = new Mesh();
            Vector3[] meshVertices = new Vector3[triCount * 3];
            int[] meshTriangles = new int[triCount * 3];

            for (int i = 0; i < triCount; i++)
            {
                var triple = i * 3;
                meshVertices[triple] = triangles[i].one;
                meshVertices[triple + 1] = triangles[i].two;
                meshVertices[triple + 2] = triangles[i].three;

                meshTriangles[triple] = triple;
                meshTriangles[triple + 1] = triple + 1;
                meshTriangles[triple + 2] = triple + 2;
            }
            mesh.vertices = meshVertices;
            mesh.triangles = meshTriangles;
            mesh.RecalculateNormals();
            
            MeshFilter.sharedMesh = mesh;
            MeshCollider.sharedMesh = mesh;

            ReleaseBuffers();
        }

        private void CreateBuffers()
        {
            ReleaseBuffers();

            var numberOfCornersPerAxis = Vector3Int.FloorToInt(Size / TileSize) + Vector3Int.one;
            int numberOfCorners = numberOfCornersPerAxis.x * numberOfCornersPerAxis.y * numberOfCornersPerAxis.z;

            var numberOfVoxelsPerAxis = numberOfCornersPerAxis - Vector3Int.one;
            int numberOfVoxels = numberOfVoxelsPerAxis.x * numberOfVoxelsPerAxis.y * numberOfVoxelsPerAxis.z;

            int maxNumberOfTriangles = numberOfVoxels * 5;

            _trianglesBuffer = new ComputeBuffer(
                maxNumberOfTriangles,
                sizeof(int) * 3 * 3,
                ComputeBufferType.Append);

            _cornersBuffer = new ComputeBuffer(
                numberOfCorners,
                sizeof(float) * 4
                );

            _trianglesCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
        }

        public void ReleaseBuffers()
        {
            if(_trianglesBuffer != null)
            {
                _trianglesBuffer.Release();
                _cornersBuffer.Release();
                _trianglesCountBuffer.Release();
            }
        }

        public ComputeBuffer FillCornersBuffer(ComputeBuffer cornersBuffer, int numberOfCorners)
        {
            Vector4[] data = new Vector4[numberOfCorners];

            int index = 0;
            for (float x = (-Size / 2).x; x <= (Size / 2).x; x += TileSize)
                for (float y = (-Size / 2).y; y <= (Size / 2).y; y += TileSize)
                    for (float z = (-Size / 2).z; z <= (Size / 2).z; z += TileSize)
                    {
                        float value = ScalarField.ValueAt(new Vector3(x, y, z) + transform.position);
                        data[index] = new Vector4(x,y,z,value);
                        index++;
                    }
            cornersBuffer.SetData(data);
            return cornersBuffer;
        }
    }
}
