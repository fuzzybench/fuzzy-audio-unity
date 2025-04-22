using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(TestController)), CanEditMultipleObjects]
public class TestInspector : Editor
{
    public override void OnInspectorGUI()
    {
        GUI.enabled = Application.isPlaying;

        GUILayout.BeginHorizontal();

        TestController actions = target as TestController;

        if (GUILayout.Button("GET"))
        {
            actions.GetServerTestFromServer();
        }

        if (GUILayout.Button("POST"))
        {
            actions.PostServerTestToServer();
        }

        GUILayout.EndHorizontal();
        base.OnInspectorGUI();
    }
}