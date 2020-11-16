using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace VoxelTerrains.Model
{
    public class AutoSaver
    {
        private static AutoSaver _instance = new AutoSaver();
        public static AutoSaver Instance { get => _instance; }

        public IChunkRepository ChunkRepository { get; set; }
        public ConcurrentQueue<LocatedChunk> ChunksToSave { get; private set; } = new ConcurrentQueue<LocatedChunk>();
        private Thread _thread;

        public void Start()
        {
            if(ChunkRepository == null)
            {
                throw new NullReferenceException("ChunkRepository cannot be null");
            }
            _thread = new Thread(Run);
            _thread.Start();
        }

        public void Stop()
        {
            _thread.Interrupt();
        }

        private void Run()
        {
            while (true)
            {
                try
                {
                    var empty = !ChunksToSave.TryDequeue(out var c);
                    if (!empty)
                    {
                        ChunkRepository.SaveChunk(c.ChunkIndex, c.Chunk);
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
                catch (ThreadInterruptedException e)
                {
                    Debug.Log("Autosaver stopped");
                    break;
                }
            }
        }
    }
}
