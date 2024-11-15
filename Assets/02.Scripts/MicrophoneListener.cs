using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; // UI �ؽ�Ʈ ����� ����ϱ� ���� ���ӽ����̽� �߰�
#if UNITY_ANDROID
using UnityEngine.Android; // Android ���� ��û�� ���� �ʿ�
#endif

public class MicrophoneListener : MonoBehaviour
{
    [SerializeField] private Image _imageSound; // ����ũ �Ҹ� ũ�⸦ �ð�ȭ�ϴ� �̹���
    [SerializeField] private Text loudnessText; // �ؽ�Ʈ UI ��Ҹ� ������ ���� �߰� (�Ҹ� ũ�� ǥ��)
    public float sensitivity = 100; // �Ҹ� ũ�⸦ �����ϴ� ����
    public float loudness = 0; // ���� �Ҹ� ũ�� (����) ����
    public float pitch = 0; // ���� �Ҹ��� ���� (���ļ�) ����
    private AudioSource _audio; // ����� �ҽ� ������Ʈ ���� ����

    public float RmsValue; // RMS �� ���� (�Ҹ��� ũ�⸦ ��Ÿ��)
    public float DbValue; // ���ú� �� ���� (������ �α� ��)
    public float PitchValue; // ���� (���ļ�) �� ����

    private const int QSamples = 1024; // FFT ���� ��, ���� �м��� ��Ȯ���� ����
    private const float RefValue = 0.1f; // ���� ���ú� ��, ���ú� ��꿡 ���
    private const float Threshold = 0.02f; // ���ļ� ����Ʈ���� �ּ� �Ӱ谪

    private float[] _samples; // ����� ���� �����͸� ������ �迭
    private float[] _spectrum; // ���ļ� ����Ʈ�� �����͸� ������ �迭
    private float _fSample; // ���ø� ���ļ� �� ����

    public bool startMicOnStartup = true; // ������ �� ����ũ�� �ڵ����� Ȱ��ȭ���� ����
    public bool stopMicrophoneListener = false; // ����ũ �����ʸ� �����ϴ� �÷���
    public bool startMicrophoneListener = false; // ����ũ �����ʸ� �����ϴ� �÷���

    private bool microphoneListenerOn = false; // ����ũ �������� ���� ���¸� ����
    public bool disableOutputSound = false; // �Ҹ� ��� ��Ȱ��ȭ �÷���

    private AudioSource src; // ����� �ҽ� ������Ʈ�� ���� ����
    public AudioMixer masterMixer; // ����� �ͼ��� �����ϱ� ���� ����

    private float timeSinceRestart = 0; // ����ũ �����ʰ� ���������� ����۵� ���� ���

    void Start()
    {
        // �ȵ���̵忡�� ����ũ ������ Ȯ���ϰ� ��û�ϴ� �κ�
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone); // ����ũ ���� ��û
        }
        else
        {
            // ������ �̹� ���� ��� ����ũ ������ ����
            RestartMicrophoneListener();
            StartMicrophoneListener();
        }
#endif

        if (startMicOnStartup) // ������ �� ����ũ�� �ڵ� Ȱ��ȭ�� ���
        {
            RestartMicrophoneListener(); // ����ũ ������ �ʱ�ȭ
            StartMicrophoneListener(); // ����ũ ������ ����

            _audio = GetComponent<AudioSource>(); // AudioSource ������Ʈ�� ������
            _audio.clip = Microphone.Start(null, true, 10, 44100); // ����ũ �Է��� 10�� ���̷� ����
            _audio.loop = true; // ����� �ҽ��� �ݺ� ���
            while (!(Microphone.GetPosition(null) > 0)) { } // ����ũ�� ���۵Ǳ⸦ ��ٸ�
            _audio.Play(); // ����� �ҽ� ��� ����
            _samples = new float[QSamples]; // ����� ���� �����͸� ������ �迭 �ʱ�ȭ
            _spectrum = new float[QSamples]; // ���ļ� ����Ʈ�� �����͸� ������ �迭 �ʱ�ȭ
            _fSample = AudioSettings.outputSampleRate; // ���ø� ���ļ��� �ý��ۿ��� ������

            Debug.LogWarning("[debug] : Microphone started successfully."); // ����ũ ���� ���� ����� ���
        }
    }

    void Update()
    {
        if (stopMicrophoneListener) // ����ũ ������ ���� �÷��װ� Ȱ��ȭ�� ���
        {
            StopMicrophoneListener(); // ����ũ ������ ����
            stopMicrophoneListener = false; // �÷��� �ʱ�ȭ
        }
        if (startMicrophoneListener) // ����ũ ������ ���� �÷��װ� Ȱ��ȭ�� ���
        {
            StartMicrophoneListener(); // ����ũ ������ ����
            startMicrophoneListener = false; // �÷��� �ʱ�ȭ
        }

        MicrophoneIntoAudioSource(microphoneListenerOn); // ����ũ�� AudioSource�� ����
        DisableSound(!disableOutputSound); // �Ҹ� ����� Ȱ��ȭ �Ǵ� ��Ȱ��ȭ

        loudness = GetAveragedVolume() * sensitivity; // ���� ���� ������ ���� ����
        UpdateLoudnessText(); // UI �ؽ�Ʈ ������Ʈ

        // UI ������Ʈ �κ�
        UpdateImageSound();

        Debug.LogWarning("[debug] : Current Loudness: " + loudness); // ���� �� ����׷� ���
    }

    void UpdateImageSound()
    {
        // �Ҹ��� Ư�� ũ�� �̻��� ��� �̹��� ä�� ������ �ε巴�� ����
        if (loudness > 5f) // �Ҹ��� Ư�� ũ�� �̻��� ���
            _imageSound.fillAmount = Mathf.Lerp(_imageSound.fillAmount, 1f, Time.deltaTime * 5f); // �ε巴�� ä��
        else
            _imageSound.fillAmount = Mathf.Lerp(_imageSound.fillAmount, 0.65f, Time.deltaTime * 5f); // �ε巴�� ����
    }

    void UpdateLoudnessText()
    {
        // �Ҹ� ũ�� �� (loudness)�� ȭ�鿡 ǥ��
        if (loudnessText != null)
        {
            loudnessText.text = "Loudness: " + loudness.ToString("F2"); // �Ҹ� ũ�⸦ �Ҽ��� 2�ڸ��� ǥ��
        }
    }

    float GetAveragedVolume()
    {
        float[] data = new float[256]; // 256 ���� �����͸� ������ �迭 ����
        float a = 0; // ���� �հ踦 ���� ����
        _audio.GetOutputData(data, 0); // ����� �ҽ��� ��� �����͸� ������
        foreach (float s in data) // �� ���� ���� ��ȸ
        {
            a += Mathf.Abs(s); // �� ������ ���밪�� ����
        }
        return a / 256; // ��� ������ ��ȯ
    }

    void MicrophoneIntoAudioSource(bool isMicOn)
    {
        if (!isMicOn) return; // ����ũ�� Ȱ��ȭ���� ���� ��� ����
        _audio = GetComponent<AudioSource>();
        if (_audio.clip == null) return;

        int micPosition = Microphone.GetPosition(null); // ����ũ ��ġ�� ������
        if (micPosition <= 0) return; // ����ũ�� ����� �۵����� ������ ����

        float[] samples = new float[micPosition]; // ����ũ ���� �����͸� ������ �迭 ����
        _audio.clip.GetData(samples, 0); // ����ũ �����͸� ������

        Debug.LogWarning("[debug] : Microphone is active and capturing audio."); // ����ũ Ȱ�� ���� ����׷� ���
    }

    void StopMicrophoneListener()
    {
        microphoneListenerOn = false; // ����ũ ������ ����
        Microphone.End(null); // ����ũ ����
        Debug.LogWarning("[debug] : Microphone stopped."); // ����ũ ���� ����׷� ���
    }

    void StartMicrophoneListener()
    {
        microphoneListenerOn = true; // ����ũ ������ ����
        _audio.loop = true; // ����� �ҽ� �ݺ� ����
        Debug.LogWarning("[debug] : Microphone listener started."); // ����ũ ������ ���� ����׷� ���
    }

    void DisableSound(bool disable)
    {
        if (disable) // �Ҹ� ��Ȱ��ȭ
        {
            _audio.volume = 0;
        }
        else // �Ҹ� Ȱ��ȭ
        {
            _audio.volume = 1;
        }
    }

    void RestartMicrophoneListener()
    {
        StopMicrophoneListener(); // ���� ����ũ ������ ����
        startMicrophoneListener = true; // ������ ���� �÷��� Ȱ��ȭ
        Debug.LogWarning("[debug] : Restarting microphone listener."); // ����ũ ������ ����� ����׷� ���
    }
}
