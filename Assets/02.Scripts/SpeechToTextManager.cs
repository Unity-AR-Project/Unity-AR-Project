using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ���� �ν� ����� ó���ϰ� �̺�Ʈ�� �߻���Ű�� Ŭ����
/// </summary>
public class SpeechToTextManager : MonoBehaviour
{
    public static SpeechToTextManager instance { get; private set; }

    public UnityEvent onHooDetected;

    private GoogleSpeechToText googleSpeechToText;
    private MicrophoneInput microphoneInput;
    private AudioClip recordedClip;
    private string currentLanguageCode = "ko-KR";

    private bool isRecording = false; // ���� ������ ����

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
        googleSpeechToText = FindObjectOfType<GoogleSpeechToText>();
        microphoneInput = MicrophoneInput.instance;

        if (googleSpeechToText != null)
        {
            Debug.Log("HandleSpeechRecognized");
            googleSpeechToText.OnSpeechRecognized += HandleSpeechRecognized;
        }
        else
        {
            Debug.LogError("GoogleSpeechToText script not found.");
        }
    }

    /// <summary>
    /// ���� �ν��� �����մϴ�.
    /// </summary>
    public void StartSpeechRecognition()
    {
        if (!isRecording)
        {
            Debug.Log("Calling StartRecording()");
            StartRecording();
        }
    }

    /// <summary>
    /// ����ũ ������ �����մϴ�.
    /// </summary>
    private void StartRecording()
    {
        Debug.Log("Processing StartRecording ");
        recordedClip = microphoneInput.StartRecording();
        isRecording = true;
        //UIManager.instance.ShowLoadingUI("���� ��...");
    }

    /// <summary>
    /// ����ũ ������ �����ϰ� ���� �ν��� ��û�մϴ�.
    /// </summary>
    public void StopSpeechRecognition()
    {
        if (isRecording)
        {

            Debug.Log("StopSpeechRecognition");
            microphoneInput.StopRecording();
            isRecording = false;
            //UIManager.instance.HideLoadingUI();

            if (recordedClip != null)
            {
                Debug.Log("There are recordedClip");
                googleSpeechToText.SendAudioClip(recordedClip, currentLanguageCode);
            }
            else
            {
                Debug.LogError("There are no recorded audio clips.");
            }
        }
    }

    /// <summary>
    /// ���� �ν� ����� ó���մϴ�.
    /// </summary>
    /// <param name="transcript">�νĵ� �ؽ�Ʈ</param>
    private void HandleSpeechRecognized(string transcript)
    {
        Debug.Log("Processing HandleSpeechRecognized");
        Debug.Log("transcript :"+transcript);
        if (transcript.Contains("��"))
        {

            Debug.Log("Whoo! It's detected!");
            onHooDetected?.Invoke();
        }
        else
        {
            Debug.Log("�νĵ� �ؽ�Ʈ: " + transcript);
        }
    }

    private void Update()
    {
        // ���� �ð� ���� (��: 5��)
        if (isRecording && microphoneInput.GetRecordingTime() >= 5f)
        {
            StopSpeechRecognition();
        }
    }

    private void OnDestroy()
    {
        if (googleSpeechToText != null)
        {
            googleSpeechToText.OnSpeechRecognized -= HandleSpeechRecognized;
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
}
