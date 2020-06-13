using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEditor;
using UnityEngine;

namespace VoxelTerrains.Noise
{
    [RequireComponent(typeof(UnityEngine.Renderer)), ExecuteInEditMode]
    public class NoiseDebugger : MonoBehaviour
    {
        [SerializeField, Range(2, 2048)] private int resolution = 256;
        [SerializeField, Range(0.0001f, 1f)] private float scale = 0.015f;
        [SerializeField, Range(1, 8)] private int octave = 6;
        [SerializeField, Range(0f, 10f)] private float lacunarity = 2f;
        [SerializeField, Range(0f, 10f)] private float persistance = 0.5f;
        [SerializeField, Range(0f, 1f)] private float offset = 0.1f;
        [SerializeField, Range(1f, 5f)] private float multifractaclA = 3f;
        [SerializeField] private int seed = 0;
        [SerializeField] private NoiseHandler.NoiseAdditionType additionType = NoiseHandler.NoiseAdditionType.FBM;
        [SerializeField] private NoiseHandler.NoiseType noiseType = NoiseHandler.NoiseType.OpenSimplexNoise;
        [Space]
        Material _material = null;
        Texture2D texture = null;

        // Start is called before the first frame update
        void Start()
        {
            _material = this.GetComponent<UnityEngine.Renderer>().sharedMaterial;
            GenerateTexture();
        }

        public void GenerateTexture()
        {
            if (_material == null)
                _material = this.GetComponent<UnityEngine.Renderer>().sharedMaterial;

            texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, false);
            texture.name = "GeneratedTexture";
            _material.SetTexture("_MainTex", texture);
            Random.InitState(seed);
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    //Debug.Log(((NoiseHandler.Noise(x, y, octave, lacunarity, persistance, scale, offset, seed, noiseType, additionType) + 1) * 0.5f));
                    texture.SetPixel(x, y, ((NoiseHandler.Noise(x, y, octave, lacunarity, persistance, scale, offset, multifractaclA, seed, noiseType, additionType) + 1) * 0.5f) * Color.white);
                }
            }
            texture.Apply();
        }
    }

}