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
        [SerializeField, Range(0.0001f, 100f)] private float scale = 1;
        [SerializeField] private int seed = 0;

        Material _material = null;
        Texture2D texture = null;
        OpenSimplexNoise noise = null;

        // Start is called before the first frame update
        void Start()
        {
            _material = this.GetComponent<UnityEngine.Renderer>().sharedMaterial;
            noise = new OpenSimplexNoise(seed);
        }

        // Update is called once per frame
        void Update()
        {
            if (_material == null)
                _material = this.GetComponent<UnityEngine.Renderer>().sharedMaterial;

            texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, false);
            texture.name = "GeneratedTexture";
            _material.SetTexture("_MainTex", texture);

            GenerateTexture();
        }

        void GenerateTexture()
        {
            Random.InitState(seed);
            noise = new OpenSimplexNoise(seed);
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    texture.SetPixel(x, y, (((float)noise.eval(((float)x / (float)resolution) * scale, ((float)y/ (float)resolution) * scale)+1f)*0.5f) * Color.white);
                }
            }
            texture.Apply();
        }
    }

}