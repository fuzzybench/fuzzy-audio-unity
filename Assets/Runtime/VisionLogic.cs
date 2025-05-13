using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class VisionLogic : MonoBehaviour
{
    [Header("Server Settings")] public string serverUri = "https://128.84.84.84:44000";

    public Texture2D image;
    public Texture2D samImage;

    void Start()
    {
        
    }

    public void StartServerImage()
    {
        StartCoroutine(PostImage());
    }

    public IEnumerator PostImage()
    {
        byte[] bytePayload = image.EncodeToPNG();
        UnityWebRequest webRequest = new UnityWebRequest(serverUri + "/image", "POST");
        webRequest.SetRequestHeader("Content-Type", "image/png");
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
            print(webRequest.downloadHandler.text);
            webRequest.Dispose();
        }
    }


    public IEnumerator PostSam()
    {
        byte[] bytePayload = image.EncodeToPNG();
        UnityWebRequest webRequest = new UnityWebRequest(serverUri + "/sam", "POST");
        webRequest.SetRequestHeader("Content-Type", "image/png");
        DownloadHandler dH = new DownloadHandlerTexture();
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
            samImage = DownloadHandlerTexture.GetContent(webRequest);
            webRequest.Dispose();
        }
    }
}