using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using Newtonsoft.Json; // Newtonsoft.Json ���

public class GoogleSpeechToText : MonoBehaviour
{
    // ���� API Ű�� ��ü�ϼ���. ������ ���� �ڵ忡 ���� �ϵ��ڵ��ϴ� ���� �������� �ʽ��ϴ�.
    private string apiKey = "";
    private string apiUrl = "https://speech.googleapis.com/v1/speech:recognize?key=";

    public delegate void SpeechRecognizedDelegate(string transcript);
    public event SpeechRecognizedDelegate OnSpeechRecognized;

    private void Start()
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_API_KEY_HERE")
        {
            Debug.LogError("Google Cloud API Ű�� �������� �ʾҽ��ϴ�. ��ũ��Ʈ�� 'apiKey' ������ API Ű�� �Է��ϼ���.");
        }

        apiUrl += apiKey;
    }

    /// <summary>
    /// ����� Ŭ���� �޾Ƽ� Google Cloud Speech-to-Text API�� �����մϴ�.
    /// </summary>
    /// <param name="clip">������ ����� Ŭ��</param>
    /// <param name="languageCode">��� �ڵ� (��: "ko-KR")</param>
    public void SendAudioClip(AudioClip clip, string languageCode = "ko-KR")
    {
        if (clip == null)
        {
            Debug.LogError("����� Ŭ���� null�Դϴ�.");
            return;
        }

        StartCoroutine(ProcessAudioClip(clip, languageCode));
    }

    /// <summary>
    /// ����� Ŭ���� ó���ϰ� Google Cloud Speech-to-Text API�� �����ϴ� �ڷ�ƾ
    /// </summary>
    private IEnumerator ProcessAudioClip(AudioClip clip, string languageCode)
    {
        Debug.Log("Starting audio clip processing...");

        int sampleRate = clip.frequency;

        // ����� ������ ��������
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        // float �迭�� Int16 �迭�� ��ȯ
        Int16[] intData = new Int16[samples.Length];
        const float rescaleFactor = 32767; // float�� Int16�� ��ȯ�ϱ� ���� ������ ����

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
        }

        // Int16 �迭�� ����Ʈ �迭�� ��ȯ
        byte[] bytesData = new byte[intData.Length * sizeof(short)];
        Buffer.BlockCopy(intData, 0, bytesData, 0, bytesData.Length);

        // ����� ������ ����
        Debug.Log("Audio Clip Samples: " + clip.samples);
        Debug.Log("Audio Clip Channels: " + clip.channels);
        Debug.Log("Audio Clip Frequency: " + clip.frequency);
        Debug.Log("Byte Data Length: " + bytesData.Length);

        // Base64�� ���ڵ�
        string base64Audio = Convert.ToBase64String(bytesData);

        // ��û ��ü ����
        SpeechRequest request = new SpeechRequest()
        {
            config = new RecognitionConfig()
            {
                languageCode = languageCode,
                encoding = "LINEAR16",
                sampleRateHertz = sampleRate,
            },
            audio = new RecognitionAudio()
            {
                content = base64Audio
            }
        };

        // ��û ��ü�� JSON���� ����ȭ
        string jsonRequest = JsonConvert.SerializeObject(request, Formatting.None);

        // ������ JSON ���
        Debug.Log("JSON Request: " + jsonRequest);

        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            Debug.Log("Request URL: " + apiUrl);

            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequest);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // ��û ��� ���
            Debug.Log("Request Headers:");
            Debug.Log("Content-Type: application/json");

            Debug.Log("Request Body Length: " + bodyRaw.Length);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("www.result" + www.result);
                Debug.LogError("WWWError: " + www.error);

                // ���� ���� �ڵ� ���
                long responseCode = www.responseCode;
                Debug.LogError("Response Code: " + responseCode);

                // ���� ���� ���
                string errorResponse = www.downloadHandler.text;
                Debug.LogError("Error Response Text: " + errorResponse);

                // ���� ��� ���
                var responseHeaders = www.GetResponseHeaders();
                if (responseHeaders != null)
                {
                    Debug.Log("Response Headers:");
                    foreach (var header in responseHeaders)
                    {
                        Debug.Log(header.Key + ": " + header.Value);
                    }
                }
            }
            else
            {
                string responseText = www.downloadHandler.text;
                Debug.Log("Response Code: " + www.responseCode);
                Debug.Log("Response Text: " + responseText);

                // JSON ���� �Ľ�
                SpeechResponse response = JsonConvert.DeserializeObject<SpeechResponse>(responseText);
                if (response != null && response.results != null && response.results.Count > 0)
                {
                    Debug.Log("I got the voice recognition result.");
                    string transcript = response.results[0].alternatives[0].transcript;
                    if (!string.IsNullOrEmpty(transcript))
                    {
                        Debug.Log("Transcript: " + transcript);
                        OnSpeechRecognized?.Invoke(transcript);
                    }
                    else
                    {
                        Debug.LogWarning("����� Transcript�� �����ϴ�.");
                        // UIManager.instance.ShowErrorMessage("���� �ν� ����� �����ϴ�.");
                    }
                }
                else
                {
                    Debug.LogWarning("���� �ν� ����� �����ϴ�.");
                    // UIManager.instance.ShowErrorMessage("���� �ν� ����� �����ϴ�.");
                }
            }
        }
    }

    // ��û �� ���� Ŭ����

    [Serializable]
    private class SpeechRequest
    {
        [JsonProperty("config")]
        public RecognitionConfig config;

        [JsonProperty("audio")]
        public RecognitionAudio audio;
    }

    [Serializable]
    private class RecognitionConfig
    {
        [JsonProperty("languageCode")]
        public string languageCode;

        [JsonProperty("encoding")]
        public string encoding;

        [JsonProperty("sampleRateHertz")]
        public int sampleRateHertz;

        // �ʿ��� ��� �߰� �ɼ� �ʵ���� ���⿡ �߰��� �� �ֽ��ϴ�.
    }

    [Serializable]
    private class RecognitionAudio
    {
        [JsonProperty("content")]
        public string content;
    }

    [Serializable]
    private class SpeechResponse
    {
        [JsonProperty("results")]
        public List<SpeechRecognitionResult> results;
    }

    [Serializable]
    private class SpeechRecognitionResult
    {
        [JsonProperty("alternatives")]
        public List<SpeechRecognitionAlternative> alternatives;
    }

    [Serializable]
    private class SpeechRecognitionAlternative
    {
        [JsonProperty("transcript")]
        public string transcript;

        [JsonProperty("confidence")]
        public float confidence;
    }
}
