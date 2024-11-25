using UnityEngine;
using UnityEngine.Playables;
using TMPro;

public class Chap4Controller : MonoBehaviour, IChapterController
{
    public PlayableDirector playableDirector; // Ÿ�Ӷ��� ��� ���� PlayableDirector
    public TextMeshProUGUI uiText; // UI �ؽ�Ʈ ������Ʈ (�ȳ� �޽���)

    private int touchCount = 0; // ���� ��ġ Ƚ��
    private bool isPaused = false; // Ÿ�Ӷ��� ���� ���¸� �����ϴ� �÷���

    private const double PauseTime = 13.8; // Ÿ�Ӷ��� ���� �ð� (13.80��)
    public LayerMask groundLayer; // Ground ���̾ ���� (����ĳ��Ʈ�� �浹�� ���̾�)

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
