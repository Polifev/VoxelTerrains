using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VoxelTerrains.Renderer;

[CustomEditor(typeof(VoxelRenderer))]
public class VoxelRenderer_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Show terrain"))
        {
            ((VoxelRenderer)target).RefreshMesh();
        }
    }
}
