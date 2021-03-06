﻿using System;
using System.IO;
using UnityEngine;

namespace VoxelTerrains
{
    public class CsvCubeConfigurationsLoader
    {
        public CubeConfiguration[] LoadFromFile(string path)
        {
            var result = new CubeConfiguration[256];

            using (var reader = new StreamReader(File.OpenRead(path)))
            {
                for(int i = 0; i < result.Length; i++)
                {
                    try
                    {
                        CubeConfiguration config = new CubeConfiguration();
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
                                config.Triangles[j] = int.Parse(vectorStrings[j]);
                            }
                        }
                        else
                        {
                            config.Vertices = new Vector3[0];
                            config.Triangles = new int[0];
                        }
                    } 
                    catch(EndOfStreamException e)
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
            var coordinateStrings = vectorString.Split(':');

            return new Vector3(
                float.Parse(coordinateStrings[0]),
                float.Parse(coordinateStrings[1]),
                float.Parse(coordinateStrings[2])
            );
        }
    }
}
