using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class TestData : Payload<TestData>
{
    public string testString;
}


public class TestController : MonoBehaviour
{
    public static TestController api;

    public TestData testData;
    
    private void Awake()
    {
        api = this;
    }
    
    public void GetServerTestFromServer()
    {
        string endpoint = $"demo/{Server.api.deviceIdentifier}";
        StartCoroutine(Server.api.Get(endpoint, (json) =>
        {
            Debug.Log($"Server Response: {json}");
            TestController.api.testData = TestData.CreateFromJson(json);
        }));
    }

    public void PostServerTestToServer()
    {
        TestData testData = TestController.api.testData;
        string endpoint = $"demo/{Server.api.deviceIdentifier}";
        string json = testData.SerializeToJson();
        StartCoroutine(Server.api.Post(endpoint, json, (response) =>
        {
            Debug.Log($"Response is: {response}");
        }));
    }
}
