using UnityEngine;
using VoxelTerrains.Model;

namespace VoxelTerrains.Renderer
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class ChunkRenderer : MonoBehaviour
    {
        [SerializeField]
        private ComputeShader _renderingShader = null;
        private MeshFilter _meshFilter = null;
        private MeshCollider _meshCollider = null;

        public bool Empty { get; private set; } = false;
        public ChunkProvider World { get; set; } = null;
        public float Scale { get; set; } = 1.0f;
        
        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector3.one * (Chunk.SIZE / 2) + transform.position, Vector3.one * Chunk.SIZE);
        }

        public void RefreshMesh()
        {
            Vector3Int chunkIndex = Util.GetChunkIndex(transform.position, Vector3Int.one * Chunk.SIZE);

            int cornersCount = Chunk.SIZE * Chunk.SIZE * Chunk.SIZE;
            int maxTrianglesCount = (Chunk.SIZE - 1) * (Chunk.SIZE - 1) * (Chunk.SIZE - 1) * 5;

            ComputeBuffer cornersBuffer = new ComputeBuffer(cornersCount, sizeof(float));
            ComputeBuffer trianglesBuffer = new ComputeBuffer(
                maxTrianglesCount,
                sizeof(float) * 3 * 3, // 3 float3 per triangles, 5 triangles per voxel (max)
                ComputeBufferType.Append);
            trianglesBuffer.SetCounterValue(0);

            ComputeBuffer trianglesCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);

            World.FillBuffer(chunkIndex, cornersBuffer);
            
            _renderingShader.SetBuffer(0, "corners", cornersBuffer);
            _renderingShader.SetBuffer(0, "triangles", trianglesBuffer);
            _renderingShader.SetFloat("scale", Scale);

            _renderingShader.Dispatch(0, 8, 8, 8);

            ComputeBuffer.CopyCount(trianglesBuffer, trianglesCountBuffer, 0);
            int[] trianglesCountArray = new int[1];
            trianglesCountBuffer.GetData(trianglesCountArray);
            int trianglesCount = trianglesCountArray[0];

            Triangle[] triangles = new Triangle[trianglesCount];
            trianglesBuffer.GetData(triangles, 0, 0, trianglesCount);

            Vector3[] meshVertices = new Vector3[trianglesCount * 3];
            int[] meshTriangles = new int[trianglesCount * 3];

            for (int i = 0; i < trianglesCount; i++)
            {
                var triple = i * 3;
                meshVertices[triple] = triangles[i].a;
                meshVertices[triple + 1] = triangles[i].b;
                meshVertices[triple + 2] = triangles[i].c;

                meshTriangles[triple] = triple;
                meshTriangles[triple + 1] = triple + 1;
                meshTriangles[triple + 2] = triple + 2;
            }

            Mesh mesh = new Mesh();
            mesh.name = "Chunk " + chunkIndex.ToString();

            mesh.vertices = meshVertices;
            mesh.triangles = meshTriangles;
            mesh.RecalculateNormals();

            _meshFilter.mesh = mesh;
            _meshCollider.sharedMesh = mesh;

            trianglesBuffer.Release();
            trianglesCountBuffer.Release();
            cornersBuffer.Release();

            Empty = meshVertices.Length <= 0;
        }
    }
}
