using System;
using System.Collections;
using UnityEngine;


[Serializable]
public class AudioObject : Payload<AudioObject>
{
    public string audioHash;
    public int audioTimestamp;
    [HideInInspector]
    public float[] audioData;
}

public class AudioController : MonoBehaviour
{
    [TextArea]
    public string transcript;
    public static AudioController api;
    public AudioObject audioObject;
    public AudioSource audioSource;

    public bool isRecording;
    public bool isRecordingSent;

    private AudioClip microphoneRecordingBuffer;
    public AudioClip lastRecordedaAudioClip;

    private float recordingStartTime;
    private float[] data;

    private void Awake()
    {
        api = this;
        isRecording = false;
        isRecordingSent = false;
        audioSource = this.GetComponent<AudioSource>();
    }
    
    private void StartAudioRecording()
    {
        audioObject = new AudioObject
        {
            audioTimestamp = (int)DateTimeOffset.Now.ToUnixTimeSeconds(),
            audioHash = Guid.NewGuid().ToString()
        };
        isRecording = true;
        recordingStartTime = Time.time;
        microphoneRecordingBuffer = Microphone.Start("", false, 300, 44100);
    }

    private void StopAudioRecording()
    {
        Microphone.End("");
        GenerateAudioClipFromRecording();
        isRecording = false;
    }

    // TODO: refactor plz
    private void GenerateAudioClipFromRecording()
    {
        //latestRecordingClip.UnloadAudioData();
        // ... calculate time between start and stop of recording
        float recordingEndTime = Time.time;
        float recordingLength = recordingEndTime - recordingStartTime;
        Debug.Log($"recording length: {recordingLength}");
        // ... create new audio clip
        /*AudioClip recordingNew = AudioClip.Create(_microphoneRecordingBuffer.name,
            (int) (recordingLength * _microphoneRecordingBuffer.frequency),
            _microphoneRecordingBuffer.channels, _microphoneRecordingBuffer.frequency, false);*/
        // ... create new data object
        print($"creating data @ {Time.time}");
        data = new float[(int)(recordingLength * microphoneRecordingBuffer.frequency)];
        // ... pull values in FROM mic to data object
        print($"copy data @ {Time.time}");
        microphoneRecordingBuffer.GetData(data, 0);
        // ... set values from data object INTO the audio clip object
        //recordingNew.SetData(_data, 0);

        // ... copy audio clip reference to public field
        //latestRecordingClip = recordingNew;
        // ... copy audio float data reference to public field
        audioObject.audioData = data;
        print($"unloading audio @ {Time.time}");
        microphoneRecordingBuffer.UnloadAudioData();
        //recordingNew.UnloadAudioData();
    }

    public void StartRecording()
    {
        if (isRecording) {return;}
        StartAudioRecording();
    }

    public void StopRecording()
    {
        if (!isRecording) {return;}
        StopAudioRecording();
    }

    public IEnumerator PostLatestAudio()
    {
        print("posting audio");
        string endpoint = $"message/audio";
        string json = audioObject.SerializeToJson();
        yield return StartCoroutine(Server.api.Post(endpoint, json, Debug.Log));
        print("exit posting audio");
    }

    public void StartPlayback()
    {
        
    }

    public void StopPlayback()
    {
        
    }

    public void PausePlayback()
    {
        
    }
}