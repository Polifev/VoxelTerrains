using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Windows;
using VoxelTerrains.Model;
using VoxelTerrains.Renderer;

namespace VoxelTerrains
{
    public class VoxelManager : MonoBehaviour
    {
        [SerializeField]
        private ComputeShader _generatorShader = null;
        [SerializeField]
        private GameObject _voxelRendererPrefab = null;
        [SerializeField]
        private Transform[] _voxelAgents = new Transform[0];
        
        [SerializeField]
        private int _allowedRenderPerFrame = 1;
        [SerializeField]
        private int _agentsSpiralSize = 4;
        [SerializeField]
        private int _agentsSpiralHalfHeight = 2;

        private WorldGenerator _worldGenerator = null;
        private IChunkRepository _chunkRepository = null;
        private ChunkProvider _world = null;
        private Dictionary<Vector3Int, GameObject> _renderers = new Dictionary<Vector3Int, GameObject>();
        private Coroutine[] _agentRenderingCoroutines = null;
        private Vector3Int[] _agentChunkIndices = null;

        private void Start()
        {
            _worldGenerator = new WorldGenerator();
            _worldGenerator.GeneratorShader = _generatorShader;
            
            _chunkRepository = new FileSystemChunkRepository(Path.Combine(UnityEngine.Windows.Directory.localFolder, "save", "chunks"));
            AutoSaver.Instance.ChunkRepository = _chunkRepository;
            
            _world = new ChunkProvider(_worldGenerator, _chunkRepository);

            // Start a coroutine that will show progressively the terrain to each agent
            _agentRenderingCoroutines = new Coroutine[_voxelAgents.Length];
            _agentChunkIndices = new Vector3Int[_voxelAgents.Length];
            for (int i = 0; i < _voxelAgents.Length; i++)
            {
                var agent = _voxelAgents[i];
                var chunkIndex = Util.GetChunkIndex(agent.position, Vector3Int.one * Chunk.SIZE);
                _agentChunkIndices[i] = chunkIndex;
                _agentRenderingCoroutines[i] = StartCoroutine(SpawnRenderersCoroutine(chunkIndex));
            }

            // Starts the autosaver
            AutoSaver.Instance.Start();
        }

        private void Update()
        {
            for(int i = 0; i < _voxelAgents.Length; i++)
            {
                var agent = _voxelAgents[i];
                var chunkIndex = Util.GetChunkIndex(agent.position, Vector3Int.one * Chunk.SIZE);
                if(chunkIndex != _agentChunkIndices[i])
                {
                    _agentChunkIndices[i] = chunkIndex;
                    StopCoroutine(_agentRenderingCoroutines[i]);
                    _agentRenderingCoroutines[i] = StartCoroutine(SpawnRenderersCoroutine(chunkIndex));
                }
            }
        }

        private void OnDestroy()
        {
            AutoSaver.Instance.Stop();
            foreach (var r in _renderers.Values)
            {
                r.GetComponent<ChunkRenderer>().Release();
            }
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        private IEnumerator SpawnRenderersCoroutine(Vector3Int chunkIndex)
        {
            int notRenderingCount = 0;
            int renderingCount = 0;
            while (true)
            {
                foreach (var spiral in Util.GetSquaredSpiral(_agentsSpiralSize))
                {
                    for (int i = -_agentsSpiralHalfHeight; i < _agentsSpiralHalfHeight; i++)
                    {
                        // TPACPC: c'est normal d'ajouter y à l'axe Z (la spirale se génère en 2d)
                        var temp = new Vector3Int(
                            chunkIndex.x + spiral.x,
                            chunkIndex.y + i,
                            chunkIndex.z + spiral.y);

                        if (!_renderers.ContainsKey(temp))
                        {
                            renderingCount++;
                            InstantiateAndRender(temp);
                        }
                        else
                        {
                            notRenderingCount++;
                        }

                        if (renderingCount >= _allowedRenderPerFrame / _voxelAgents.Length || notRenderingCount >= _allowedRenderPerFrame * 60 / _voxelAgents.Length)
                        {
                            renderingCount = 0;
                            notRenderingCount = 0;
                            yield return new WaitForEndOfFrame();
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
            // TODO set scale

            _renderers.Add(index, instance);
            return renderer;
        }
    }
}

