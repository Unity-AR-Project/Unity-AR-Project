using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 음성 인식 결과를 처리하고 이벤트를 발생시키는 클래스
/// </summary>
public class SpeechToTextManager : MonoBehaviour
{
    public static SpeechToTextManager instance { get; private set; }

    public UnityEvent onHooDetected;

    private SpeechRecognitionManager speechRecognitionManager;

    private string currentLanguageCode = "ko-KR";
    private bool isRecognizing = false; // 음성 인식 중인지 여부

    public bool IsRecognizing
    {
        get { return isRecognizing; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        speechRecognitionManager = FindObjectOfType<SpeechRecognitionManager>();

        if (speechRecognitionManager != null)
        {
            speechRecognitionManager.OnSpeechRecognized += HandleSpeechRecognized;
        }
        else
        {
            Debug.LogError("SpeechRecognitionManager script not found.");
        }
    }

    /// <summary>
    /// 음성 인식을 시작합니다.
    /// </summary>
    public void StartSpeechRecognition()
    {
        if (isRecognizing)
        {
            Debug.LogWarning("Speech recognition is already in progress.");
            return;
        }

        Debug.Log("StartSpeechRecognition called.");
        speechRecognitionManager.StartRecording();
        isRecognizing = true;
    }

    /// <summary>
    /// 음성 인식을 중지하고 결과를 처리합니다.
    /// </summary>
    public void StopSpeechRecognition()
    {
        if (!isRecognizing)
        {
            Debug.LogWarning("Speech recognition is not in progress.");
            return;
        }

        Debug.Log("StopSpeechRecognition called.");
        speechRecognitionManager.StopRecording();
        isRecognizing = false;
    }

    /// <summary>
    /// 음성 인식 결과를 처리합니다.
    /// </summary>
    /// <param name="transcript">인식된 텍스트</param>
    private void HandleSpeechRecognized(string transcript)
    {
        Debug.Log("HandleSpeechRecognized called.");
        Debug.Log("Transcript: " + transcript);

        isRecognizing = false; // 음성 인식 완료

        if (transcript.Contains("후"))
        {
            Debug.Log("Hoo detected!");
            //onHooDetected?.Invoke();
        }
        else
        {
            Debug.Log("No target word detected in the transcript.");
        }
    }

    private void OnDestroy()
    {
        if (speechRecognitionManager != null)
        {
            speechRecognitionManager.OnSpeechRecognized -= HandleSpeechRecognized;
        }
    }

    /// <summary>
    /// 음성 인식 언어 코드를 설정합니다.
    /// </summary>
    /// <param name="languageCode">언어 코드 (예: "ko-KR")</param>
    public void SetLanguageCode(string languageCode)
    {
        currentLanguageCode = languageCode;
    }

    /// <summary>
    /// 녹음 시간을 반환합니다.
    /// </summary>
    /// <returns>녹음 시간 (초)</returns>
    public float GetRecordingLength()
    {
        //return speechRecognitionManager != null ? speechRecognitionManager.recordingLength : 5f;
        return speechRecognitionManager.recordingLength;
    }

}
