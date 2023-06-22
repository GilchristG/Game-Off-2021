using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MoveData))]
[CanEditMultipleObjects]
public class MoveDataExtension : Editor
{
    
    MoveData data;
    
    void OnEnable()
    {
        data = (MoveData)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();
        
        if(GUILayout.Button("Process animation"))
        {
            data.CheckAnimationFrames();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
