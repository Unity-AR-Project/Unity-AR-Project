using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ���� �ν� ����� ó���ϰ� �̺�Ʈ�� �߻���Ű�� Ŭ����
/// </summary>
public class SpeechToTextManager : MonoBehaviour
{
    public static SpeechToTextManager instance { get; private set; }

    public UnityEvent onHooDetected;

    private SpeechRecognitionManager speechRecognitionManager;

    private string currentLanguageCode = "ko-KR";
    private bool isRecognizing = false; // ���� �ν� ������ ����

    public bool IsRecognizing
    {
        get { return isRecognizing; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �ÿ��� ����
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
    /// ���� �ν��� �����մϴ�.
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
    /// ���� �ν��� �����ϰ� ����� ó���մϴ�.
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
    /// ���� �ν� ����� ó���մϴ�.
    /// </summary>
    /// <param name="transcript">�νĵ� �ؽ�Ʈ</param>
    private void HandleSpeechRecognized(string transcript)
    {
        Debug.Log("HandleSpeechRecognized called.");
        Debug.Log("Transcript: " + transcript);

        isRecognizing = false; // ���� �ν� �Ϸ�

        if (transcript.Contains("��"))
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
    /// ���� �ν� ��� �ڵ带 �����մϴ�.
    /// </summary>
    /// <param name="languageCode">��� �ڵ� (��: "ko-KR")</param>
    public void SetLanguageCode(string languageCode)
    {
        currentLanguageCode = languageCode;
    }

    /// <summary>
    /// ���� �ð��� ��ȯ�մϴ�.
    /// </summary>
    /// <returns>���� �ð� (��)</returns>
    public float GetRecordingLength()
    {
        //return speechRecognitionManager != null ? speechRecognitionManager.recordingLength : 5f;
        return speechRecognitionManager.recordingLength;
    }

}
