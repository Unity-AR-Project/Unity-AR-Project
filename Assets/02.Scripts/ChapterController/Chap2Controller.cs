using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class Chap2Controller : MonoBehaviour, IChapterController
{
    [SerializeField]
    private PlayableDirector playableDirector;

    private int maxBlowAttempts = 3; // �ִ� �õ� Ƚ��
    private int currentAttempt = 0;

    private SpeechToTextManager speechManager;

    private bool isBlowProcessStarted = false; // �߰�: �� �ұ� ���μ��� ���� ����
    private bool isPaused = false; // �Ͻ����� ���� ����

    //������ �ʱ�ȭ
    /*   [SerializeField] private GameObject chapter2Prefab; // é�� 7 ������
       [SerializeField] private Transform prefabParent; // �������� �ν��Ͻ�ȭ�� �θ� ������Ʈ
       private GameObject chapter2Instance; // ���� Ȱ��ȭ�� é�� 7 �ν��Ͻ�*/

    void OnEnable()
    {
        /* if (chapter2Instance != null)
         {
             Destroy(chapter2Instance);
         }

         // é�� 2 ������ �ν��Ͻ�ȭ
         if (chapter2Prefab != null && prefabParent != null)
         {
             chapter2Instance = Instantiate(chapter2Prefab, prefabParent);
             chapter2Instance.tag = "Chapter2Instance"; // �ʿ� �� �±� ����
             chapter2Instance.SetActive(true);
             Debug.Log("[chap2Controller] Chapter2 prefab instantiated.");
         }
         else
         {
             Debug.LogError("[chap2Controller] Chapter2Prefab or PrefabParent is not assigned.");
         }

         // Ÿ�Ӷ��� �ʱ� ����: ������� �ʰ� ��� ���·� ����
         if (playableDirector != null)
         {*/
        // ���� �ʱ�ȭ
        ResetState();

        // Ÿ�Ӷ��� ����
        playableDirector.time = 0; // Ÿ�Ӷ��� �ð� �ʱ�ȭ
        playableDirector.Stop();   // Ÿ�Ӷ��� ����

        // �ʱ� �޽��� ǥ��
        UIManager.instance.ShowMessage("2é�� ���۵Ǿ����ϴ�.\n " +
            "��ٷ��ּ���!");
        playableDirector.Play();   // Ÿ�Ӷ��� ���
        /*   }
           else
           {
               Debug.LogError("[chap2Controller] PlayableDirector not assigned.");
           }*/
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

            // ��� ���
            yield return new WaitForSeconds(1f);

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