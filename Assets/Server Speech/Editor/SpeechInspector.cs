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
        GUILayout.Label("Speech To Text (STT)");
        if (GUILayout.Button("Start Audio Recording"))
        {
            actions.StartAudioRecording();
        }

        if (GUILayout.Button("Stop Audio Recording"))
        {
            actions.StopAudioRecording();
        }

        if (GUILayout.Button("Send Audio Recording"))
        {
            actions.PostLatestAudio();
        }
        
        
        GUILayout.Label("Text To Speech (STT)");

        if (GUILayout.Button("Start Playback"))
        {
            actions.StartPlayback();
        }

        if (GUILayout.Button("Pause Playback"))
        {
            actions.PausePlayback();
        }

        if (GUILayout.Button("Stop Playback"))
        {
            actions.StopPlayback();
        }

        if (GUILayout.Button("Generate TTS"))
        {
            actions.StartPlayback();
        }

        if (GUILayout.Button("Play TTS"))
        {
            actions.StopPlayback();
        }


        GUI.enabled = Application.isEditor;

        base.OnInspectorGUI();
    }
}