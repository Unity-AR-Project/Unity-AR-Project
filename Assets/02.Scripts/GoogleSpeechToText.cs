using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using Newtonsoft.Json; // Newtonsoft.Json 사용

public class GoogleSpeechToText : MonoBehaviour
{
    // 실제 API 키로 교체하세요. 보안을 위해 코드에 직접 하드코딩하는 것은 권장하지 않습니다.
    private string apiKey = "";
    private string apiUrl = "https://speech.googleapis.com/v1/speech:recognize?key=";

    public delegate void SpeechRecognizedDelegate(string transcript);
    public event SpeechRecognizedDelegate OnSpeechRecognized;

    private void Start()
    {
        if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_API_KEY_HERE")
        {
            Debug.LogError("Google Cloud API 키가 설정되지 않았습니다. 스크립트의 'apiKey' 변수에 API 키를 입력하세요.");
        }

        apiUrl += apiKey;
    }

    /// <summary>
    /// 오디오 클립을 받아서 Google Cloud Speech-to-Text API에 전송합니다.
    /// </summary>
    /// <param name="clip">녹음된 오디오 클립</param>
    /// <param name="languageCode">언어 코드 (예: "ko-KR")</param>
    public void SendAudioClip(AudioClip clip, string languageCode = "ko-KR")
    {
        if (clip == null)
        {
            Debug.LogError("오디오 클립이 null입니다.");
            return;
        }

        StartCoroutine(ProcessAudioClip(clip, languageCode));
    }

    /// <summary>
    /// 오디오 클립을 처리하고 Google Cloud Speech-to-Text API로 전송하는 코루틴
    /// </summary>
    private IEnumerator ProcessAudioClip(AudioClip clip, string languageCode)
    {
        Debug.Log("Starting audio clip processing...");

        int sampleRate = clip.frequency;

        // 오디오 데이터 가져오기
        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        // float 배열을 Int16 배열로 변환
        Int16[] intData = new Int16[samples.Length];
        const float rescaleFactor = 32767; // float를 Int16로 변환하기 위한 스케일 팩터

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
        }

        // Int16 배열을 바이트 배열로 변환
        byte[] bytesData = new byte[intData.Length * sizeof(short)];
        Buffer.BlockCopy(intData, 0, bytesData, 0, bytesData.Length);

        // 오디오 데이터 검증
        Debug.Log("Audio Clip Samples: " + clip.samples);
        Debug.Log("Audio Clip Channels: " + clip.channels);
        Debug.Log("Audio Clip Frequency: " + clip.frequency);
        Debug.Log("Byte Data Length: " + bytesData.Length);

        // Base64로 인코딩
        string base64Audio = Convert.ToBase64String(bytesData);

        // 요청 객체 생성
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

        // 요청 객체를 JSON으로 직렬화
        string jsonRequest = JsonConvert.SerializeObject(request, Formatting.None);

        // 생성된 JSON 출력
        Debug.Log("JSON Request: " + jsonRequest);

        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            Debug.Log("Request URL: " + apiUrl);

            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequest);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // 요청 헤더 출력
            Debug.Log("Request Headers:");
            Debug.Log("Content-Type: application/json");

            Debug.Log("Request Body Length: " + bodyRaw.Length);

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("www.result" + www.result);
                Debug.LogError("WWWError: " + www.error);

                // 응답 상태 코드 출력
                long responseCode = www.responseCode;
                Debug.LogError("Response Code: " + responseCode);

                // 응답 본문 출력
                string errorResponse = www.downloadHandler.text;
                Debug.LogError("Error Response Text: " + errorResponse);

                // 응답 헤더 출력
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

                // JSON 응답 파싱
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
                        Debug.LogWarning("결과에 Transcript가 없습니다.");
                        // UIManager.instance.ShowErrorMessage("음성 인식 결과가 없습니다.");
                    }
                }
                else
                {
                    Debug.LogWarning("음성 인식 결과가 없습니다.");
                    // UIManager.instance.ShowErrorMessage("음성 인식 결과가 없습니다.");
                }
            }
        }
    }

    // 요청 및 응답 클래스

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

        // 필요한 경우 추가 옵션 필드들을 여기에 추가할 수 있습니다.
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
