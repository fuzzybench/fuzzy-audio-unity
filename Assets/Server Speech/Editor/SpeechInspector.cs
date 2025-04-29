using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpeechLogic)), CanEditMultipleObjects]
public class SpeechInspector : Editor
{
    public override void OnInspectorGUI()
    {
        GUI.enabled = Application.isPlaying;
        
        SpeechLogic actions = target as SpeechLogic;

        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Start\nRecording"))
        {
            actions.StartAudioRecording();
        }
        
        if (GUILayout.Button("Stop\nRecording"))
        {
            actions.StopAudioRecording();
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
        if (GUILayout.Button("Pause\nPlayback"))
        {
            actions.PausePlayback();
        }
        
        if (GUILayout.Button("Stop\nPlayback"))
        {
            actions.StopPlayback();
        }
        
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate\nTTS"))
        {
            actions.StartPlayback();
        }
        
        if (GUILayout.Button("Play\nTTS"))
        {
            actions.StopPlayback();
        }
        
        GUILayout.EndHorizontal();
        
        GUI.enabled = Application.isEditor;
        
        base.OnInspectorGUI();
    }
}
