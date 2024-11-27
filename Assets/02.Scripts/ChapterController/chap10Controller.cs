using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.EventSystems;

public class chap10Controller : MonoBehaviour, IChapterController
{
    // ���� �� ������Ʈ ����
    private bool isTouched = false;
    private GameObject selectedObj;
    private Camera arCamera; // AR Camera ���� �ʿ�
    [SerializeField] private LayerMask _selectMask; // ��ġ ������ ������Ʈ ���̾�
    [SerializeField] private LayerMask _groundMask; // �̵� ������ �ٴ� ���̾�

    private Vector3 initialPosition;

    [SerializeField] private PlayableDirector playableDirector; // Ÿ�Ӷ��� ����

    private bool isPaused = false; // �Ͻ����� ���� ����
    private const float PauseTime = 2.2f; // Ÿ�Ӷ��� �ߴ� �ð�

    // ���̾� �� �±� ��� ����
    private const string SELECTABLE_LAYER = "ARSelectable";
    private const string SELECTED_LAYER = "ARSelected";
    private const string WOLF_TAG = "Wolf";
    private const string CHIMNEY_TAG = "Chimney";

    //������ �ʱ�ȭ
   /* [SerializeField] private GameObject chapter10Prefab; // é�� 7 ������
    [SerializeField] private Transform prefabParent; // �������� �ν��Ͻ�ȭ�� �θ� ������Ʈ
    private GameObject chapter10Instance; // ���� Ȱ��ȭ�� é�� 7 �ν��Ͻ�*/

    void OnEnable()
    {/*
        if (chapter10Instance != null)
        {
            Destroy(chapter10Instance);
        }

        // é�� 10 ������ �ν��Ͻ�ȭ
        if (chapter10Prefab != null && prefabParent != null)
        {
            chapter10Instance = Instantiate(chapter10Prefab, prefabParent);
            chapter10Instance.tag = "Chapter10Instance"; // �ʿ� �� �±� ����
            chapter10Instance.SetActive(true);
            Debug.Log("[chap10Controller] Chapter10 prefab instantiated.");
        }
        else
        {
            Debug.LogError("[chap10Controller] Chapter10Prefab or PrefabParent is not assigned.");
        }

        // Ÿ�Ӷ��� �ʱ� ����: ������� �ʰ� ��� ���·� ����
        if (playableDirector != null)
        {*/
            // Ÿ�Ӷ��� ����
            playableDirector.time = 0; // Ÿ�Ӷ��� �ð� �ʱ�ȭ
            playableDirector.Stop();   // Ÿ�Ӷ��� ����
            playableDirector.Play();   // Ÿ�Ӷ��� ���
     /*   }
        else
        {
            Debug.LogError("[chap10Controller] PlayableDirector not assigned.");
        }*/
    }

    private void Start()
    {
        // AR ī�޶� ����
        if (arCamera == null)
        {
            arCamera = Camera.main;
            if (arCamera == null)
            {
                Debug.LogError("[chap10Controller] AR Camera not assigned.");
            }
        }

        // Ÿ�Ӷ��� �̺�Ʈ ����
        if (playableDirector != null)
        {
            playableDirector.stopped += OnTimelineStopped; // Ÿ�Ӷ��� ���� �̺�Ʈ ���
            playableDirector.played += OnTimelinePlayed;   // Ÿ�Ӷ��� ���� �̺�Ʈ ���
            playableDirector.time = 0; // Ÿ�Ӷ��� �ð� �ʱ�ȭ
            playableDirector.Play();   // Ÿ�Ӷ��� ���
            // Signal Track�� ���� Ÿ�Ӷ��� �ߴ��� ó���ϹǷ� Invoke�� ����
        }
        else
        {
            Debug.LogError("[chap10Controller] PlayableDirector not assigned.");
        }

        // �ʱ� �޽��� ǥ��
        UIManager.instance.ShowMessage("10é�� ���۵Ǿ����ϴ�. ��ٷ��ּ���!");

        // ������ �� ������Ʈ ���� �ʱ�ȭ
        GameObject wolf = GameObject.FindWithTag(WOLF_TAG);
        if (wolf != null)
        {
            wolf.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[chap10Controller] Wolf object not found in the scene.");
        }
    }

    private void Update()
    {
        if (!isPaused || Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        // UI ��� ���� ��ġ�� ����
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                OnTouchBegan(touch);
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                OnTouchMoved(touch);
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                OnTouchEnded(touch);
                break;
        }
    }

    private void OnTouchBegan(Touch touch)
    {
        Ray ray = arCamera.ScreenPointToRay(touch.position);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _selectMask))
        {
            if (hit.collider.CompareTag(WOLF_TAG))
            {
                selectedObj = hit.collider.gameObject;
                initialPosition = selectedObj.transform.position;
                isTouched = true;
                selectedObj.layer = LayerMask.NameToLayer(SELECTED_LAYER);
                selectedObj.GetComponent<Collider>().enabled = false; // �ݶ��̴� ��Ȱ��ȭ
                Debug.Log($"[chap10Controller] {selectedObj.name} selected.");

                // UI �޽��� ǥ��
                UIManager.instance.ShowMessage("�������� ���븦 �̵������ּ���!");
            }
        }
    }

    private void OnTouchMoved(Touch touch)
    {
        if (!isTouched || selectedObj == null) return;

        Ray ray = arCamera.ScreenPointToRay(touch.position);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundMask))
        {
            if (hit.collider.CompareTag(CHIMNEY_TAG))
            {
                Debug.Log($"[chap10Controller] {selectedObj.name} reached the Chimney!");

                // ���� ��Ȱ��ȭ
                selectedObj.SetActive(false);

                // Ÿ�Ӷ��� �� UI �簳
                ResumeTimeline();
            }
            else
            {
                // ������ �ƴ� ������ �̵�
                selectedObj.transform.position = hit.point;
                Debug.Log($"[chap10Controller] {selectedObj.name} moved to {hit.point}");
            }
        }
    }

    private void OnTouchEnded(Touch touch)
    {
        if (!isTouched || selectedObj == null) return;

        isTouched = false;
        selectedObj.layer = LayerMask.NameToLayer(SELECTABLE_LAYER);
        selectedObj.GetComponent<Collider>().enabled = true; // �ݶ��̴� ��Ȱ��ȭ

        // ������Ʈ�� ��ġ�� ������� �ǵ����� ���� (����ڰ� ���ϴ� ��ġ�� ������Ʈ�� �ε���)
        selectedObj = null;

        // UI �޽��� ����
        UIManager.instance.HideMessage();

        Debug.Log("[chap10Controller] Object deselected.");
    }

    private void ResumeTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.Play();
            isPaused = false;

            // UI �޽��� ����
            UIManager.instance.HideMessage();

            Debug.Log("[chap10Controller] Timeline resumed.");
        }
    }

    // Signal�� ���� ȣ��Ǵ� �޼���
    public void PauseTimelineAtSpecificTime()
    {
        if (playableDirector != null)
        {
            playableDirector.Pause();
            isPaused = true;

            // UI �޽��� ǥ��
            UIManager.instance.ShowMessage("���ҿ� ���븦 �Ű��ּ���!");

            Debug.Log("[chap10Controller] Timeline paused at 2.2 seconds.");
        }
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        if (director == playableDirector)
        {
            Debug.Log("[chap10Controller] Timeline has ended.");
        }
    }

    private void OnTimelinePlayed(PlayableDirector director)
    {
        Debug.Log("[chap10Controller] Timeline started.");
    }

    /// <summary>
    /// Ÿ�Ӷ��� �Ͻ�����/�簳 ���
    /// </summary>
    public void TogglePause()
    {
        if (playableDirector == null) return;

        if (isPaused)
        {
            playableDirector.Play();
            isPaused = false;

            // UI �޽��� ����
            UIManager.instance.HideMessage();

            Debug.Log("[chap10Controller] Timeline resumed.");
        }
        else
        {
            playableDirector.Pause();
            isPaused = true;

            // UI �޽��� ǥ��
            UIManager.instance.ShowMessage("Ÿ�Ӷ����� �Ͻ������Ǿ����ϴ�.");

            Debug.Log("[chap10Controller] Timeline paused.");
        }
    }
}
