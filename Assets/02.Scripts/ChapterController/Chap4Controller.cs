using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Chap4Controller : MonoBehaviour
{
    public PlayableDirector playableDirector; // Ÿ�Ӷ��� ��� ���� PlayableDirector
    public GameObject uiText; // UI �ؽ�Ʈ ������Ʈ (�ȳ� �޽���)

    private int touchCount = 0; // ���� ��ġ Ƚ��
    private bool isPaused = false; // Ÿ�Ӷ��� ���� ���¸� �����ϴ� �÷���

    private const double PauseTime = 13.8; // Ÿ�Ӷ��� ���� �ð� (13.80��)
    public LayerMask groundLayer; // Ground ���̾ ���� (����ĳ��Ʈ�� �浹�� ���̾�)

    private void Start()
    {
        // PlayableDirector�� �ִٸ� Ÿ�Ӷ��� ���� �� �̺�Ʈ�� ���
        if (playableDirector != null)
        {
            playableDirector.stopped += OnPlayableDirectorStopped;

            // Ÿ�Ӷ����� ���۵Ǹ� ���� �ð��� ���� �� �ڵ����� ���߰� ����
            Invoke(nameof(PauseTimelineAtSpecificTime), (float)PauseTime);
        }

        // UI �ؽ�Ʈ�� ���� �� ��Ȱ��ȭ
        if (uiText != null)
        {
            uiText.SetActive(false);
        }
    }

    private void Update()
    {
        // Ÿ�Ӷ����� ���� ���� ���� ��ġ �Է��� ó��
        if (isPaused && Input.GetMouseButtonDown(0)) // ���콺 Ŭ�� (��ġ ��ü ����)
        {
            // ȭ���� ��ġ ��ġ���� ������ ��� �浹�� Ȯ��
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast�� ��ġ�� ��ü�� Ground ���̾ �ִ��� Ȯ��
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                // ��ġ�� ������Ʈ�� �̸��� "Wood house"�� ���
                if (hit.collider.gameObject.name == "Wood house")
                {
                    touchCount++; // ��ġ Ƚ�� ����
                    Debug.LogWarning($"[Debug] : Wood house touched {touchCount} times!");

                    // ��ġ Ƚ���� 3 �̻��̸� Ÿ�Ӷ����� �簳
                    if (touchCount >= 3)
                    {
                        ResumeTimeline(); // Ÿ�Ӷ��� �簳
                    }
                }
            }
        }
    }

    // Ÿ�Ӷ����� Ư�� �ð����� ���߰� UI �ؽ�Ʈ ǥ��
    private void PauseTimelineAtSpecificTime()
    {
        if (playableDirector != null)
        {
            playableDirector.time = PauseTime; // Ÿ�Ӷ����� 3.50�ʷ� �̵�
            playableDirector.Pause(); // Ÿ�Ӷ��� ����
            isPaused = true; // ���� ���� �÷��� ����

            if (uiText != null)
            {
                uiText.SetActive(true); // UI �ؽ�Ʈ Ȱ��ȭ
                uiText.GetComponent<Text>().text = "Touch the wood house 3 times to continue!"; // �ȳ� �޽��� ����
            }

            Debug.LogWarning("[Debug] : Timeline paused at 3.50 seconds.");
        }
    }

    // Ÿ�Ӷ����� �簳�ϰ� UI �ؽ�Ʈ ��Ȱ��ȭ
    private void ResumeTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.Play(); // Ÿ�Ӷ��� �簳
            isPaused = false; // ���� ���� �÷��� ����

            if (uiText != null)
            {
                uiText.SetActive(false); // UI �ؽ�Ʈ ��Ȱ��ȭ
            }

            touchCount = 0; // ��ġ Ƚ�� �ʱ�ȭ
            Debug.LogWarning("[Debug] : Timeline resumed.");
        }
    }

    // Ÿ�Ӷ����� ����Ǿ��� �� ȣ��Ǵ� �޼���
    private void OnPlayableDirectorStopped(PlayableDirector director)
    {
        if (director == playableDirector)
        {
            Debug.LogWarning("[Debug] : Timeline has ended!");
            // �ʿ��� �߰� ���� �ۼ�
        }
    }
}
