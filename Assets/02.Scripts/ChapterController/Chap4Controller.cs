using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class Chap4Controller : MonoBehaviour, IChapterController
{
    public PlayableDirector playableDirector; // 타임라인 제어를 위한 PlayableDirector
    public TextMeshProUGUI uiText; // UI 텍스트 오브젝트 (안내 메시지)

    private int touchCount = 0; // 현재 터치 횟수
    private bool isPaused = false; // 타임라인 멈춤 상태를 추적하는 플래그

    private const double PauseTime = 13.8; // 타임라인 멈출 시간 (13.80초)
    public LayerMask groundLayer; // Ground 레이어를 지정 (레이캐스트가 충돌할 레이어)

    private AudioSource audioSource;   // 재생 중인 AudioSource

    /*//프리팹 초기화
    [SerializeField] private GameObject chapter4Prefab; // 챕터 7 프리팹
    [SerializeField] private Transform prefabParent; // 프리팹을 인스턴스화할 부모 오브젝트
    private GameObject chapter4Instance; // 현재 활성화된 챕터 7 인스턴스
*/
    void OnEnable()
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
            playableDirector.Play();   // 타임라인 재생
      /*  }
        else
        {
            Debug.LogError("[chap1Controller] PlayableDirector not assigned.");
        }*/
    }


    private void Start()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped += OnPlayableDirectorStopped;
            Invoke(nameof(PauseTimelineAtSpecificTime), (float)PauseTime);
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
                //uiText.gameObject.SetActive(true);
                // uiText.text = "Touch the wood house 3 times to continue!";
                // UI 메시지 표시
                UIManager.instance.ShowMessage("나무집을 세 번 터치 해주세요");
            }

            Debug.LogWarning("[Debug] : Timeline paused at 13.80 seconds.");
        }
    }

    private void ResumeTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.Play();
            isPaused = false;
            audioSource.UnPause();

            if (uiText != null)
            {
                uiText.gameObject.SetActive(false);
            }

            touchCount = 0;
            Debug.LogWarning("[Debug] : Timeline resumed.");
        }
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
        audioSource.Pause();
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
