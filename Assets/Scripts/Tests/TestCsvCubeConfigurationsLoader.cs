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
            var cubeConfigurations = loader.LoadFromFile("./Assets/Scripts/Tests/TestData/TestImport_marchingCubesCases.csv");
            Assert.AreEqual(256, cubeConfigurations.Length);
        }

        [Test]
        public void LoadsEmptyConfigurationOnEmptyLines()
        {
            var loader = new CsvCubeConfigurationsLoader();
            var cubeConfigurations = loader.LoadFromFile("./Assets/Scripts/Tests/TestData/TestImport_marchingCubesCases.csv");
            Assert.AreEqual(0, cubeConfigurations[0].Vertices.Length);
            Assert.AreEqual(0, cubeConfigurations[0].Triangles.Length);
        }

        [Test]
        public void LoadsCorrectlyVerticesData()
        {
            // (0:0:0.5),(0:0.5:0),(0.5:1:0),(0.5:1:1),(0.5:0:1); 1,0,4,2,1,4,3,2,4
            var loader = new CsvCubeConfigurationsLoader();
            var cubeConfigurations = loader.LoadFromFile("./Assets/Scripts/Tests/TestData/TestImport_marchingCubesCases.csv");
            Assert.AreEqual(5, cubeConfigurations[14].Vertices.Length);

            Assert.AreEqual(new Vector3(0f, 0f, 0.5f), cubeConfigurations[14].Vertices[0]);
            Assert.AreEqual(new Vector3(0f, 0.5f, 0f), cubeConfigurations[14].Vertices[1]);
            Assert.AreEqual(new Vector3(0.5f, 1f, 0f), cubeConfigurations[14].Vertices[2]);
            Assert.AreEqual(new Vector3(0.5f, 1f, 1f), cubeConfigurations[14].Vertices[3]);
            Assert.AreEqual(new Vector3(0.5f, 0f, 1f), cubeConfigurations[14].Vertices[4]);
        }

        [Test]
        public void LoadsCorrectlyTrianglesData()
        {
            // (0:0:0.5),(0:0.5:0),(0.5:1:0),(0.5:1:1),(0.5:0:1); 1,0,4,2,1,4,3,2,4
            var loader = new CsvCubeConfigurationsLoader();
            var cubeConfigurations = loader.LoadFromFile("./Assets/Scripts/Tests/TestData/TestImport_marchingCubesCases.csv");
            Assert.AreEqual(9, cubeConfigurations[14].Triangles.Length);

            Assert.AreEqual(1, cubeConfigurations[14].Triangles[0]);
            Assert.AreEqual(0, cubeConfigurations[14].Triangles[1]);
            Assert.AreEqual(4, cubeConfigurations[14].Triangles[2]);
            Assert.AreEqual(2, cubeConfigurations[14].Triangles[3]);
            Assert.AreEqual(1, cubeConfigurations[14].Triangles[4]);
            Assert.AreEqual(4, cubeConfigurations[14].Triangles[5]);
            Assert.AreEqual(3, cubeConfigurations[14].Triangles[6]);
            Assert.AreEqual(2, cubeConfigurations[14].Triangles[7]);
            Assert.AreEqual(4, cubeConfigurations[14].Triangles[8]);
        }
    }
}
