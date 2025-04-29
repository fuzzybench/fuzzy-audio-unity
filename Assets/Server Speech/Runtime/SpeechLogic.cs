using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;


[Serializable]
public class Payload<T>
{
    // NOTE: This is a wrapper for all server payloads
    public string SerializeToJson()
    {
        return JsonUtility.ToJson(this);
    }
    public static T CreateFromJson(string json)
    {
        return JsonUtility.FromJson<T>(json);
    }
}

[Serializable]
public class AudioObject : Payload<AudioObject>
{
    public float[] audioData;
}

public class SpeechLogic : MonoBehaviour
{
    [Header("Server Settings")] 
    public string serverUri = "http://100.00.00.1:44000";
    
    [Header("STT Data (Whisper)")]
    public AudioClip sttAudioClip;
    [TextArea]
    public string sttTranscript;
    
    [Header("TTS Data (Coqui)")]
    public AudioObject audioObject;
    
    
    private AudioSource audioSource;
    [SerializeField]
    private bool isRecording;
    [SerializeField]
    private AudioClip microphoneRecordingBuffer;
    private float recordingStartTime;
    private float[] data;

    private void Start()
    {
        isRecording = false;
        audioSource = this.GetComponent<AudioSource>();
    }
    
    public void StartAudioRecording()
    {
        print($"Start: Audio Recording");
        if (isRecording) {return;}
        audioObject = new AudioObject();
        isRecording = true;
        recordingStartTime = Time.time;
        microphoneRecordingBuffer = Microphone.Start("", false, 300, 44100);
    }

    public void StopAudioRecording()
    {
        
        if (!isRecording) {return;}
        print($"Stop: Audio Recording");
        Microphone.End("");
        // TODO: refactor plz
        //latestRecordingClip.UnloadAudioData();
        // ... calculate time between start and stop of recording
        float recordingEndTime = Time.time;
        float recordingLength = recordingEndTime - recordingStartTime;
        Debug.Log($"recording length: {recordingLength}");
        // ... create new audio clip
        AudioClip recordingNew = AudioClip.Create(microphoneRecordingBuffer.name,
            (int) (recordingLength * microphoneRecordingBuffer.frequency),
            microphoneRecordingBuffer.channels, microphoneRecordingBuffer.frequency, false);
        // ... create new data object
        print($"creating data @ {Time.time}");
        data = new float[(int)(recordingLength * microphoneRecordingBuffer.frequency)];
        // ... pull values in FROM mic to data object
        print($"copy data @ {Time.time}");
        microphoneRecordingBuffer.GetData(data, 0);
        // ... set values from data object INTO the audio clip object
        recordingNew.SetData(data, 0);

        // ... copy audio clip reference to public field
        sttAudioClip = recordingNew;
        // ... copy audio float data reference to public field
        audioObject.audioData = data;
        print($"unloading audio @ {Time.time}");
        microphoneRecordingBuffer.UnloadAudioData();
        //recordingNew.UnloadAudioData();
        isRecording = false;
    }



    public IEnumerator PostLatestAudio()
    {
        
        print($"POST: Audio Recording");
        string endpoint = $"{serverUri}/audio";
        string json = audioObject.SerializeToJson();
        yield return StartCoroutine(Post(endpoint, json, Debug.Log));
        print("exit posting audio");
    }

    public void StartPlayback()
    {
        audioSource.PlayOneShot(sttAudioClip);
    }

    public void StopPlayback()
    {
        audioSource.Stop();
    }

    public void PausePlayback()
    {
        audioSource.Pause();
    }

    public IEnumerator Get(string endpoint, Action<string> callback)
    {
        //UnityWebRequest webRequest = CreateWebRequest(endpoint, "GET");
        UnityWebRequest webRequest = new UnityWebRequest(serverUri, "GET");
        
        DownloadHandler dH = new DownloadHandlerBuffer();
        webRequest.downloadHandler = dH;
        UploadHandler uH = new UploadHandlerRaw(null);
        webRequest.uploadHandler = uH;
        
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            callback(webRequest.error);
            webRequest.Dispose();
        }
        else
        {
            callback(webRequest.downloadHandler.text);
            webRequest.Dispose();
        }
    }

    public IEnumerator Post(string endpoint, string json, Action<string> callback)
    {
        byte[] bytePayload = System.Text.Encoding.UTF8.GetBytes(json);
        print($"size is {bytePayload.Length}");
        
        UnityWebRequest webRequest = new UnityWebRequest(serverUri, "POST");
        webRequest.SetRequestHeader("Content-Type", "application/json");
        DownloadHandler dH = new DownloadHandlerBuffer();
        webRequest.downloadHandler = dH;
        UploadHandler uH = new UploadHandlerRaw(bytePayload);
        webRequest.uploadHandler = uH;
        
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            callback(webRequest.error);
            webRequest.Dispose();
        }
        else
        {
            callback(webRequest.downloadHandler.text);
            webRequest.Dispose();
        }
    }
    
    public IEnumerator PostTTS(string endpoint, string json, Action<string> callback)
    {
        byte[] bytePayload = System.Text.Encoding.UTF8.GetBytes(json);
        print($"size is {bytePayload.Length}");
        
        UnityWebRequest webRequest = new UnityWebRequest(serverUri, "POST");
        webRequest.SetRequestHeader("Content-Type", "application/json");
        DownloadHandler dH = new DownloadHandlerBuffer();
        webRequest.downloadHandler = dH;
        UploadHandler uH = new UploadHandlerRaw(bytePayload);
        webRequest.uploadHandler = uH;
        
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            callback(webRequest.error);
            webRequest.Dispose();
        }
        else
        {
            callback(webRequest.downloadHandler.text);
            webRequest.Dispose();
        }
    }
}
