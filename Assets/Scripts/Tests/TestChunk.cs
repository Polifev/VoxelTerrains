using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VoxelTerrains;

namespace VoxelTerrains.Tests
{
    public class TestChunk
    {
        [Test]
        public void StartWithAllChunkNodesAtMinus1()
        {
            var chunk = new Chunk(new Vector3Int(16, 24, 32));
            for (int x = 0; x < 16; x++)
                for (int y = 0; y < 24; y++)
                    for (int z = 0; z < 32; z++)
                        Assert.AreEqual(-1.0f, chunk.ValueAt(new Vector3Int(x, y, z)));
        }

        [Test]
        public void KnowsItsDimensions()
        {
            var chunk = new Chunk(new Vector3Int(16, 24, 32));
            Assert.AreEqual(new Vector3Int(16, 24, 32), chunk.Dimensions);
        }

        [Test]
        public void CanSetValueWithExactCoordinates()
        {
            var point = new Vector3Int(0, 0, 0);
            var chunk = new Chunk(new Vector3Int(2, 2, 2));

            chunk.SetValueAt(point, 1.0f);
            Assert.AreEqual(1.0f, chunk.ValueAt(point));
        }
    }
}
