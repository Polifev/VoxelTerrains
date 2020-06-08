﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VoxelTerrains;
using System.IO;
using VoxelTerrains.ScriptableObjects;

[CustomEditor(typeof(CubeConfigurations))]
public class CubeConfigurations_Editor : Editor
{
    private string _csvPath = "";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUIStyle rich = new GUIStyle(EditorStyles.boldLabel);
        rich.fontStyle = FontStyle.Bold;
        rich.fontSize = 12;
        rich.richText = true;

        //if(target.Configuration.Length <= 0)
        //{
        //    _csvPath = EditorUtility.OpenFilePanel("Load CSV configuration", "", "csv");
        //}

        if (((CubeConfigurations)target).Configurations == null || ((CubeConfigurations)target).Configurations.Length == 0)
        {
            GUILayout.Label("<color=red>Please select a CSV configuration</color>", rich);
        }
        else
        {
            GUILayout.Label("<color=green>CSV loaded successfully.</color>", rich);
        }

        if (GUILayout.Button("Load CSV configuration"))
        {
            Undo.RecordObject(target, "Load new CSV");
            _csvPath = EditorUtility.OpenFilePanel("Load CSV configuration", "", "csv");
            ((CubeConfigurations)target).Configurations = new CsvCubeConfigurationsLoader().LoadFromFile(_csvPath);
            EditorUtility.SetDirty(target);
        }

        GUILayout.Space(15.0f);
        
    }
}