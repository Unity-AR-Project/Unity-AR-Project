using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables; // PlayableDirector ���� ���ӽ����̽� �߰�

public class chap8Controller : MonoBehaviour, IChapterController
{
    private bool isTouched = false;
    private GameObject selectedObj;
    [SerializeField] private Camera arCamera;

    [SerializeField] private LayerMask _selectMask;
    [SerializeField] private LayerMask _groundMask;

    private Vector3 initialPosition;

    [SerializeField] private GameObject uiTextObject; // �ؽ�Ʈ�� ���Ե� UI ������Ʈ
    private Text uiText; // UI �ؽ�Ʈ ������Ʈ

    [SerializeField] private PlayableDirector timelineDirector; // Ÿ�Ӷ��� ���� �߰�
    private bool timelineCompleted = false; // Ÿ�Ӷ��� ���� ���� Ȯ�� ����

    private bool isPaused = false; // Ÿ�Ӷ��� ���� ���¸� �����ϴ� �÷���

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

        // Ÿ�Ӷ��� ���� ����
        if (timelineDirector != null)
        {
            timelineDirector.stopped += OnTimelineStopped; // Ÿ�Ӷ��� ���� �̺�Ʈ ���
            timelineDirector.Play(); // Ÿ�Ӷ��� ���
        }
        else
        {
            Debug.LogError("[Debug] : PlayableDirector not assigned.");
        }

        // UI �ؽ�Ʈ �ʱ�ȭ
        if (uiTextObject != null)
        {
            uiText = uiTextObject.GetComponent<Text>();
            if (uiText == null)
            {
                Debug.LogError("No Text component found on the assigned GameObject.");
            }

            uiTextObject.SetActive(false); // ���� �� UI ��Ȱ��ȭ
            uiText.text = "���� �������� ������ �Ű��ּ���."; // �ʱ� �ؽ�Ʈ ����
        }
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        if (director == timelineDirector)
        {
            Debug.Log("[Debug] : Timeline has finished.");
            timelineCompleted = true; // Ÿ�Ӷ��� ���� ���� ������Ʈ

            // UI �ؽ�Ʈ Ȱ��ȭ
            if (uiTextObject != null)
            {
                uiTextObject.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (!timelineCompleted) return; // Ÿ�Ӷ����� ������ ������ ��ġ ��� ��Ȱ��ȭ

        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        // ��ġ ����
        if (touch.phase == TouchPhase.Began)
        {
            Ray ray = arCamera.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _selectMask))
            {
                // ������ ���õǾ����� Ȯ��
                PigStatus pigStatus = hit.collider.GetComponent<PigStatus>();
                if (pigStatus != null && !pigStatus.isOnTree)
                {
                    selectedObj = hit.collider.gameObject;
                    initialPosition = selectedObj.transform.position;
                    isTouched = true;
                    selectedObj.layer = LayerMask.NameToLayer("ARSelected");
                    Debug.LogWarning($"[Debug] : {selectedObj.name} selected.");
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
                if (hit.collider.gameObject.name == "tree_2")
                {
                    Debug.LogWarning($"[Debug] : {selectedObj.name} hit TargetTree!");

                    // ���� ��ġ ����
                    Vector3 fixedPosition = GetFixedPositionForObject(selectedObj.name);
                    selectedObj.transform.localPosition = fixedPosition;

                    // ���� ���� ������Ʈ
                    selectedObj.GetComponent<PigStatus>().isOnTree = true;
                    Debug.LogWarning($"[Debug] : {selectedObj.name} is now on tree: {selectedObj.GetComponent<PigStatus>().isOnTree}");

                    Debug.LogWarning($"[Debug] : {selectedObj.name} fixed at {fixedPosition}");

                    // ��� ������ ������ �����Ǿ����� Ȯ��
                    CheckAllPigsOnTree();
                }
                else
                {
                    // ������ �ƴ� ������ �̵�
                    selectedObj.transform.position = hit.point;
                    Debug.LogWarning($"[Debug] : {selectedObj.name} moved to {hit.point}");
                }
            }
        }

        // ��ġ ����
        if (touch.phase == TouchPhase.Ended && selectedObj != null)
        {
            isTouched = false;
            selectedObj.layer = LayerMask.NameToLayer("ARSelectable");

            if (!selectedObj.GetComponent<PigStatus>().isOnTree)
            {
                selectedObj.transform.position = initialPosition;
                Debug.LogWarning($"[Debug] : {selectedObj.name} reset to initial position");
            }
            else
            {
                Debug.LogWarning($"[Debug] : {selectedObj.name} remains at fixed position");
            }

            selectedObj = null;
        }
    }

    // ���� �̸��� ���� ���� ��ġ ��ȯ
    private Vector3 GetFixedPositionForObject(string objectName)
    {
        switch (objectName)
        {
            case "FirstPig": return new Vector3(-0.68f, 7.18f, -1.07f); // FirstPig�� ���� ��ġ
            case "SecondPig": return new Vector3(2.35f, 8.63f, -1.56f); // SecondPig�� ���� ��ġ
            case "ThirdPig": return new Vector3(5.56f, 6.89f, -0.99f); // ThirdPig�� ���� ��ġ
            default: return Vector3.zero; // �⺻ ��ġ�� 0, 0, 0
        }
    }

    // ��� ������ ������ �����Ǿ����� Ȯ��
    private void CheckAllPigsOnTree()
    {
        bool allPigsOnTree = GameObject.FindGameObjectsWithTag("Pig").All(pig => pig.GetComponent<PigStatus>().isOnTree);
        Debug.LogWarning($"[Debug] : All pigs on tree: {allPigsOnTree}"); // ���� Ȯ��

        if (allPigsOnTree)
        {
            if (uiTextObject != null)
            {
                Debug.LogWarning($"[Debug] : Hiding UI Text...");
                uiTextObject.SetActive(false); // ��� ������ �����Ǹ� UI ����
            }
            Debug.LogWarning("[Debug] : ��� ������ ������ �����Ǿ����ϴ�!");
        }
    }

    /// <summary>
    /// Ÿ�Ӷ��� �Ͻ�����/�簳 ���
    /// </summary>
    public void TogglePause()
    {
        if (isPaused)
        {
            timelineDirector.Play();
            isPaused = false;
        }
        else
        {
            timelineDirector.Pause();
            isPaused = true;
        }
    }

}
