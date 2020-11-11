using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VoxelTerrains.Model
{
    public class AutoSaver
    {
        private static AutoSaver _instance;
        public static AutoSaver Instance { get => _instance ??= new AutoSaver(); }

        public ConcurrentBag<Chunk> Issou;
        public ConcurrentQueue<Chunk> ChunksToSave { get; private set; }
        private Thread _thread;

        public void Start()
        {
            _thread = new Thread(Run);
            _thread.Start();
        }

        public void Stop()
        {
            ChunksToSave.Enqueue(null);
        }

        private void Run()
        {
            Chunk c;
            do
            {
                bool empty = !ChunksToSave.TryDequeue(out c);
                if (empty)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    // TODO store chunk in repository
                }
            } while (c != null);
        }
    }
}
