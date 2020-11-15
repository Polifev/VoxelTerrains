using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[ExecuteInEditMode]
public class AmplifyToWorldGenerator : MonoBehaviour
{
    [SerializeField] private bool doConversion = false;
    [SerializeField] private Shader sourceShader;
    [SerializeField] private ComputeShader targetComputeShader;

    private void Start()
    {
        Convert(sourceShader, targetComputeShader);
    }

    private void Update()
    {
        if (doConversion)
        {
            Convert(sourceShader, targetComputeShader);
            doConversion = false;
        }
    }

    private string GetDefaultText(string function, string main)
    {
        string defaultText = @"
#pragma kernel CSMain
#include " + '\u0022' + "Util.compute" +'\u0022' + @"

RWStructuredBuffer<float> chunk;
float3 chunkPosition;

"
+ function +
@"

float4 Evaluate(float3 _WorldPosition) 
{
    float4 finalColor;
"
+ main +
@"
}


[numthreads(8,8,8)]
void CSMain (uint3 id : SV_DispatchThreadID)
{
    uint index = indexFromCoords(id);
    float4 finalColor = Evaluate(id + chunkPosition);
    float value = -1.0;
    if (finalColor.x > 0) 
    {
        value = 1.0;
    }
    chunk[index] = value;
}
";

        return defaultText;
    }

    public void Convert(Shader sourceShader, ComputeShader targetComputeShader)
    {
        #if UNITY_EDITOR
        if (sourceShader == null) { return; }
        if (targetComputeShader == null) { return; }

        string[] allShaderLines = File.ReadAllLines(AssetDatabase.GetAssetPath(sourceShader));
        List<string> useShaderLines_Function = new List<string>();
        List<string> useShaderLines_Main = new List<string>();

        bool keepLine = false;
        for (int i = 0; i < allShaderLines.Length; i++)
        {
            if (allShaderLines[i] == "			struct v2f") { keepLine = true; i += 10; }
            if (allShaderLines[i] == "			v2f vert ( appdata v )") { keepLine = false; break; }

            if (keepLine)
            {
                useShaderLines_Function.Add(allShaderLines[i]);
            }
        }

        keepLine = false;
        for (int i = 0; i < allShaderLines.Length; i++)
        {
            if (allShaderLines[i] == "				fixed4 finalColor;") { keepLine = true; i += 4; }
            if (allShaderLines[i] == "				return finalColor;")
            {
                useShaderLines_Main.Add(allShaderLines[i]);
                keepLine = false;
                break;
            }

            if (keepLine)
            {
                useShaderLines_Main.Add(allShaderLines[i]);
            }
        }

        string finalCode_Function = "";
        for (int i = 0; i < useShaderLines_Function.Count; i++) { finalCode_Function += useShaderLines_Function[i] + "\n"; }

        string finalCode_Main = "";
        for (int i = 0; i < useShaderLines_Main.Count; i++) { finalCode_Main += useShaderLines_Main[i] + "\n"; }

        string finalCode = GetDefaultText(finalCode_Function, finalCode_Main);
        File.WriteAllText(AssetDatabase.GetAssetPath(targetComputeShader), finalCode);
        EditorUtility.SetDirty(targetComputeShader);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        #endif
    }
}
