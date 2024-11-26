using UnityEngine;
using UnityEngine.Android;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

public class SpeechRecognitionManager : MonoBehaviour
{
    [Header("Google Cloud API Settings")]
    [SerializeField]
    private string apiKey; // API 키를 Unity 인스펙터에서 설정합니다.

    private string apiUrl = "https://speech.googleapis.com/v1/speech:recognize?key=";

    [Header("Recording Settings")]
    [SerializeField]
    private int sampleRate = 16000; // Google API 권장 샘플링 레이트
    [SerializeField]
    public int recordingLength = 3; // 녹음 시간 (초)

    [Header("Audio Amplification")]
    [Tooltip("Set the amplification factor for recorded audio (default: 1.5x).")]
    [Range(1.0f, 5.0f)]
    public float amplificationFactor = 1.5f; // 인스펙터에서 설정 가능한 증폭 비율

    private AudioClip recordedClip;
    private bool isRecording = false;

    public delegate void SpeechRecognizedDelegate(string transcript);
    public event SpeechRecognizedDelegate OnSpeechRecognized;

    void Start()
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            Debug.LogError("Google Cloud API key is not set. Please set the API key in the inspector.");
            return;
        }

        apiUrl += apiKey;
        Debug.Log("API URL: " + apiUrl);

        RequestMicrophonePermission();
    }

    private void RequestMicrophonePermission()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            PermissionCallbacks permissionCallbacks = new PermissionCallbacks();
            permissionCallbacks.PermissionGranted += PermissionGranted;
            permissionCallbacks.PermissionDenied += PermissionDenied;
            permissionCallbacks.PermissionDeniedAndDontAskAgain += PermissionDeniedAndDontAskAgain;
            Permission.RequestUserPermission(Permission.Microphone, permissionCallbacks);
        }
        else
        {
            Debug.Log("Microphone permission is already granted.");
        }
#else
        Debug.Log("No need to request microphone permission on this platform.");
#endif
    }

    private void PermissionGranted(string permission)
    {
        Debug.Log($"{permission} permission granted.");
    }

    private void PermissionDenied(string permission)
    {
        Debug.LogError($"{permission} permission denied.");
    }

    private void PermissionDeniedAndDontAskAgain(string permission)
    {
        Debug.LogError($"{permission} permission denied and 'Don't Ask Again' selected.");
    }

    public void StartRecording()
    {
        if (isRecording)
        {
            Debug.LogWarning("Already recording.");
            return;
        }

        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone devices found.");
            return;
        }

        recordedClip = Microphone.Start(null, false, recordingLength, sampleRate);
        isRecording = true;
        Debug.Log("Recording started...");
    }

    public void StopRecording()
    {
        if (!isRecording)
        {
            Debug.LogWarning("Not currently recording.");
            return;
        }

        Microphone.End(null);
        isRecording = false;
        Debug.Log("Recording stopped.");

        if (recordedClip != null)
        {
            // Save WAV and process with normalization
            SaveAndProcessAudio(recordedClip);
        }
        else
        {
            Debug.LogError("Recorded AudioClip is null.");
        }
    }

    private void SaveAndProcessAudio(AudioClip clip)
    {
        string filePath = Path.Combine(Application.persistentDataPath, "recordedAudio.wav");
        Debug.Log("Saving audio to: " + filePath);

        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        // Normalize and amplify audio data
        AmplifyAudio(samples);

        // Save normalized audio to a WAV file
        SaveNormalizedWav(filePath, clip, samples);

        // Process and send audio
        StartCoroutine(ProcessAndSendAudioClip(recordedClip));
    }

    private void AmplifyAudio(float[] samples)
    {
        float maxAmplitude = Mathf.Max(samples);
        if (maxAmplitude > 0)
        {
            float normalizationFactor = 1.0f / maxAmplitude; // Normalization
            float totalAmplification = normalizationFactor * amplificationFactor; // Amplification factor applied

            Debug.Log($"Applying Amplification Factor: {totalAmplification}");

            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] *= totalAmplification;
            }
        }
    }


    private void SaveNormalizedWav(string filePath, AudioClip clip, float[] normalizedSamples)
    {
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            byte[] wavHeader = GetWavHeader(clip.samples, clip.channels, clip.frequency);
            fileStream.Write(wavHeader, 0, wavHeader.Length);

            short[] intData = new short[normalizedSamples.Length];
            for (int i = 0; i < normalizedSamples.Length; i++)
            {
                intData[i] = (short)(normalizedSamples[i] * short.MaxValue);
            }

            byte[] bytesData = new byte[intData.Length * sizeof(short)];
            Buffer.BlockCopy(intData, 0, bytesData, 0, bytesData.Length);
            fileStream.Write(bytesData, 0, bytesData.Length);
        }

        Debug.Log("Normalized WAV file saved.");
    }

    private byte[] GetWavHeader(int samples, int channels, int sampleRate)
    {
        byte[] header = new byte[44];
        Encoding.ASCII.GetBytes("RIFF").CopyTo(header, 0);
        BitConverter.GetBytes(36 + samples * channels * 2).CopyTo(header, 4);
        Encoding.ASCII.GetBytes("WAVEfmt ").CopyTo(header, 8);
        BitConverter.GetBytes(16).CopyTo(header, 16);
        BitConverter.GetBytes((short)1).CopyTo(header, 20);
        BitConverter.GetBytes((short)channels).CopyTo(header, 22);
        BitConverter.GetBytes(sampleRate).CopyTo(header, 24);
        BitConverter.GetBytes(sampleRate * channels * 2).CopyTo(header, 28);
        BitConverter.GetBytes((short)(channels * 2)).CopyTo(header, 32);
        BitConverter.GetBytes((short)16).CopyTo(header, 34);
        Encoding.ASCII.GetBytes("data").CopyTo(header, 36);
        BitConverter.GetBytes(samples * channels * 2).CopyTo(header, 40);
        return header;
    }

    private void SaveWavFile(string filename, AudioClip clip)
    {
        var filepath = Path.Combine(Application.persistentDataPath, filename);
        Debug.Log("Saving WAV file to: " + filepath);

        using (var fileStream = new FileStream(filepath, FileMode.Create))
        {
            byte[] wavHeader = GetWavHeader(clip);
            fileStream.Write(wavHeader, 0, wavHeader.Length);

            float[] samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            Int16[] intData = new Int16[samples.Length];
            const float rescaleFactor = 32767;

            for (int i = 0; i < samples.Length; i++)
            {
                intData[i] = (short)(samples[i] * rescaleFactor);
            }

            byte[] bytesData = new byte[intData.Length * sizeof(short)];
            Buffer.BlockCopy(intData, 0, bytesData, 0, bytesData.Length);
            fileStream.Write(bytesData, 0, bytesData.Length);
        }

        Debug.Log("WAV file saved successfully.");
    }

    private IEnumerator ProcessAndSendAudioClip(AudioClip clip)
    {
        Debug.Log("Processing recorded audio clip...");

        float[] samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        // 오디오 데이터 검증
        float minSample = float.MaxValue;
        float maxSample = float.MinValue;
        float sumSamples = 0f;

        for (int i = 0; i < samples.Length; i++)
        {
            float sample = samples[i];
            if (sample < minSample) minSample = sample;
            if (sample > maxSample) maxSample = sample;
            sumSamples += Mathf.Abs(sample);
        }

        float avgSample = sumSamples / samples.Length;
        Debug.Log($"Sample Min: {minSample}, Max: {maxSample}, Avg: {avgSample}");

        if (Mathf.Approximately(maxSample, 0f))
        {
            Debug.LogError("No meaningful audio signal detected.");
            yield break;
        }

        // 오디오 데이터 정규화
        float maxAmplitude = maxSample;
        if (maxAmplitude > 0)
        {
            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] /= maxAmplitude;
            }
        }

        Int16[] intData = new Int16[samples.Length];
        const float rescaleFactor = 32767;

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
        }

        byte[] bytesData = new byte[intData.Length * sizeof(short)];
        Buffer.BlockCopy(intData, 0, bytesData, 0, bytesData.Length);

        string base64Audio = Convert.ToBase64String(bytesData);
        Debug.Log("Base64 Audio Length: " + base64Audio.Length);

        // v1 API 요청 객체 생성
        SpeechRequest request = new SpeechRequest()
        {
            config = new RecognitionConfig()
            {
                languageCode = "ko-KR",
                encoding = "LINEAR16",
                sampleRateHertz = sampleRate,
                audioChannelCount = clip.channels,
                enableSeparateRecognitionPerChannel = false
            },
            audio = new RecognitionAudio()
            {
                content = base64Audio
            }
        };

        string jsonRequest = JsonConvert.SerializeObject(request, Formatting.Indented);
        yield return StartCoroutine(SendRequest(jsonRequest));
    }

    private IEnumerator SendRequest(string jsonRequest)
    {
        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            Debug.Log("Sending request to Google Cloud Speech-to-Text API v1...");

            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequest);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            Debug.Log($"Request Body Length: {bodyRaw.Length}");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Request failed: " + www.error);
                Debug.LogError("Response Code: " + www.responseCode);
                Debug.LogError("Error Response Text: " + www.downloadHandler.text);

                try
                {
                    var errorObj = JsonConvert.DeserializeObject<GoogleApiError>(www.downloadHandler.text);
                    if (errorObj != null && !string.IsNullOrEmpty(errorObj.error.message))
                    {
                        Debug.LogError("Error Message: " + errorObj.error.message);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error parsing error response: " + ex.Message);
                }
            }
            else
            {
                string responseText = www.downloadHandler.text;
                Debug.Log("Response Code: " + www.responseCode);
                Debug.Log("Response Text: " + responseText);

                SpeechResponse response = JsonConvert.DeserializeObject<SpeechResponse>(responseText);
                if (response != null && response.results != null && response.results.Count > 0)
                {
                    Debug.Log("Speech recognition result received.");
                    string transcript = response.results[0].alternatives[0].transcript;
                    if (!string.IsNullOrEmpty(transcript))
                    {
                        Debug.Log("Transcript: " + transcript);
                        OnSpeechRecognized?.Invoke(transcript);
                    }
                    else
                    {
                        Debug.LogWarning("No transcript found in the results.");
                    }
                }
                else
                {
                    Debug.LogWarning("No speech recognition results found.");
                }
            }
        }
    }

    private byte[] GetWavHeader(AudioClip clip)
    {
        int hz = clip.frequency;
        int channels = clip.channels;
        int samples = clip.samples;

        byte[] header = new byte[44];

        header[0] = (byte)'R';
        header[1] = (byte)'I';
        header[2] = (byte)'F';
        header[3] = (byte)'F';

        int fileSize = 36 + samples * channels * 2;
        BitConverter.GetBytes(fileSize).CopyTo(header, 4);

        header[8] = (byte)'W';
        header[9] = (byte)'A';
        header[10] = (byte)'V';
        header[11] = (byte)'E';

        header[12] = (byte)'f';
        header[13] = (byte)'m';
        header[14] = (byte)'t';
        header[15] = (byte)' ';

        BitConverter.GetBytes(16).CopyTo(header, 16);
        BitConverter.GetBytes((short)1).CopyTo(header, 20);
        BitConverter.GetBytes((short)channels).CopyTo(header, 22);
        BitConverter.GetBytes(hz).CopyTo(header, 24);
        BitConverter.GetBytes(hz * channels * 2).CopyTo(header, 28);
        BitConverter.GetBytes((short)(channels * 2)).CopyTo(header, 32);
        BitConverter.GetBytes((short)16).CopyTo(header, 34);

        header[36] = (byte)'d';
        header[37] = (byte)'a';
        header[38] = (byte)'t';
        header[39] = (byte)'a';

        BitConverter.GetBytes(samples * channels * 2).CopyTo(header, 40);

        return header;
    }

    // v1 API 요청 및 응답 클래스

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
        [JsonProperty("encoding")]
        public string encoding;

        [JsonProperty("sampleRateHertz")]
        public int sampleRateHertz;

        [JsonProperty("languageCode")]
        public string languageCode;

        [JsonProperty("audioChannelCount")]
        public int audioChannelCount;

        [JsonProperty("enableSeparateRecognitionPerChannel")]
        public bool enableSeparateRecognitionPerChannel;
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

    [Serializable]
    private class GoogleApiError
    {
        [JsonProperty("error")]
        public ErrorDetail error;

        [Serializable]
        public class ErrorDetail
        {
            [JsonProperty("code")]
            public int code;

            [JsonProperty("message")]
            public string message;

            [JsonProperty("status")]
            public string status;
        }
    }

}
