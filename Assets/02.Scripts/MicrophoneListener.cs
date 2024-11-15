using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; // UI 텍스트 기능을 사용하기 위한 네임스페이스 추가
#if UNITY_ANDROID
using UnityEngine.Android; // Android 권한 요청을 위해 필요
#endif

public class MicrophoneListener : MonoBehaviour
{
    [SerializeField] private Image _imageSound; // 마이크 소리 크기를 시각화하는 이미지
    [SerializeField] private Text loudnessText; // 텍스트 UI 요소를 연결할 변수 추가 (소리 크기 표시)
    public float sensitivity = 100; // 소리 크기를 조절하는 감도
    public float loudness = 0; // 현재 소리 크기 (볼륨) 저장
    public float pitch = 0; // 현재 소리의 음정 (주파수) 저장
    private AudioSource _audio; // 오디오 소스 컴포넌트 참조 변수

    public float RmsValue; // RMS 값 저장 (소리의 크기를 나타냄)
    public float DbValue; // 데시벨 값 저장 (볼륨의 로그 값)
    public float PitchValue; // 음정 (주파수) 값 저장

    private const int QSamples = 1024; // FFT 샘플 수, 음성 분석의 정확도를 결정
    private const float RefValue = 0.1f; // 기준 데시벨 값, 데시벨 계산에 사용
    private const float Threshold = 0.02f; // 주파수 스펙트럼의 최소 임계값

    private float[] _samples; // 오디오 샘플 데이터를 저장할 배열
    private float[] _spectrum; // 주파수 스펙트럼 데이터를 저장할 배열
    private float _fSample; // 샘플링 주파수 값 저장

    public bool startMicOnStartup = true; // 시작할 때 마이크를 자동으로 활성화할지 여부
    public bool stopMicrophoneListener = false; // 마이크 리스너를 중지하는 플래그
    public bool startMicrophoneListener = false; // 마이크 리스너를 시작하는 플래그

    private bool microphoneListenerOn = false; // 마이크 리스너의 현재 상태를 저장
    public bool disableOutputSound = false; // 소리 출력 비활성화 플래그

    private AudioSource src; // 오디오 소스 컴포넌트를 위한 변수
    public AudioMixer masterMixer; // 오디오 믹서를 제어하기 위한 변수

    private float timeSinceRestart = 0; // 마이크 리스너가 마지막으로 재시작된 시점 기록

    void Start()
    {
        // 안드로이드에서 마이크 권한을 확인하고 요청하는 부분
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone); // 마이크 권한 요청
        }
        else
        {
            // 권한이 이미 허용된 경우 마이크 리스너 시작
            RestartMicrophoneListener();
            StartMicrophoneListener();
        }
#endif

        if (startMicOnStartup) // 시작할 때 마이크를 자동 활성화할 경우
        {
            RestartMicrophoneListener(); // 마이크 리스너 초기화
            StartMicrophoneListener(); // 마이크 리스너 시작

            _audio = GetComponent<AudioSource>(); // AudioSource 컴포넌트를 가져옴
            _audio.clip = Microphone.Start(null, true, 10, 44100); // 마이크 입력을 10초 길이로 설정
            _audio.loop = true; // 오디오 소스를 반복 재생
            while (!(Microphone.GetPosition(null) > 0)) { } // 마이크가 시작되기를 기다림
            _audio.Play(); // 오디오 소스 재생 시작
            _samples = new float[QSamples]; // 오디오 샘플 데이터를 저장할 배열 초기화
            _spectrum = new float[QSamples]; // 주파수 스펙트럼 데이터를 저장할 배열 초기화
            _fSample = AudioSettings.outputSampleRate; // 샘플링 주파수를 시스템에서 가져옴

            Debug.LogWarning("[debug] : Microphone started successfully."); // 마이크 시작 여부 디버그 출력
        }
    }

    void Update()
    {
        if (stopMicrophoneListener) // 마이크 리스너 중지 플래그가 활성화된 경우
        {
            StopMicrophoneListener(); // 마이크 리스너 중지
            stopMicrophoneListener = false; // 플래그 초기화
        }
        if (startMicrophoneListener) // 마이크 리스너 시작 플래그가 활성화된 경우
        {
            StartMicrophoneListener(); // 마이크 리스너 시작
            startMicrophoneListener = false; // 플래그 초기화
        }

        MicrophoneIntoAudioSource(microphoneListenerOn); // 마이크를 AudioSource에 연결
        DisableSound(!disableOutputSound); // 소리 출력을 활성화 또는 비활성화

        loudness = GetAveragedVolume() * sensitivity; // 볼륨 값을 가져와 감도 조절
        UpdateLoudnessText(); // UI 텍스트 업데이트

        // UI 업데이트 부분
        UpdateImageSound();

        Debug.LogWarning("[debug] : Current Loudness: " + loudness); // 볼륨 값 디버그로 출력
    }

    void UpdateImageSound()
    {
        // 소리가 특정 크기 이상일 경우 이미지 채움 비율을 부드럽게 변경
        if (loudness > 5f) // 소리가 특정 크기 이상일 경우
            _imageSound.fillAmount = Mathf.Lerp(_imageSound.fillAmount, 1f, Time.deltaTime * 5f); // 부드럽게 채움
        else
            _imageSound.fillAmount = Mathf.Lerp(_imageSound.fillAmount, 0.65f, Time.deltaTime * 5f); // 부드럽게 감소
    }

    void UpdateLoudnessText()
    {
        // 소리 크기 값 (loudness)을 화면에 표시
        if (loudnessText != null)
        {
            loudnessText.text = "Loudness: " + loudness.ToString("F2"); // 소리 크기를 소수점 2자리로 표시
        }
    }

    float GetAveragedVolume()
    {
        float[] data = new float[256]; // 256 샘플 데이터를 저장할 배열 생성
        float a = 0; // 볼륨 합계를 위한 변수
        _audio.GetOutputData(data, 0); // 오디오 소스의 출력 데이터를 가져옴
        foreach (float s in data) // 각 샘플 값을 순회
        {
            a += Mathf.Abs(s); // 각 샘플의 절대값을 더함
        }
        return a / 256; // 평균 볼륨을 반환
    }

    void MicrophoneIntoAudioSource(bool isMicOn)
    {
        if (!isMicOn) return; // 마이크가 활성화되지 않은 경우 종료
        _audio = GetComponent<AudioSource>();
        if (_audio.clip == null) return;

        int micPosition = Microphone.GetPosition(null); // 마이크 위치를 가져옴
        if (micPosition <= 0) return; // 마이크가 제대로 작동하지 않으면 종료

        float[] samples = new float[micPosition]; // 마이크 샘플 데이터를 저장할 배열 생성
        _audio.clip.GetData(samples, 0); // 마이크 데이터를 가져옴

        Debug.LogWarning("[debug] : Microphone is active and capturing audio."); // 마이크 활성 상태 디버그로 출력
    }

    void StopMicrophoneListener()
    {
        microphoneListenerOn = false; // 마이크 리스너 중지
        Microphone.End(null); // 마이크 종료
        Debug.LogWarning("[debug] : Microphone stopped."); // 마이크 종료 디버그로 출력
    }

    void StartMicrophoneListener()
    {
        microphoneListenerOn = true; // 마이크 리스너 시작
        _audio.loop = true; // 오디오 소스 반복 설정
        Debug.LogWarning("[debug] : Microphone listener started."); // 마이크 리스너 시작 디버그로 출력
    }

    void DisableSound(bool disable)
    {
        if (disable) // 소리 비활성화
        {
            _audio.volume = 0;
        }
        else // 소리 활성화
        {
            _audio.volume = 1;
        }
    }

    void RestartMicrophoneListener()
    {
        StopMicrophoneListener(); // 기존 마이크 리스너 중지
        startMicrophoneListener = true; // 리스너 시작 플래그 활성화
        Debug.LogWarning("[debug] : Restarting microphone listener."); // 마이크 리스너 재시작 디버그로 출력
    }
}
