using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using VoxelTerrains.Model;
using VoxelTerrains.Renderer;
using VoxelTerrains.ScalarField;

namespace VoxelTerrains
{
    public class VoxelManager : MonoBehaviour
    {
        [SerializeField]
        private ComputeShader _generatorShader = null;
        [SerializeField]
        private ComputeShader _renderingShader = null;
        [SerializeField]
        private GameObject _voxelRendererPrefab = null;
        [SerializeField]
        private Transform[] _voxelAgents = new Transform[0];
        [SerializeField]
        private int _allowedRenderPerFrame = 1;

        private WorldGenerator _worldGenerator = null;
        private ChunkProvider _world = null;
        private Dictionary<Vector3Int, GameObject> _renderers = new Dictionary<Vector3Int, GameObject>();
        private Coroutine[] _agentRenderingCoroutines = new Coroutine[0];
        private Vector3Int[] _agentChunkIndices = new Vector3Int[0];

        private void Start()
        {
            _worldGenerator = new WorldGenerator();
            _worldGenerator.GeneratorShader = _generatorShader;
            _world = new ChunkProvider(_worldGenerator);

            // Start a coroutine that will show progressively the terrain to each agent
            /*_agentRenderingCoroutines = new Coroutine[_voxelAgents.Length];
            _agentChunkIndices = new Vector3Int[_voxelAgents.Length];
            for (int i = 0; i < _voxelAgents.Length; i++)
            {
                var agent = _voxelAgents[i];
                var chunkIndex = Util.GetChunkIndex(agent.position, Vector3Int.one * Chunk.SIZE);
                _agentChunkIndices[i] = chunkIndex;
                _agentRenderingCoroutines[i] = StartCoroutine(SpawnRenderersCoroutine(chunkIndex));
            }*/
            var chunkIndex = Util.GetChunkIndex(_voxelAgents[0].position, Vector3Int.one * 64);
            foreach (var spiral in Util.GetSquaredSpiral(4))
            {
                for (int i = 0; i < 2; i++)
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
                            InstantiateAndRender(temp);
                        }
                    }
                }
            }
        }

        private void Update()
        {
            /*for(int i = 0; i < _voxelAgents.Length; i++)
            {
                var agent = _voxelAgents[i];
                var chunkIndex = Util.GetChunkIndex(agent.position, Vector3Int.one * Chunk.SIZE);
                if(chunkIndex != _agentChunkIndices[i])
                {
                    _agentChunkIndices[i] = chunkIndex;
                    StopCoroutine(_agentRenderingCoroutines[i]);
                    _agentRenderingCoroutines[i] = StartCoroutine(SpawnRenderersCoroutine(chunkIndex));
                }
            }*/
        }

        private IEnumerator SpawnRenderersCoroutine(Vector3Int chunkIndex)
        {
            int count = 0;
            while (true)
            {
                foreach (var spiral in Util.GetSquaredSpiral(int.MaxValue))
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

        private void RenderChunksAround(Vector3 location)
        {
            Vector3Int centerIndex = Util.GetChunkIndex(location, Vector3Int.one * Chunk.SIZE);

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
            var renderer = instance.GetComponent<ChunkRenderer>();

            renderer.RefreshMesh();
            instance.SetActive(!renderer.Empty);
        }

        private ChunkRenderer InstantiateRenderer(Vector3Int index)
        {
            var instance = Instantiate(_voxelRendererPrefab, this.transform);
            instance.transform.Translate(index * Chunk.SIZE);

            var renderer = instance.GetComponent<ChunkRenderer>();
            renderer.World = _world;
            renderer.RenderingShader = _renderingShader;
            // TODO set scale

            _renderers.Add(index, instance);
            return renderer;
        }
    }
}

