using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

public class GoogleSpeechToTextAPI : MonoBehaviour
{
    [SerializeField] private string apiKey;   // 실제 API 키를 입력
    private string apiUrl = "https://speech.googleapis.com/v1/speech:recognize?key=";

    public IEnumerator SendBase64ToGoogle(
       string base64Audio,
       Action<string> onSuccess,
       Action<string> onError)
    {
        string url = apiUrl + apiKey;

        var requestData = new
        {
            config = new
            {
                encoding = "LINEAR16",
                sampleRateHertz = 16000,
                languageCode = "ko-KR"
            },
            audio = new
            {
                content = base64Audio
            }
        };

        string json = JsonUtility.ToJson(requestData);

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                onError?.Invoke(www.error);
            }
            else
            {
                onSuccess?.Invoke(www.downloadHandler.text);
            }
        }
    }
}