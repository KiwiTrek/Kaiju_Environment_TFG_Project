using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HeatmapGenerator))]
public class DataCompilatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HeatmapGenerator script = (HeatmapGenerator)target;
        GUILayout.Space(10.0f);
        if (GUILayout.Button("Refresh Map", GUILayout.Height(50.0f)))
        {
            if (script.canUpdate) script.DownloadData();
        }
    }
}
