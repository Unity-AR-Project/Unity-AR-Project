using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;
using System.Collections;

public class Chap7Controller : MonoBehaviour, IChapterController
{
    [SerializeField]
    private PlayableDirector playableDirector;

    private int maxBlowAttempts = 3; // 최대 시도 횟수
    private int currentAttempt = 0;

    private SpeechToTextManager speechManager;

    private bool isBlowProcessStarted = false; // 추가: 후 불기 프로세스 시작 여부
    private bool isPaused = false; // 일시정지 상태 여부


    void OnEnable()
    {
        // 상태 초기화
        ResetState();

        // 타임라인 시작
        playableDirector.time = 0; // 타임라인 시간 초기화
        playableDirector.Stop();   // 타임라인 정지
        playableDirector.Play();   // 타임라인 재생
    }

    /// <summary>
    /// 상태를 초기화하는 메서드
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

    // 시그널에 의해 호출될 메서드
    public void OnBlowSignalReceived()
    {
        if (isBlowProcessStarted)
            return; // 이미 프로세스가 시작된 경우 중복 실행 방지

        isBlowProcessStarted = true;

        Debug.Log("OnBlowSignalReceived");
        // 타임라인 일시정지
        playableDirector.Pause();

        // 음성 인식 프로세스 시작
        StartCoroutine(BlowWindProcess());
    }

    private IEnumerator BlowWindProcess()
    {
        bool success = false;

        while (currentAttempt < maxBlowAttempts && !success)
        {
            currentAttempt++;

            // UIManager를 통해 메시지 표시
            UIManager.instance.ShowMessage("후 하고 불어주세요!");

            // 음성 인식 이벤트 리스너 설정
            UnityAction hooListener = () => success = true;
            speechManager.onHooDetected.AddListener(hooListener);

            // 음성 인식 시작
            speechManager.StartSpeechRecognition();

            // 녹음 시간 동안 대기
            yield return new WaitForSeconds(speechManager.GetRecordingLength());

            // 음성 인식 중지
            speechManager.StopSpeechRecognition();

            // 이벤트 리스너 제거
            speechManager.onHooDetected.RemoveListener(hooListener);

            // 결과 확인
            if (success)
            {
                Debug.Log("Blow Success");
                // 성공 메시지 표시
                UIManager.instance.ShowMessage("잘했어요!");

                // 잠시 대기
                yield return new WaitForSeconds(1f);

                // 타임라인 재생
                playableDirector.Play();
            }
            else
            {
                if (currentAttempt < maxBlowAttempts)
                {
                    // 재시도 메시지 표시
                    UIManager.instance.ShowMessage("다시 한 번 불어주세요!");
                }
                else
                {
                    // 실패 메시지 표시
                    UIManager.instance.ShowMessage("아쉽네요~");

                    // 잠시 대기
                    yield return new WaitForSeconds(1f);

                    // 타임라인 재생
                    playableDirector.Play();
                }

                // 잠시 대기
                yield return new WaitForSeconds(1f);
            }
        }
    }

    /// <summary>
    /// 타임라인 일시정지/재개 토글
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
