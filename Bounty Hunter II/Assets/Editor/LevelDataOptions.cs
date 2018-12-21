using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelData))]
public class LevelDataOptions : Editor
{
    
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        LevelData lvl = (LevelData)target;
        

        if (GUILayout.Button("Save Level"))
        {
            lvl.SaveThing();
        }
        

    }
}
