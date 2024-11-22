using UnityEngine;

public class AudioToSpeechExample : MonoBehaviour
{
    private AudioToBase64Converter base64Converter;
    private GoogleSpeechToTextAPI googleSpeechAPI;

    private void Start()
    {
        base64Converter = GetComponent<AudioToBase64Converter>();
        googleSpeechAPI = GetComponent<GoogleSpeechToTextAPI>();
    }

    public void ConvertAndSendAudio(string filePath)
    {
        filePath = Application.dataPath + "/AudioFiles/recordedAudio.wav";

        // WAV ������ Base64�� ��ȯ
        string base64Audio = base64Converter.ConvertWavToBase64(filePath);
        if (string.IsNullOrEmpty(base64Audio))
        {
            Debug.LogError("Failed to convert audio file to Base64.");
            return;
        }

        // Google STT API ��û ����
        StartCoroutine(googleSpeechAPI.SendBase64ToGoogle(
            base64Audio,
            onSuccess: (response) =>
            {
                Debug.Log("Google STT Success: " + response);
            },
            onError: (error) =>
            {
                Debug.LogError("Google STT Error: " + error);
            }
        ));
    }
}
