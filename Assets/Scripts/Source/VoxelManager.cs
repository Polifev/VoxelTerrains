using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
        [SerializeField]
        private Transform[] _voxelAgents;
        [SerializeField]
        private int _allowedRenderPerFrame = 1;


        private Dictionary<Vector3Int, GameObject> _renderers = new Dictionary<Vector3Int, GameObject>();
        private Coroutine[] _agentRenderingCoroutines;
        private Vector3Int[] _agentChunkIndices;

        private void Start()
        {
            
            _terrain.OnTerrainChanged += RenderChunksAround;

            // Start a coroutine that will show progressively the terrain to each agent
            _agentRenderingCoroutines = new Coroutine[_voxelAgents.Length];
            _agentChunkIndices = new Vector3Int[_voxelAgents.Length];
            for (int i = 0; i < _voxelAgents.Length; i++)
            {
                var agent = _voxelAgents[i];
                var chunkIndex = GetChunkIndex(agent.position, Vector3Int.one * _rendererSize);
                _agentChunkIndices[i] = chunkIndex;
                _agentRenderingCoroutines[i] = StartCoroutine(SpawnRenderersCoroutine(chunkIndex));
            }

            // Start a thread that will generate chunk data around player position
        }

        private void Update()
        {
            for(int i = 0; i < _voxelAgents.Length; i++)
            {
                var agent = _voxelAgents[i];
                var chunkIndex = GetChunkIndex(agent.position, Vector3Int.one * _rendererSize);
                if(chunkIndex != _agentChunkIndices[i])
                {
                    _agentChunkIndices[i] = chunkIndex;
                    StopCoroutine(_agentRenderingCoroutines[i]);
                    _agentRenderingCoroutines[i] = StartCoroutine(SpawnRenderersCoroutine(chunkIndex));
                }
                
            }
        }

        private IEnumerator SpawnRenderersCoroutine(Vector3Int chunkIndex)
        {
            int count = 0;
            while (true)
            {
                foreach (var spiral in GetSquaredSpiral(int.MaxValue))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            // TPACPC: c'est normal d'ajouter y à l'axe Z (la spirale se génère en 2d)
                            var temp = new Vector3Int(
                                chunkIndex.x + spiral.x,
                                chunkIndex.y + ((j == 0) ? i : -i),
                                chunkIndex.z + spiral.y);

                            if (!_renderers.ContainsKey(temp))
                            {
                                count++;
                                InstantiateAndRender(temp);
                            }

                            if (count >= _allowedRenderPerFrame / _voxelAgents.Length)
                            {
                                count = 0;
                                yield return new WaitForEndOfFrame();
                            }
                        }
                    }
                }
            }
        }

        private static Vector3Int GetChunkIndex(Vector3 position, Vector3Int chunkSize)
        {
            int x = Mathf.FloorToInt(position.x / chunkSize.x);
            int y = Mathf.FloorToInt(position.y / chunkSize.y);
            int z = Mathf.FloorToInt(position.z / chunkSize.z);
            return new Vector3Int(x, y, z);
        }

        private static IEnumerable<Vector2Int> GetSquaredSpiral(int total)
        {
            var lastPosition = Vector2Int.zero;
            var currentOrientation = 0;
            var orientations = new Vector2Int[]
            {
                        new Vector2Int(0, 1),
                        new Vector2Int(1, 0),
                        new Vector2Int(0, -1),
                        new Vector2Int(-1, 0)
            };

            for (int i = 0; i < total; i++)
            {
                for (int twice = 0; twice < 2; twice++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        yield return lastPosition;
                        lastPosition = lastPosition + orientations[currentOrientation];
                    }
                    currentOrientation = (currentOrientation + 1) % 4;
                }
            }
        }

        private void RenderChunksAround(Vector3 location)
        {
            Vector3Int centerIndex = GetChunkIndex(location, Vector3Int.one * _rendererSize);

            IList<Vector3Int> updateIndexes = new List<Vector3Int>();
            updateIndexes.Add(centerIndex);

            // faces
            updateIndexes.Add(centerIndex + new Vector3Int(1, 0, 0));
            updateIndexes.Add(centerIndex + new Vector3Int(-1, 0, 0));
            updateIndexes.Add(centerIndex + new Vector3Int(0, 1, 0));
            updateIndexes.Add(centerIndex + new Vector3Int(0, -1, 0));
            updateIndexes.Add(centerIndex + new Vector3Int(0, 0, 1));
            updateIndexes.Add(centerIndex + new Vector3Int(0, 0, -1));

            // edges
            updateIndexes.Add(centerIndex + new Vector3Int(1, 1, 0));
            updateIndexes.Add(centerIndex + new Vector3Int(1, -1, 0));
            updateIndexes.Add(centerIndex + new Vector3Int(-1, 1, 0));
            updateIndexes.Add(centerIndex + new Vector3Int(-1, -1, 0));
            updateIndexes.Add(centerIndex + new Vector3Int(1, 0, 1));
            updateIndexes.Add(centerIndex + new Vector3Int(1, 0, -1));
            updateIndexes.Add(centerIndex + new Vector3Int(-1, 0, 1));
            updateIndexes.Add(centerIndex + new Vector3Int(-1, 0, -1));
            updateIndexes.Add(centerIndex + new Vector3Int(0, 1, 1));
            updateIndexes.Add(centerIndex + new Vector3Int(0, 1, -1));
            updateIndexes.Add(centerIndex + new Vector3Int(0, -1, 1));
            updateIndexes.Add(centerIndex + new Vector3Int(0, -1, -1));

            // vertices
            updateIndexes.Add(centerIndex + new Vector3Int(1, 1, 1));
            updateIndexes.Add(centerIndex + new Vector3Int(1, 1, -1));
            updateIndexes.Add(centerIndex + new Vector3Int(1, -1, 1));
            updateIndexes.Add(centerIndex + new Vector3Int(1, -1, -1));
            updateIndexes.Add(centerIndex + new Vector3Int(-1, 1, 1));
            updateIndexes.Add(centerIndex + new Vector3Int(-1, 1, -1));
            updateIndexes.Add(centerIndex + new Vector3Int(-1, -1, 1));
            updateIndexes.Add(centerIndex + new Vector3Int(-1, -1, -1));


            foreach (Vector3Int index in updateIndexes)
            {
                InstantiateAndRender(index);
            }

        }

        private void InstantiateAndRender(Vector3Int rendererIndex)
        {
            if (!_renderers.ContainsKey(rendererIndex))
            {
                InstantiateRenderer(rendererIndex);
            }

            var instance = _renderers[rendererIndex];
            var renderer = instance.GetComponent<VoxelRenderer>();

            renderer.RefreshMesh();
            if (renderer.isEmpty())
            {
                instance.SetActive(false);
            }
        }

        private VoxelRenderer InstantiateRenderer(Vector3Int index)
        {
            var instance = Instantiate(_voxelRendererPrefab, this.transform);
            instance.transform.Translate(index * _rendererSize);

            var renderer = instance.GetComponent<VoxelRenderer>();
            renderer.ScalarField = _terrain;
            renderer.Size = Vector3.one * _rendererSize;

            _renderers.Add(index, instance);
            return renderer;
        }
    }
}

