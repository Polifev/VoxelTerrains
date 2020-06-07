using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using VoxelTerrains;

namespace VoxelTerrains.Tests
{
    public class TestCsvCubeConfigurationsLoader
    {
        [Test]
        public void LoadsRightNumberOfConfigurations()
        {
            var loader = new CsvCubeConfigurationsLoader();
            var cubeConfigurations = loader.LoadFromFile("./Assets/Scripts/Tests/TestData/marchingCubesCases.csv");
            Assert.AreEqual(256, cubeConfigurations.Length);
        }
    }
}
