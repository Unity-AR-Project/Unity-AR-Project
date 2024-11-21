using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Chap4Controller : MonoBehaviour
{
    public PlayableDirector playableDirector; // 타임라인 제어를 위한 PlayableDirector
    public GameObject uiText; // UI 텍스트 오브젝트 (안내 메시지)

    private int touchCount = 0; // 현재 터치 횟수
    private bool isPaused = false; // 타임라인 멈춤 상태를 추적하는 플래그

    private const double PauseTime = 13.8; // 타임라인 멈출 시간 (13.80초)
    public LayerMask groundLayer; // Ground 레이어를 지정 (레이캐스트가 충돌할 레이어)

    private void Start()
    {
        // PlayableDirector가 있다면 타임라인 종료 시 이벤트를 등록
        if (playableDirector != null)
        {
            playableDirector.stopped += OnPlayableDirectorStopped;

            // 타임라인이 시작되면 일정 시간이 지난 후 자동으로 멈추게 설정
            Invoke(nameof(PauseTimelineAtSpecificTime), (float)PauseTime);
        }

        // UI 텍스트를 시작 시 비활성화
        if (uiText != null)
        {
            uiText.SetActive(false);
        }
    }

    private void Update()
    {
        // 타임라인이 멈춰 있을 때만 터치 입력을 처리
        if (isPaused && Input.GetMouseButtonDown(0)) // 마우스 클릭 (터치 대체 가능)
        {
            // 화면의 터치 위치에서 광선을 쏘아 충돌을 확인
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast로 터치된 물체가 Ground 레이어에 있는지 확인
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                // 터치된 오브젝트의 이름이 "Wood house"인 경우
                if (hit.collider.gameObject.name == "Wood house")
                {
                    touchCount++; // 터치 횟수 증가
                    Debug.LogWarning($"[Debug] : Wood house touched {touchCount} times!");

                    // 터치 횟수가 3 이상이면 타임라인을 재개
                    if (touchCount >= 3)
                    {
                        ResumeTimeline(); // 타임라인 재개
                    }
                }
            }
        }
    }

    // 타임라인을 특정 시간에서 멈추고 UI 텍스트 표시
    private void PauseTimelineAtSpecificTime()
    {
        if (playableDirector != null)
        {
            playableDirector.time = PauseTime; // 타임라인을 3.50초로 이동
            playableDirector.Pause(); // 타임라인 멈춤
            isPaused = true; // 멈춤 상태 플래그 설정

            if (uiText != null)
            {
                uiText.SetActive(true); // UI 텍스트 활성화
                uiText.GetComponent<Text>().text = "Touch the wood house 3 times to continue!"; // 안내 메시지 설정
            }

            Debug.LogWarning("[Debug] : Timeline paused at 3.50 seconds.");
        }
    }

    // 타임라인을 재개하고 UI 텍스트 비활성화
    private void ResumeTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.Play(); // 타임라인 재개
            isPaused = false; // 멈춤 상태 플래그 해제

            if (uiText != null)
            {
                uiText.SetActive(false); // UI 텍스트 비활성화
            }

            touchCount = 0; // 터치 횟수 초기화
            Debug.LogWarning("[Debug] : Timeline resumed.");
        }
    }

    // 타임라인이 종료되었을 때 호출되는 메서드
    private void OnPlayableDirectorStopped(PlayableDirector director)
    {
        if (director == playableDirector)
        {
            Debug.LogWarning("[Debug] : Timeline has ended!");
            // 필요한 추가 로직 작성
        }
    }
}
