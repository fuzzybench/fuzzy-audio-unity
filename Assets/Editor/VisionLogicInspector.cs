using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using XRC.Toolkit.Speech;


    [CustomEditor(typeof(VisionLogic)), CanEditMultipleObjects]
    public class VisionLogicInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            VisionLogic actions = target as VisionLogic;
            GUILayout.Label("Speech To Text (STT)");

            if (GUILayout.Button("Post Image"))
            {
                actions.StartServerImage();
            }

            GUI.enabled = Application.isEditor;
        }
    }