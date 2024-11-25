using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;
using System.Collections;

public class Chap7Controller : MonoBehaviour, IChapterController
{
    [SerializeField]
    private PlayableDirector playableDirector;

    private int maxBlowAttempts = 3; // �ִ� �õ� Ƚ��
    private int currentAttempt = 0;

    private SpeechToTextManager speechManager;

    private bool isBlowProcessStarted = false; // �߰�: �� �ұ� ���μ��� ���� ����
    private bool isPaused = false; // �Ͻ����� ���� ����


    void OnEnable()
    {
        // ���� �ʱ�ȭ
        ResetState();

        // Ÿ�Ӷ��� ����
        playableDirector.time = 0; // Ÿ�Ӷ��� �ð� �ʱ�ȭ
        playableDirector.Stop();   // Ÿ�Ӷ��� ����
        playableDirector.Play();   // Ÿ�Ӷ��� ���
    }

    /// <summary>
    /// ���¸� �ʱ�ȭ�ϴ� �޼���
    /// </summary>
    private void ResetState()
    {
        currentAttempt = 0;
        isBlowProcessStarted = false;
    }

    void Start()
    {
        speechManager = SpeechToTextManager.instance;
        if (speechManager == null)
        {
            Debug.LogError("SpeechToTextManager not found in the scene.");
            return;
        }
    }

    // �ñ׳ο� ���� ȣ��� �޼���
    public void OnBlowSignalReceived()
    {
        if (isBlowProcessStarted)
            return; // �̹� ���μ����� ���۵� ��� �ߺ� ���� ����

        isBlowProcessStarted = true;

        Debug.Log("OnBlowSignalReceived");
        // Ÿ�Ӷ��� �Ͻ�����
        playableDirector.Pause();

        // ���� �ν� ���μ��� ����
        StartCoroutine(BlowWindProcess());
    }

    private IEnumerator BlowWindProcess()
    {
        bool success = false;

        while (currentAttempt < maxBlowAttempts && !success)
        {
            currentAttempt++;

            // UIManager�� ���� �޽��� ǥ��
            UIManager.instance.ShowMessage("�� �ϰ� �Ҿ��ּ���!");

            // ���� �ν� �̺�Ʈ ������ ����
            UnityAction hooListener = () => success = true;
            speechManager.onHooDetected.AddListener(hooListener);

            // ���� �ν� ����
            speechManager.StartSpeechRecognition();

            // ���� �ð� ���� ���
            yield return new WaitForSeconds(speechManager.GetRecordingLength());

            // ���� �ν� ����
            speechManager.StopSpeechRecognition();

            // �̺�Ʈ ������ ����
            speechManager.onHooDetected.RemoveListener(hooListener);

            // ��� Ȯ��
            if (success)
            {
                Debug.Log("Blow Success");
                // ���� �޽��� ǥ��
                UIManager.instance.ShowMessage("���߾��!");

                // ��� ���
                yield return new WaitForSeconds(1f);

                // Ÿ�Ӷ��� ���
                playableDirector.Play();
            }
            else
            {
                if (currentAttempt < maxBlowAttempts)
                {
                    // ��õ� �޽��� ǥ��
                    UIManager.instance.ShowMessage("�ٽ� �� �� �Ҿ��ּ���!");
                }
                else
                {
                    // ���� �޽��� ǥ��
                    UIManager.instance.ShowMessage("�ƽ��׿�~");

                    // ��� ���
                    yield return new WaitForSeconds(1f);

                    // Ÿ�Ӷ��� ���
                    playableDirector.Play();
                }

                // ��� ���
                yield return new WaitForSeconds(1f);
            }
        }
    }

    /// <summary>
    /// Ÿ�Ӷ��� �Ͻ�����/�簳 ���
    /// </summary>
    public void TogglePause()
    {
        if (isPaused)
        {
            playableDirector.Play();
            isPaused = false;
        }
        else
        {
            playableDirector.Pause();
            isPaused = true;
        }
    }

}
