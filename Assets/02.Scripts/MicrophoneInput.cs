using UnityEngine;

/// <summary>
/// ����ũ �Է��� �����ϴ� �̱��� Ŭ����
/// </summary>
public class MicrophoneInput : MonoBehaviour
{
    public static MicrophoneInput instance { get; private set; }

    private string microphoneDevice;
    private float recordingStartTime;

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

    /// <summary>
    /// ����ũ ������ �����մϴ�.
    /// </summary>
    /// <param name="maxDuration">�ִ� ���� �ð� (��)</param>
    /// <param name="sampleRate">���ø� ����Ʈ</param>
    /// <returns>������ ����� Ŭ��</returns>
    public AudioClip StartRecording(int maxDuration = 5, int sampleRate = 16000)
    {
        if (Microphone.devices.Length > 0)
        {
            microphoneDevice = Microphone.devices[0];
            recordingStartTime = Time.time;
            return Microphone.Start(microphoneDevice, false, maxDuration, sampleRate);
        }
        else
        {
            Debug.LogError("Microphone not found.");
            return null;
        }
    }

    /// <summary>
    /// ����ũ ������ �����մϴ�.
    /// </summary>
    public void StopRecording()
    {
        if (microphoneDevice != null)
        {
            Microphone.End(microphoneDevice);
            microphoneDevice = null;
        }
    }

    /// <summary>
    /// ���� ���� �ð��� ��ȯ�մϴ�.
    /// </summary>
    /// <returns>���� �ð� (��)</returns>
    public float GetRecordingTime()
    {
        if (microphoneDevice != null)
        {
            return Time.time - recordingStartTime;
        }
        else
        {
            return 0f;
        }
    }
}
