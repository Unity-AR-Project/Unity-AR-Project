using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 음성 인식 결과를 처리하고 이벤트를 발생시키는 클래스
/// </summary>
public class SpeechToTextManager : MonoBehaviour
{
    public static SpeechToTextManager instance { get; private set; }

    public UnityEvent onHooDetected;

    private GoogleSpeechToText googleSpeechToText;
    private MicrophoneInput microphoneInput;
    private AudioClip recordedClip;
    private string currentLanguageCode = "ko-KR";

    private bool isRecording = false; // 녹음 중인지 여부

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
    /// 음성 인식을 시작합니다.
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
    /// 마이크 녹음을 시작합니다.
    /// </summary>
    private void StartRecording()
    {
        Debug.Log("Processing StartRecording ");
        recordedClip = microphoneInput.StartRecording();
        isRecording = true;
        //UIManager.instance.ShowLoadingUI("녹음 중...");
    }

    /// <summary>
    /// 마이크 녹음을 중지하고 음성 인식을 요청합니다.
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
    /// 음성 인식 결과를 처리합니다.
    /// </summary>
    /// <param name="transcript">인식된 텍스트</param>
    private void HandleSpeechRecognized(string transcript)
    {
        Debug.Log("Processing HandleSpeechRecognized");
        Debug.Log("transcript :"+transcript);
        if (transcript.Contains("후"))
        {

            Debug.Log("Whoo! It's detected!");
            onHooDetected?.Invoke();
        }
        else
        {
            Debug.Log("인식된 텍스트: " + transcript);
        }
    }

    private void Update()
    {
        // 녹음 시간 제한 (예: 5초)
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
    /// 음성 인식 언어 코드를 설정합니다.
    /// </summary>
    /// <param name="languageCode">언어 코드 (예: "ko-KR")</param>
    public void SetLanguageCode(string languageCode)
    {
        currentLanguageCode = languageCode;
    }
}
