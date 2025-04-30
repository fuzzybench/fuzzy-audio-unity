using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpeechLogic)), CanEditMultipleObjects]
public class SpeechLogicInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUI.enabled = Application.isPlaying;

        SpeechLogic actions = target as SpeechLogic;
        GUILayout.Label("Speech To Text (STT)");
        
        if (GUILayout.Button("Start STT Recording"))
        {
            actions.StartAudioRecording();
        }

        if (GUILayout.Button("Stop STT Recording"))
        {
            actions.StopAudioRecording();
        }


        if (GUILayout.Button("Generate Sever STT"))
        {
            actions.StartServerSTT();
        }


        GUILayout.Label("Text To Speech (STT)");

        if (GUILayout.Button("Generate Server TTS"))
        {
            actions.StartServerTTS();
        }


        GUI.enabled = Application.isEditor;
    }
}