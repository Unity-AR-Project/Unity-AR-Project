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
    private int maxBlowAttempts = 3; // 최대 시도 횟수
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

        // 타임라인 시작
        playableDirector.Play();

        // 타임라인의 특정 지점에서 이벤트 트리거를 설정해야 합니다.
        // 이를 위해 타임라인에 시그널을 설정하고, 해당 시그널에서 아래 메서드를 호출하도록 설정하세요.
        // 예시로 코루틴을 시작합니다.
        StartCoroutine(CheckForBlowSignal());
    }

    private IEnumerator CheckForBlowSignal()
    {
        // 타임라인의 특정 지점까지 대기
        yield return new WaitForSeconds(2f); // 예시로 2초 대기

        // 타임라인 일시정지
        playableDirector.Pause();

        // 음성 인식 프로세스 시작
        //StartCoroutine(BlowWindProcess());
    }

    //private IEnumerator BlowWindProcess()
    //{
    //    bool success = false;

    //    while (currentAttempt < maxBlowAttempts && !success)
    //    {
    //        currentAttempt++;

    //        // UIManager를 통해 메시지 표시
    //        UIManager.instance.ShowMessage("후 하고 불어주세요!");

    //        // 음성 인식 시작
    //        speechManager.onHooDetected.RemoveAllListeners();
    //        speechManager.onHooDetected.AddListener(() => success = true);
    //        speechManager.StartSpeechRecognition();

    //        // 녹음 시간 동안 대기
    //        yield return new WaitForSeconds(speechManager.GetRecordingLength());
    //        // 음성 인식 중지
    //        speechManager.StopSpeechRecognition();

    //        // 결과 확인
    //        if (success)
    //        {
    //            // 성공 메시지 표시
    //            UIManager.instance.ShowMessage("잘했어요!");

    //            // 잠시 대기
    //            yield return new WaitForSeconds(1f);

    //            // 타임라인 재생
    //            playableDirector.Play();
    //        }
    //        else
    //        {
    //            if (currentAttempt < maxBlowAttempts)
    //            {
    //                // 재시도 메시지 표시
    //                UIManager.instance.ShowMessage("다시 한 번 불어주세요!");
    //            }
    //            else
    //            {
    //                // 실패 메시지 표시
    //                UIManager.instance.ShowMessage("아쉽네요~");

    //                // 잠시 대기
    //                yield return new WaitForSeconds(1f);

    //                // 타임라인 재생
    //                playableDirector.Play();
    //            }

    //            // 잠시 대기
    //            yield return new WaitForSeconds(1f);
    //        }
    //    }
    //}
}