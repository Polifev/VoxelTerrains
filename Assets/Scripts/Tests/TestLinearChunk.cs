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
    public class TestLinearChunk
    {
        [Test]
        public void StartWithAllChunkNodesAtMinus1()
        {
            var chunk = new LinearChunk(new Vector3Int(16, 24, 32));
            for (int x = 0; x < 16; x++)
                for (int y = 0; y < 24; y++)
                    for (int z = 0; z < 32; z++)
                        Assert.AreEqual(-1.0f, chunk.ValueAt(new Vector3(x, y, z)));
        }

        [Test]
        public void KnowsItsDimensions()
        {
            var chunk = new LinearChunk(new Vector3Int(16, 24, 32));
            Assert.AreEqual(new Vector3(16, 24, 32), chunk.Dimensions);
        }

        [Test]
        public void CanSetValueWithExactCoordinates()
        {
            var point = new Vector3(0.0f, 0.0f, 0.0f);
            var chunk = new LinearChunk(new Vector3Int(2, 2, 2));

            chunk.SetNearestValueAt(point, 1.0f);
            Assert.AreEqual(1.0f, chunk.ValueAt(point));
        }

        [Test]
        public void CanSetValueWithInexactCoordinates()
        {
            var point = new Vector3(0.1f, 0.0f, 0.0f);
            var chunk = new LinearChunk(new Vector3Int(2, 2, 2));

            chunk.SetNearestValueAt(point, 1.0f);
            Assert.AreEqual(1.0f, chunk.ValueAt(new Vector3(0f, 0f, 0f)));
        }

        [Test]
        public void InterpolateCorrectly()
        {
            var chunk = new LinearChunk(new Vector3Int(2, 2, 2));
            chunk.SetNearestValueAt(new Vector3(0f, 0f, 0f), 1.0f);
            Assert.AreEqual(-0.75f, chunk.ValueAt(new Vector3(0.5f, 0.5f, 0.5f)));
        }
    }
}
