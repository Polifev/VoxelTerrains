using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VoxelTerrains.ScalarField;

namespace VoxelTerrains.Tests.ScriptableObjects
{
    public class TestHorizontalScalarField
    {
        [Test]
        public void NegativeOneAboveHeight()
        {
            var scalarField = new HorizontalScalarField(15.0f);
            Assert.Less(scalarField.ValueAt(new Vector3(0.0f, 22.0f, 0.0f)), 0.0f);
        }

        [Test]
        public void PositiveOneBelowHeight()
        {
            var scalarField = new HorizontalScalarField(15.0f);
            Assert.Greater(scalarField.ValueAt(new Vector3(0.0f, 12.0f, 0.0f)), 0.0f);
        }

        [Test]
        public void PositiveOneAtHeight()
        {
            var scalarField = new HorizontalScalarField(15.0f);
            Assert.Greater(scalarField.ValueAt(new Vector3(0.0f, 12.0f, 0.0f)), 0.0f);
        }
    }
}
