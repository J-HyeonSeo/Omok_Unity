using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class RestConnector : MonoBehaviour
{

    private static RestConnector instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    //외부 접근
    public static RestConnector Instance 
    { 
        get 
        { 
            return instance; 
        } 
    }

    private static string host = "http://localhost:8080";

    // Get 요청 외부 호출
    public void GetRequest(string path, Action<UnityWebRequest> callback, string accessToken = "")
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(host + path);
        webRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);
        StartCoroutine(SendRequest(webRequest, callback));
    }

    // Post 요청 외부 호출
    public void Request(string path, string requestBody, string method, Action<UnityWebRequest> callback, string accessToken = "")
    {
        UnityWebRequest webRequest = new UnityWebRequest(host + path, method);

        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);
        webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestBody));
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        StartCoroutine(SendRequest(webRequest, callback));
    }

    private IEnumerator SendRequest(UnityWebRequest webRequest, Action<UnityWebRequest> callback)
    {
        yield return webRequest.SendWebRequest();
        callback(webRequest);
    }

}
