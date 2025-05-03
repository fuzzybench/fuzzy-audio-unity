using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif


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
public class STTRequest : Payload<STTRequest>
{
    public float[] audioData;
}


[Serializable]
public class STTResponse : Payload<STTResponse>
{
    public string transcript;
}

[Serializable]
public class TTSRequest : Payload<TTSRequest>
{
    public string transcript;
}

[Serializable]
public class TTSResponse : Payload<TTSResponse>
{
    public float[] audioData;
}

public class SpeechLogic : MonoBehaviour
{
    public float timeStart = 0;
    public float timeEnd = 0;
    public float timeElapsed = 0;
    [Header("Server Settings")] public string serverUri = "https://128.84.84.84:44000";
    public STTRequest sttRequest;
    public STTResponse sttResponse;
    public TTSRequest ttsRequest;
    public TTSResponse ttsResponse;

    [Header("STT Data (Whisper)")] public AudioClip sttAudioClip;
    [TextArea] public string sttTranscript;

    [Header("TTS Data (Coqui)")] public AudioClip ttsAudioClip;
    [TextArea] public string ttsTranscript;

    // 
    [SerializeField] private bool isRecording;
    [SerializeField] private AudioClip microphoneRecordingBuffer;
    private float recordingStartTime;
    private float[] data;
    GameObject dialog = null;

    private void Start()
    {
        isRecording = false;
        ttsRequest = new TTSRequest();
        sttRequest = new STTRequest();


// #if PLATFORM_ANDROID
//         if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
//         {
//             Permission.RequestUserPermission(Permission.Microphone);
//             dialog = new GameObject();
//         }
//
//         if (enableCameraRequest)
//         {
//             if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
//             {
//                 Permission.RequestUserPermission(Permission.Camera);
//                 dialog = new GameObject();
//             }
//         }
//
// #endif
    }

    public void CopySTTtoTTS()
    {
        ttsTranscript = sttTranscript;
    }


    public void StartServerSTT()
    {
        print($"Starting Coroutine: PostServerSTT");
        StartCoroutine(PostSTT());
    }

    public void StartServerTTS()
    {
        print($"Starting Coroutine: Post Server TTS");
        StartCoroutine(PostTTS());
    }

    public void StartAudioRecording()
    {
        print($"Start: Audio Recording");
        if (isRecording)
        {
            return;
        }

        sttRequest = new STTRequest();
        isRecording = true;
        recordingStartTime = Time.time;
        microphoneRecordingBuffer = Microphone.Start("", false, 300, 44100);
    }

    public void StopAudioRecording()
    {
        if (!isRecording)
        {
            return;
        }
        
        print($"Stop: Audio Recording");
        timeStart = Time.time;

        Microphone.End("");
        // TODO: refactor plz
        //latestRecordingClip.UnloadAudioData();
        // ... calculate time between start and stop of recording
        float recordingEndTime = Time.time;
        float recordingLength = recordingEndTime - recordingStartTime;
        Debug.Log($"recording length: {recordingLength}");
        // ... create new audio clip
        AudioClip recordingNew = AudioClip.Create(microphoneRecordingBuffer.name,
            (int)(recordingLength * microphoneRecordingBuffer.frequency),
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
        sttRequest.audioData = data;
        print($"unloading audio @ {Time.time}");
        microphoneRecordingBuffer.UnloadAudioData();
        //recordingNew.UnloadAudioData();
        isRecording = false;
        StartServerSTT();
    }


    public IEnumerator PostSTT()
    {
        string json = sttRequest.SerializeToJson();
        byte[] bytePayload = System.Text.Encoding.UTF8.GetBytes(json);
        print($"size is {bytePayload.Length}");

        string completeUri = serverUri + "/stt";
        print($"The uri - {completeUri}");

        UnityWebRequest webRequest = new UnityWebRequest(completeUri, "POST");
        webRequest.SetRequestHeader("Content-Type", "application/json");
        DownloadHandler dH = new DownloadHandlerBuffer();
        webRequest.downloadHandler = dH;
        UploadHandler uH = new UploadHandlerRaw(bytePayload);
        webRequest.uploadHandler = uH;

        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            print(webRequest.error);
            webRequest.Dispose();
        }
        else
        {
            print(sttResponse);
            print(webRequest.downloadHandler.text);
            sttTranscript = webRequest.downloadHandler.text;
            CopySTTtoTTS();
            StartServerTTS();
            webRequest.Dispose();
        }
    }

    public IEnumerator PostTTS()
    {
        ttsRequest.transcript = ttsTranscript;
        string json = ttsRequest.SerializeToJson();
        byte[] bytePayload = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest webRequest = new UnityWebRequest(serverUri + "/tts", "POST");
        webRequest.SetRequestHeader("Content-Type", "application/json");
        DownloadHandler dH = new DownloadHandlerBuffer();
        webRequest.downloadHandler = dH;
        UploadHandler uH = new UploadHandlerRaw(bytePayload);
        webRequest.uploadHandler = uH;
        webRequest.timeout = 60;

        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            print(webRequest.error);
            webRequest.Dispose();
        }
        else
        {
            print(ttsResponse);
            print(webRequest.downloadHandler.data);
            byte[] results = webRequest.downloadHandler.data;
            string rawDataString = System.Text.Encoding.UTF8.GetString(results);
            print(rawDataString);
            string partialTrimDataString = rawDataString.Remove(0, 2);
            string trimmedDataString = partialTrimDataString.Remove(partialTrimDataString.Length - 2, 2);
            print("trimmed data string");
            print(trimmedDataString);
            float[] audioData = Array.ConvertAll(trimmedDataString.Split(','), float.Parse);
            ttsResponse.audioData = audioData;
            ttsAudioClip = AudioClip.Create("tts",
                (int)(audioData.Length), 1, 24000, false);
            ttsAudioClip.SetData(audioData, 0);
            print("conversion complete");
            
            webRequest.Dispose();

            timeEnd = Time.time;
            timeElapsed = timeEnd - timeStart;
            
            AudioSource currentAudioSource = GetComponent<AudioSource>();
            currentAudioSource.clip = ttsAudioClip;
            currentAudioSource.Play();

            
        }
    }
}