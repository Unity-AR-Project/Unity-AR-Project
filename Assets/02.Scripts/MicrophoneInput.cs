using UnityEngine;

/// <summary>
/// 마이크 입력을 관리하는 싱글톤 클래스
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
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 마이크 녹음을 시작합니다.
    /// </summary>
    /// <param name="maxDuration">최대 녹음 시간 (초)</param>
    /// <param name="sampleRate">샘플링 레이트</param>
    /// <returns>녹음된 오디오 클립</returns>
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
    /// 마이크 녹음을 중지합니다.
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
    /// 현재 녹음 시간을 반환합니다.
    /// </summary>
    /// <returns>녹음 시간 (초)</returns>
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
