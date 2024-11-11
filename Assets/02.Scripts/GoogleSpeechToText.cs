/*using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine.Networking;

public class GoogleSpeechToText : MonoBehaviour
{
    private string apiKey = "YOUR_GOOGLE_CLOUD_API_KEY"; // 서비스 계정 키 대신 API 키 사용
    private string apiUrl = "https://speech.googleapis.com/v1/speech:recognize?key=";

    public delegate void SpeechRecognizedDelegate(string transcript);
    public event SpeechRecognizedDelegate OnSpeechRecognized;

    public void SendAudioClip(AudioClip clip)
    {
        StartCoroutine(ProcessAudioClip(clip));
    }

    IEnumerator ProcessAudioClip(AudioClip clip)
    {
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        short[] intData = new short[samples.Length];
        for (int i = 0; i < samples.Length; i++)
        {
            float sample = Mathf.Clamp(samples[i], -1f, 1f);
            intData[i] = (short)(sample * short.MaxValue);
        }

        byte[] byteData = new byte[intData.Length * sizeof(short)];
        Buffer.BlockCopy(intData, 0, byteData, 0, byteData.Length);

        string base64Audio = System.Convert.ToBase64String(byteData);

        JObject json = new JObject(
            new JProperty("config", new JObject(
                new JProperty("encoding", "LINEAR16"),
                new JProperty("sampleRateHertz", 16000),
                new JProperty("languageCode", "ko-KR")
            )),
            new JProperty("audio", new JObject(
                new JProperty("content", base64Audio)
            ))
        );

        string jsonString = json.ToString();

        using (UnityWebRequest www = new UnityWebRequest(apiUrl + apiKey, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                string responseText = www.downloadHandler.text;
                JObject responseJson = JObject.Parse(responseText);
                string transcript = "";
                if (responseJson["results"] != null && responseJson["results"].HasValues)
                {
                    transcript = responseJson["results"][0]["alternatives"][0]["transcript"].ToString();
                }
                OnSpeechRecognized?.Invoke(transcript);
            }
        }
    }
}
*/