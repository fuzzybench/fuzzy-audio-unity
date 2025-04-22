using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioController)), CanEditMultipleObjects]
public class AudioInspector : Editor
{
    public override void OnInspectorGUI()
    {
        GUI.enabled = Application.isPlaying;
        
        AudioController actions = target as AudioController;

        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Start\nRecording"))
        {
            actions.StartRecording();
        }
        
        if (GUILayout.Button("Stop\nRecording"))
        {
            actions.StopRecording();
        }
        
        GUILayout.EndHorizontal();
        
        if (GUILayout.Button("POST Recording"))
        {
            actions.PostLatestAudio();
        }
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Start\nPlayback"))
        {
            actions.StartPlayback();
        }
        
        if (GUILayout.Button("Stop\nPlayback"))
        {
            actions.StopPlayback();
        }
        
        GUILayout.EndHorizontal();
        
        GUI.enabled = Application.isEditor;
        
        base.OnInspectorGUI();
    }
}
