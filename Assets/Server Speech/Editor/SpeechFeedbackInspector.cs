using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpeechFeedback)), CanEditMultipleObjects]
public class SpeechFeedbackInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUI.enabled = Application.isPlaying;

        SpeechFeedback actions = target as SpeechFeedback;
        GUILayout.Label("Speech To Text (STT)");

        if (GUILayout.Button("Start STT Playback"))
        {
            actions.StartSTTPlayback();
        }

        if (GUILayout.Button("Pause STT Playback"))
        {
            actions.PauseSTTPlayback();
        }

        if (GUILayout.Button("Stop STT Playback"))
        {
            actions.StopSTTPlayback();
        }


        GUILayout.Label("Text To Speech (STT)");

        if (GUILayout.Button("Start TTS Playback"))
        {
            actions.StartTTSPlayback();
        }

        if (GUILayout.Button("Pause TTS Playback"))
        {
            actions.PauseTTSPlayback();
        }

        if (GUILayout.Button("Stop TTS Playback"))
        {
            actions.StopTTSPlayback();
        }
   
        GUI.enabled = Application.isEditor;
    }
}