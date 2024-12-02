using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class Chap4Controller : MonoBehaviour, IChapterController
{
    public PlayableDirector playableDirector; // 타임라인 제어
    public TextMeshProUGUI uiText; // UI 텍스트

    private int touchCount = 0; // 터치 횟수
    private bool isPaused = false; // 타임라인 멈춤 상태

    private const double PauseTime = 13.8; // 타임라인 멈출 시간
    public LayerMask groundLayer; // Ground 레이어

    /*//프리팹 초기화
    [SerializeField] private GameObject chapter4Prefab; // 챕터 7 프리팹
    [SerializeField] private Transform prefabParent; // 프리팹을 인스턴스화할 부모 오브젝트
    private GameObject chapter4Instance; // 현재 활성화된 챕터 7 인스턴스
    */

    private void OnEnable()
    {
        /* if (chapter4Instance != null)
         {
             Destroy(chapter4Instance);
         }

         // 챕터 4 프리팹 인스턴스화
         if (chapter4Prefab != null && prefabParent != null)
         {
             chapter4Instance = Instantiate(chapter4Prefab, prefabParent);
             chapter4Instance.tag = "Chapter1Instance"; // 필요 시 태그 설정
             chapter4Instance.SetActive(true);
             Debug.Log("[chap4Controller] Chapter4 prefab instantiated.");
         }
         else
         {
             Debug.LogError("[chap4Controller] Chapter1Prefab or PrefabParent is not assigned.");
         }

         // 타임라인 초기 설정: 재생하지 않고 대기 상태로 설정
         if (playableDirector != null)
         {
        */

        // 타임라인 시작
        playableDirector.time = 0; // 타임라인 시간 초기화
        playableDirector.Stop();   // 타임라인 정지
                                   // 초기 메시지 표시
        UIManager.instance.ShowMessage("4챕터 시작되었습니다.\n " +
            "기다려주세요!");
        playableDirector.Play();   // 타임라인 재생
        /*  }
          else
          {
              Debug.LogError("[chap1Controller] PlayableDirector not assigned.");
          }*/

        AudioListener.pause = false;
    }

    private void Start()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped += OnPlayableDirectorStopped;
            Invoke(nameof(PauseTimelineAtSpecificTime), (float)PauseTime); // 타임라인 멈춤 예약
        }


        if (uiText != null)
        {
            uiText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (isPaused && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                if (hit.collider.gameObject.name == "Wood house")
                {
                    touchCount++;
                    Debug.LogWarning($"[Debug] : Wood house touched {touchCount} times!");

                    if (touchCount == 3)
                    {
                        ResumeTimeline();
                    }
                }
            }
        }
    }

    public void PauseTimelineAtSpecificTime()
    {
        if (playableDirector != null)
        {
            playableDirector.Pause();
            isPaused = true;

            if (uiText != null)
            {
                UIManager.instance.ShowMessage("나무집을 세 번 터치 해주세요");
            }

            Debug.LogWarning("[Debug] : Timeline paused at 13.80 seconds.");
        }
    }


    private void ResumeTimeline()
    {
        if (playableDirector != null)
        {
            // UI 메시지 표시
            UIManager.instance.ShowMessage("잘했어요!");
            playableDirector.Play();
            isPaused = false;
            AudioListener.pause = false;
        }


        if (uiText != null)
        {
            uiText.gameObject.SetActive(false);
        }

        touchCount = 0;
        Debug.LogWarning("[Debug] : Timeline and audio resumed.");
    }

    private void OnPlayableDirectorStopped(PlayableDirector director)
    {
        if (director == playableDirector)
        {
            Debug.LogWarning("[Debug] : Timeline has ended!");
        }
    }


    public void PauseAudio()
    {
        AudioListener.pause = true;
    }

    /// <summary>
    /// 타임라인 일시정지/재개 토글 (화면 터치 시 동작)
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