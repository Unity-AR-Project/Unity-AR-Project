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

        AudioListener.pause = false;
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
                uiText.gameObject.SetActive(true);
                uiText.text = "Touch the wood house 3 times to continue!";
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
            AudioListener.pause = false;

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
        AudioListener.pause = true;
    }

    public void ResumeAudio()
    {
        AudioListener.pause = false;
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
