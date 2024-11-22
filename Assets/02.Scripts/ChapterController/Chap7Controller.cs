using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections;
using UnityEngine.Events;

public class Chap7Controller : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector playableDirector;

    private int blowCount = 1;
    private int maxBlowAttempts = 3; // �ִ� �õ� Ƚ��
    private int currentAttempt = 0;

    private SpeechToTextManager speechManager;

    void Start()
    {
        speechManager = SpeechToTextManager.instance;
        if (speechManager == null)
        {
            Debug.LogError("SpeechToTextManager not found in the scene.");
            return;
        }

        // Ÿ�Ӷ��� ����
        playableDirector.Play();

        // Ÿ�Ӷ����� Ư�� �������� �̺�Ʈ Ʈ���Ÿ� �����ؾ� �մϴ�.
        // �̸� ���� Ÿ�Ӷ��ο� �ñ׳��� �����ϰ�, �ش� �ñ׳ο��� �Ʒ� �޼��带 ȣ���ϵ��� �����ϼ���.
        // ���÷� �ڷ�ƾ�� �����մϴ�.
        StartCoroutine(CheckForBlowSignal());
    }

    private IEnumerator CheckForBlowSignal()
    {
        // Ÿ�Ӷ����� Ư�� �������� ���
        yield return new WaitForSeconds(2f); // ���÷� 2�� ���

        // Ÿ�Ӷ��� �Ͻ�����
        playableDirector.Pause();

        // ���� �ν� ���μ��� ����
        //StartCoroutine(BlowWindProcess());
    }

    //private IEnumerator BlowWindProcess()
    //{
    //    bool success = false;

    //    while (currentAttempt < maxBlowAttempts && !success)
    //    {
    //        currentAttempt++;

    //        // UIManager�� ���� �޽��� ǥ��
    //        UIManager.instance.ShowMessage("�� �ϰ� �Ҿ��ּ���!");

    //        // ���� �ν� ����
    //        speechManager.onHooDetected.RemoveAllListeners();
    //        speechManager.onHooDetected.AddListener(() => success = true);
    //        speechManager.StartSpeechRecognition();

    //        // ���� �ð� ���� ���
    //        yield return new WaitForSeconds(speechManager.GetRecordingLength());
    //        // ���� �ν� ����
    //        speechManager.StopSpeechRecognition();

    //        // ��� Ȯ��
    //        if (success)
    //        {
    //            // ���� �޽��� ǥ��
    //            UIManager.instance.ShowMessage("���߾��!");

    //            // ��� ���
    //            yield return new WaitForSeconds(1f);

    //            // Ÿ�Ӷ��� ���
    //            playableDirector.Play();
    //        }
    //        else
    //        {
    //            if (currentAttempt < maxBlowAttempts)
    //            {
    //                // ��õ� �޽��� ǥ��
    //                UIManager.instance.ShowMessage("�ٽ� �� �� �Ҿ��ּ���!");
    //            }
    //            else
    //            {
    //                // ���� �޽��� ǥ��
    //                UIManager.instance.ShowMessage("�ƽ��׿�~");

    //                // ��� ���
    //                yield return new WaitForSeconds(1f);

    //                // Ÿ�Ӷ��� ���
    //                playableDirector.Play();
    //            }

    //            // ��� ���
    //            yield return new WaitForSeconds(1f);
    //        }
    //    }
    //}
}