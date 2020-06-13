using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;

namespace VoxelTerrains.Noise
{
    [CustomEditor(typeof(NoiseDebugger))]
    public class NoiseDebugger_Editor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate Noise"))
            {
                ((NoiseDebugger)target).GenerateTexture();
            }
        }
    }
}