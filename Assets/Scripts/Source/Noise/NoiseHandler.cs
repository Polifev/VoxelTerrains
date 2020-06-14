using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelTerrains.Noise
{
    public static class NoiseHandler
    {
        public enum NoiseType
        {
            OpenSimplexNoise
        }

        public enum NoiseAdditionType
        {
            FBM,
            Turbulence,
            Ridge, 
            Multifractal
        }

        public static float Noise(float x, float y, int octave = 6, float lacunarity = 2f, float persistance = 0.5f, float scale = 0.1f, float offset = 0f, float multifractaclA = 2f, int seed = 0, NoiseType noiseType = NoiseType.OpenSimplexNoise, NoiseAdditionType noiseAdditionType = NoiseAdditionType.FBM)
        {
            float noise = 0f;
            float lastOctave = 0f;
            float lastRidge = 0f;
            float lastRidgeAmplitude = 0f;
            float increment = 0f;
            octave = octave <= 0 ? 1 : octave;
            switch (noiseType)
            {
                case NoiseType.OpenSimplexNoise:
                    OpenSimplexNoise simplex = new OpenSimplexNoise(seed);
                    for (int i = 0; i < octave; i++)
                    {
                        float frequency = Mathf.Pow(lacunarity, i);
                        float amplitude = Mathf.Pow(persistance, i);
                        float thisOctave = (float)simplex.eval((x * scale) * frequency, (y * scale) * frequency) * amplitude;
                        switch (noiseAdditionType)
                        {
                            case NoiseAdditionType.FBM:
                                noise = FBM(noise, thisOctave);
                                increment += amplitude;
                                break;
                            case NoiseAdditionType.Turbulence:
                                noise = Turbulence(noise, thisOctave, offset);
                                increment += Mathf.Abs(((amplitude + 1f) * 0.5f) + offset - 0.5f) * 2f;
                                break;
                            case NoiseAdditionType.Ridge:
                                noise = Ridge(noise, thisOctave, offset);
                                increment += Mathf.Pow(0.25f - (Mathf.Abs(((amplitude + 1f) * 0.5f) + offset - 0.5f) * 2f), 2f);
                                break;
                            case NoiseAdditionType.Multifractal:
                                float ridgeAmplitude = Mathf.Pow(0.25f - (Mathf.Abs(((amplitude + 1f) * 0.5f) + offset - 0.5f) * 2f), 2f);
                                increment += ridgeAmplitude * (1f - multifractaclA * lastRidgeAmplitude);
                                lastRidgeAmplitude = ridgeAmplitude;
                                noise = MultiFractal(noise, thisOctave, lastOctave, offset, multifractaclA, ref lastRidge);
                                break;
                            default:
                                noise += thisOctave;
                                break;
                        }
                        lastOctave = thisOctave;
                    }
                    break;
                default:
                    break;
            }
            return noise / increment;
        }

        public static float Noise(float x, float y, float z, int octave = 6, float lacunarity = 2f, float persistance = 0.5f, float scale = 0.1f, float offset = 0f, float multifractaclA = 2f, int seed = 0, NoiseType noiseType = NoiseType.OpenSimplexNoise, NoiseAdditionType noiseAdditionType = NoiseAdditionType.FBM)
        {
            float noise = 0f;
            float lastOctave = 0f;
            float lastRidge = 0f;
            float lastRidgeAmplitude = 0f;
            float increment = 0f;
            octave = octave <= 0 ? 1 : octave;
            switch (noiseType)
            {
                case NoiseType.OpenSimplexNoise:
                    OpenSimplexNoise simplex = new OpenSimplexNoise(seed);
                    for (int i = 0; i < octave; i++)
                    {
                        float frequency = Mathf.Pow(lacunarity, i);
                        float amplitude = Mathf.Pow(persistance, i);
                        float thisOctave = (float)simplex.eval((x * scale) * frequency, (y * scale) * frequency, (z * scale) * frequency) * amplitude;
                        switch (noiseAdditionType)
                        {
                            case NoiseAdditionType.FBM:
                                noise = FBM(noise, thisOctave);
                                increment += amplitude;
                                break;
                            case NoiseAdditionType.Turbulence:
                                noise = Turbulence(noise, thisOctave, offset);
                                increment += Mathf.Abs(((amplitude + 1f) * 0.5f) + offset - 0.5f) * 2f;
                                break;
                            case NoiseAdditionType.Ridge:
                                noise = Ridge(noise, thisOctave, offset);
                                increment += Mathf.Pow(0.25f - (Mathf.Abs(((amplitude + 1f) * 0.5f) + offset - 0.5f) * 2f), 2f);
                                break;
                            case NoiseAdditionType.Multifractal:
                                float ridgeAmplitude = Mathf.Pow(0.25f - (Mathf.Abs(((amplitude + 1f) * 0.5f) + offset - 0.5f) * 2f), 2f);
                                increment += ridgeAmplitude * (1f - multifractaclA * lastRidgeAmplitude);
                                lastRidgeAmplitude = ridgeAmplitude;
                                noise = MultiFractal(noise, thisOctave, lastOctave, offset, multifractaclA, ref lastRidge);
                                break;
                            default:
                                noise += thisOctave;
                                break;
                        }
                        lastOctave = thisOctave;
                    }
                    break;
                default:
                    break;
            }
            return noise / increment;
        }

        public static float Noise(float x, float y, float z, float w, int octave = 6, float lacunarity = 2f, float persistance = 0.5f, float scale = 0.1f, float offset = 0f, float multifractaclA = 2f, int seed = 0, NoiseType noiseType = NoiseType.OpenSimplexNoise, NoiseAdditionType noiseAdditionType = NoiseAdditionType.FBM)
        {
            float noise = 0f;
            float lastOctave = 0f;
            float lastRidge = 0f;
            float lastRidgeAmplitude = 0f;
            float increment = 0f;
            octave = octave <= 0 ? 1 : octave;
            switch (noiseType)
            {
                case NoiseType.OpenSimplexNoise:
                    OpenSimplexNoise simplex = new OpenSimplexNoise(seed);
                    for (int i = 0; i < octave; i++)
                    {
                        float frequency = Mathf.Pow(lacunarity, i);
                        float amplitude = Mathf.Pow(persistance, i);
                        float thisOctave = (float)simplex.eval((x * scale) * frequency, (y * scale) * frequency, (z * scale) * frequency, (w * scale) * frequency) * amplitude;
                        switch (noiseAdditionType)
                        {
                            case NoiseAdditionType.FBM:
                                noise = FBM(noise, thisOctave);
                                increment += amplitude;
                                break;
                            case NoiseAdditionType.Turbulence:
                                noise = Turbulence(noise, thisOctave, offset);
                                increment += Mathf.Abs(((amplitude + 1f) * 0.5f) + offset - 0.5f) * 2f;
                                break;
                            case NoiseAdditionType.Ridge:
                                noise = Ridge(noise, thisOctave, offset);
                                increment += Mathf.Pow(0.25f - (Mathf.Abs(((amplitude + 1f) * 0.5f) + offset - 0.5f) * 2f), 2f);
                                break;
                            case NoiseAdditionType.Multifractal:
                                float ridgeAmplitude = Mathf.Pow(0.25f - (Mathf.Abs(((amplitude + 1f) * 0.5f) + offset - 0.5f) * 2f), 2f);
                                increment += ridgeAmplitude * (1f - multifractaclA * lastRidgeAmplitude);
                                lastRidgeAmplitude = ridgeAmplitude;
                                noise = MultiFractal(noise, thisOctave, lastOctave, offset, multifractaclA, ref lastRidge);
                                break;
                            default:
                                noise += thisOctave;
                                break;
                        }
                        lastOctave = thisOctave;
                    }
                    break;
                default:
                    break;
            }
            return noise / increment;
        }

        private static float FBM(float noise, float currentOctave)
        {
            return noise + currentOctave;
        }

        private static float Turbulence(float noise, float currentOctave, float offset)
        {
            float turbulence = (currentOctave + 1.0f) * 0.5f;
            turbulence = Mathf.Abs(turbulence + offset - 0.5f) * 2.0f;
            return noise + turbulence;
        }

        private static float Ridge(float noise, float currentOctave, float offset)
        {
            float turbulence = (currentOctave + 1.0f) * 0.5f;
            turbulence = Mathf.Abs(turbulence + offset - 0.5f) * 2.0f;

            float ridge = Mathf.Pow(0.25f - turbulence, 2f);

            return noise + ridge;
        }

        private static float MultiFractal(float noise, float currentOctave, float lastOctave, float offset, float a, ref float lastRidge)
        {

            float turbulence = (currentOctave + 1.0f) * 0.5f;
            turbulence = Mathf.Abs(turbulence + offset - 0.5f) * 2.0f;

            float ridge = Mathf.Pow(0.25f - turbulence, 2f);


            float multifractal = ridge * (1f - a * lastRidge);
            lastRidge = ridge;

            return noise + multifractal;
        }
    }
}
