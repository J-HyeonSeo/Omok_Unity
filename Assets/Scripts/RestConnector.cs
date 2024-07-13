using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

    //�ܺ� ����
    public static RestConnector Instance 
    { 
        get 
        { 
            return instance; 
        } 
    }

    private static string host = "http://localhost:8080";

    // Get ��û �ܺ� ȣ��
    public void GetRequest<T>(string path, Action<T> callback, string accessToken = "")
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(host + path);
        webRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);
        StartCoroutine(SendRequest<T>(webRequest, callback));
    }

    // Post ��û �ܺ� ȣ��
    public void Request<T>(string path, string requestBody, string method, Action<T> callback, string accessToken = "")
    {
        UnityWebRequest webRequest = new UnityWebRequest(host, method);

        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);
        webRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(requestBody));
        webRequest.downloadHandler = new DownloadHandlerBuffer();
        StartCoroutine(SendRequest<T>(webRequest, callback));
    }

    private IEnumerator SendRequest<T>(UnityWebRequest webRequest, Action<T> callback)
    {
        yield return webRequest.SendWebRequest();
        T converted = ProcessResponse<T>(webRequest);
        callback(converted);
    }

    private T ProcessResponse<T>(UnityWebRequest webRequest)
    {
        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + webRequest.error);
            return default;
        }
        else
        {
            // JSON ������ ������ Ŭ���� ���·� ��ȯ
            string json = webRequest.downloadHandler.text;
            Debug.Log(json);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

}
