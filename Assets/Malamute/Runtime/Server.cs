using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
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

public class Server : MonoBehaviour
{
    public static Server api;
    [Header("Device Info")] public string deviceIdentifier;
    public string deviceIp;
    

    [Header("Server Settings")] 
    public string serverUrl = "https://www.paperdragons.app";
    public string serverIp = "100.14.63.251";
    public string port = "44000";
    public string apiVersion = "1";

    [HideInInspector]
    [Header("Ping Settings")] public bool pingEnabled = false;
    [HideInInspector]
    public float pingInterval = 2.0f;
    [HideInInspector]
    public int lastPingTime = 0;

    private void Awake()
    {
        api = this;
        deviceIp = WhatIsMyIP();
        deviceIdentifier = WhatIsMyIdentifier();
    }

    private void Start()
    {
        StartCoroutine(PingInterval());
    }

    public IEnumerator Get(string endpoint, Action<string> callback)
    {
        UnityWebRequest webRequest = CreateWebRequest(endpoint, "GET");
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
        UnityWebRequest webRequest = CreateWebRequest(endpoint, "POST", bytePayload);
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

    public IEnumerator GetTexture2D(string endpoint, Action<Texture2D> callback)
    {
        //UnityWebRequest webRequest = CreatePNGRequest(endpoint, "GET");
        UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(GetUri(endpoint));
        Debug.Log($"endpoint for get: {webRequest.uri}");
        yield return webRequest.SendWebRequest();
        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(webRequest.error);
        }
        else
        {
            callback(((DownloadHandlerTexture)webRequest.downloadHandler).texture);
        }
    }
    public IEnumerator PostTexture2D(string endpoint, Texture2D image, Action<string> callback)
    {
        byte[] bytePayload = ImageConversion.EncodeToPNG(image);
        UnityWebRequest webRequest = CreatePNGRequest(endpoint, "POST", bytePayload);
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


    

    private UnityWebRequest CreatePNGRequest(string endpoint, string method, byte[] bytePayload = null)
    {
        UnityWebRequest webRequest;
        if (method == "GET")
        {

        }
        if (method == "POST")
        {
            webRequest = new UnityWebRequest(GetUri(endpoint), method);
            webRequest.SetRequestHeader("Content-Type", "image/png");
            DownloadHandler dH = new DownloadHandlerBuffer();
            webRequest.downloadHandler = dH;
            UploadHandler uH = new UploadHandlerRaw(bytePayload);
            webRequest.uploadHandler = uH;
            return webRequest;
        }

        return null;
    }

    private UnityWebRequest CreateWebRequest(string endpoint, string method, byte[] bytePayload = null)
    {
        UnityWebRequest webRequest = new UnityWebRequest(GetUri(endpoint), method);
        if (method == "POST")
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");
        }

        DownloadHandler dH = new DownloadHandlerBuffer();
        webRequest.downloadHandler = dH;
        UploadHandler uH = new UploadHandlerRaw(bytePayload);
        webRequest.uploadHandler = uH;
        return webRequest;
    }

    private string GetUri(string endpoint)
    {
        // HOTFIX: this overwrites the GetUri functionality. abstract into a different method.
        //return GetUrl(endpoint);
        //string uri = $"http://{serverIp}:{port}/v{apiVersion}/{endpoint}";
        string uri = GetUrl(endpoint);
        
        Debug.Log($"the uri from geturi method returns {uri}");
        return uri;
    }
    
    private string GetUrl(string endpoint)
    {
        string uri = $"{serverUrl}/v{apiVersion}/{endpoint}";

        Debug.Log($"...{uri}");
        return uri;
    }

    private static string WhatIsMyIP()
    {
        return Dns.GetHostEntry(Dns.GetHostName())
            .AddressList.First(
                f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            .ToString();
    }

    private static string WhatIsMyIdentifier()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }

    IEnumerator PingRoute()
    {
        WaitForSeconds f = new WaitForSeconds(0.05f);
        Ping p = new Ping(serverIp);
        while (p.isDone == false)
        {
            yield return f;
        }

        Debug.Log($"Ping: {p.time}");
        p.DestroyPing();
    }

    IEnumerator PingInterval()
    {
        while (pingEnabled)
        {
            yield return new WaitForSecondsRealtime(pingInterval);
            yield return StartCoroutine(PingRoute());
        }
    }
}
