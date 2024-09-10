using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshGenerator))]
public class MeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MeshGenerator generator = (MeshGenerator)target;

        if (DrawDefaultInspector())
        {
            if(generator.autoUpdate)
            {
                generator.GenerateMesh(false, false);
            }
        }

        if(GUILayout.Button("GenerateRandom"))
        {
            generator.GenerateMesh();
        }
        if (GUILayout.Button("Generate"))
        {
            generator.GenerateMesh(false, false);
        }
    }
}
