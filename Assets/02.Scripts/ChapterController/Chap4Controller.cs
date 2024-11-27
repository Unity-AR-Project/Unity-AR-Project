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

    private AudioSource audioSource;   // ��� ���� AudioSource

    /*//������ �ʱ�ȭ
    [SerializeField] private GameObject chapter4Prefab; // é�� 7 ������
    [SerializeField] private Transform prefabParent; // �������� �ν��Ͻ�ȭ�� �θ� ������Ʈ
    private GameObject chapter4Instance; // ���� Ȱ��ȭ�� é�� 7 �ν��Ͻ�
*/
    void OnEnable()
    {
       /* if (chapter4Instance != null)
        {
            Destroy(chapter4Instance);
        }

        // é�� 4 ������ �ν��Ͻ�ȭ
        if (chapter4Prefab != null && prefabParent != null)
        {
            chapter4Instance = Instantiate(chapter4Prefab, prefabParent);
            chapter4Instance.tag = "Chapter1Instance"; // �ʿ� �� �±� ����
            chapter4Instance.SetActive(true);
            Debug.Log("[chap4Controller] Chapter4 prefab instantiated.");
        }
        else
        {
            Debug.LogError("[chap4Controller] Chapter1Prefab or PrefabParent is not assigned.");
        }

        // Ÿ�Ӷ��� �ʱ� ����: ������� �ʰ� ��� ���·� ����
        if (playableDirector != null)
        {
*/

            // Ÿ�Ӷ��� ����
            playableDirector.time = 0; // Ÿ�Ӷ��� �ð� �ʱ�ȭ
            playableDirector.Stop();   // Ÿ�Ӷ��� ����
            playableDirector.Play();   // Ÿ�Ӷ��� ���
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
                // UI �޽��� ǥ��
                UIManager.instance.ShowMessage("�������� �� �� ��ġ ���ּ���");
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
