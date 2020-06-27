using Unity;
using UnityEngine;
using UnityEditor;
using VoxelTerrains.ScalarField;

[CustomEditor(typeof(ChunkBasedScalarField))]
public class ChunkBasedScalarField_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Regenerate chunks"))
        {
            ((ChunkBasedScalarField)target).ResetChunks();
        }
    }
}
