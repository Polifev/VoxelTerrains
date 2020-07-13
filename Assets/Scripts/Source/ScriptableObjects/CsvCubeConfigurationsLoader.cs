using System;
using System.IO;
using UnityEngine;

namespace VoxelTerrains.ScriptableObjects
{
    public class CsvCubeConfigurationsLoader
    {
        public MeshConfiguration[] LoadFromFile(string path, int numberOfCases)
        {
            var result = new MeshConfiguration[numberOfCases];

            using (var reader = new StreamReader(File.OpenRead(path)))
            {
                for(int i = 0; i < result.Length; i++)
                {
                    try
                    {
                        MeshConfiguration config = new MeshConfiguration();
                        var line = reader.ReadLine();
                        var parts = line.Split(';');
                        if(parts.Length == 2)
                        {
                            var vectorStrings = parts[0].Split(',');
                            var triangleStrings = parts[1].Split(',');

                            config.Vertices = new Vector3[vectorStrings.Length];
                            config.Triangles = new int[triangleStrings.Length];

                            for(int j = 0; j < config.Vertices.Length; j++)
                            {
                                config.Vertices[j] = ReadVector(vectorStrings[j]);
                            }

                            for (int j = 0; j < config.Triangles.Length; j++)
                            {
                                config.Triangles[j] = int.Parse(triangleStrings[j]);
                            }
                        }
                        else
                        {
                            config.Vertices = new Vector3[0];
                            config.Triangles = new int[0];
                        }
                        result[i] = config;
                    } 
                    catch(EndOfStreamException)
                    {
                        throw new Exception("The file does not contain all the cases");
                    }
                }
            }
            return result;
        }

        private Vector3 ReadVector(string vectorString)
        {
            vectorString = vectorString.Replace("(", "");
            vectorString = vectorString.Replace(")", "");
            vectorString = vectorString.Replace(".", ",");
            var coordinateStrings = vectorString.Split(':');

            return new Vector3(
                float.Parse(coordinateStrings[0]),
                float.Parse(coordinateStrings[1]),
                float.Parse(coordinateStrings[2])
            );
        }
    }
}
