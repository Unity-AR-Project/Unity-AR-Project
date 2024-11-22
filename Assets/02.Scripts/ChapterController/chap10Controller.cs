using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class chap10Controller : MonoBehaviour
{
    private bool isTouched = false;
    private GameObject selectedObj;
    [SerializeField] private Camera arCamera; // AR Camera ���� �ʿ�
    [SerializeField] private LayerMask _selectMask; // ��ġ ������ ������Ʈ ���̾�
    [SerializeField] private LayerMask _groundMask; // �̵� ������ �ٴ� ���̾�

    private Vector3 initialPosition;

    [SerializeField] private PlayableDirector timelineDirector; // Ÿ�Ӷ��� ����
    [SerializeField] private GameObject uiText; // UI �ؽ�Ʈ ������Ʈ (�ȳ� �޽���)

    private bool isPaused = false; // Ÿ�Ӷ����� �ߴܵǾ����� ����
    private const double PauseTime = 2.2; // Ÿ�Ӷ��� �ߴ� �ð�

    private void Start()
    {
        // AR ī�޶� ����
        if (arCamera == null)
        {
            arCamera = Camera.main;
            if (arCamera == null)
            {
                Debug.LogError("[Debug] : AR Camera not assigned.");
            }
        }

        // UI �ؽ�Ʈ�� ���� �� ��Ȱ��ȭ
        if (uiText != null)
        {
            uiText.SetActive(false);
        }

        // Ÿ�Ӷ��� �ʱ�ȭ
        if (timelineDirector != null)
        {
            timelineDirector.stopped += OnTimelineStopped;
            timelineDirector.Play(); // Ÿ�Ӷ��� ����
            Invoke(nameof(PauseTimelineAtSpecificTime), (float)PauseTime);
        }
        else
        {
            Debug.LogError("[Debug] : PlayableDirector not assigned.");
        }

        AudioListener.pause = false;
    }

    private void Update()
    {
        if (!isPaused || Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        // ��ġ ����
        if (touch.phase == TouchPhase.Began)
        {
            Ray ray = arCamera.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _selectMask))
            {
                if (hit.collider.gameObject.name == "Wolf") // ���밡 ���õǾ����� Ȯ��
                {
                    selectedObj = hit.collider.gameObject;
                    initialPosition = selectedObj.transform.position;
                    isTouched = true;
                    selectedObj.layer = LayerMask.NameToLayer("ARSelected");
                    Debug.Log($"[Debug] : {selectedObj.name} selected.");
                }
            }
        }

        // ��ġ �̵�
        if (touch.phase == TouchPhase.Moved && isTouched && selectedObj != null)
        {
            Ray ray = arCamera.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _groundMask))
            {
                if (hit.collider.gameObject.name == "Chimney") // ���ҿ� �����ߴ��� Ȯ��
                {
                    Debug.Log($"[Debug] : {selectedObj.name} reached the Chimney!");

                    // ���� ��Ȱ��ȭ
                    selectedObj.SetActive(false);

                    // Ÿ�Ӷ��� �� ����� �簳
                    ResumeTimeline();
                }
                else
                {
                    // ������ �ƴ� ������ �̵�
                    selectedObj.transform.position = hit.point;
                    Debug.Log($"[Debug] : {selectedObj.name} moved to {hit.point}");
                }
            }
        }

        // ��ġ ����
        if (touch.phase == TouchPhase.Ended && selectedObj != null)
        {
            isTouched = false;
            selectedObj.layer = LayerMask.NameToLayer("ARSelectable");
        }
    }

    private void PauseTimelineAtSpecificTime()
    {
        if (timelineDirector != null)
        {
            timelineDirector.Pause();
            isPaused = true;

            if (uiText != null)
            {
                uiText.SetActive(true);
                uiText.GetComponent<Text>().text = "���ҿ� ���븦 �Ű��ּ���!";
            }

            Debug.Log("[Debug] : Timeline paused at 2.2 seconds.");
        }
    }

    private void ResumeTimeline()
    {
        if (timelineDirector != null)
        {
            timelineDirector.Play();
            isPaused = false;
            AudioListener.pause = false; // ����� �簳
            

            if (uiText != null)
            {
                uiText.SetActive(false);
            }

            Debug.Log("[Debug] : Timeline and narration resumed.");
        }
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        if (director == timelineDirector)
        {
            Debug.Log("[Debug] : Timeline has ended.");
        }
    }

    public void PauseAudio()
    {
        AudioListener.pause = true; //����� �Ͻ�����
    }
}
