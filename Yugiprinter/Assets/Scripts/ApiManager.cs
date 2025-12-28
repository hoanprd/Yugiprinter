using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ApiManager : MonoBehaviour
{
    private static ApiManager _instance;

    public static ApiManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("ApiManager");
                _instance = go.AddComponent<ApiManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    public void Get(string url, System.Action<string> callback)
    {
        StartCoroutine(GetRequest(url, callback));
    }

    private IEnumerator GetRequest(string url, System.Action<string> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
                callback?.Invoke(null);
            }
            else
            {
                callback?.Invoke(webRequest.downloadHandler.text);
            }
        }
    }
}