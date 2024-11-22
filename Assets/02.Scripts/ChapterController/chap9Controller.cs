using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class chap9Controller : MonoBehaviour
{
    private bool isTouched = false;
    private GameObject selectedObj;
    [SerializeField] private Camera arCamera;

    [SerializeField] private LayerMask _selectMask;
    [SerializeField] private LayerMask _groundMask;

    private Vector3 initialPosition;

    [SerializeField] private GameObject uiTextObject; // �ؽ�Ʈ�� ���Ե� UI ������Ʈ
    private Text uiText; // UI �ؽ�Ʈ ������Ʈ

    [SerializeField] private PlayableDirector playableDirector; // Ÿ�Ӷ��� ��Ʈ�ѷ�
    private bool isDraggingEnabled = false; // �巡�� ��� Ȱ��ȭ ����

    private void Start()
    {
        if (arCamera == null)
        {
            arCamera = Camera.main; // �⺻������ "Main Camera"�� �Ҵ�
            if (arCamera == null)
            {
                Debug.LogError("[Debug] : chap9 AR Camera not assigned.");
            }
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
        }

        // Ÿ�Ӷ��� �̺�Ʈ ����
        if (playableDirector != null)
        {
            playableDirector.stopped += OnTimelineStopped; // Ÿ�Ӷ��� ���� �̺�Ʈ ���
        }
        else
        {
            Debug.LogError("[Debug] : PlayableDirector not assigned.");
        }
    }

    private void Update()
    {
        if (!isDraggingEnabled || Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        // ��ġ ����
        if (touch.phase == TouchPhase.Began)
        {
            Debug.LogWarning("[Debug] : chap9 Start Touch");
            Ray ray = arCamera.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _selectMask))
            {
                // ����� ���õǾ����� Ȯ��
                AppleStatus appleStatus = hit.collider.GetComponent<AppleStatus>();
                if (appleStatus != null && !appleStatus.isOnWolf)
                {
                    selectedObj = hit.collider.gameObject;
                    initialPosition = selectedObj.transform.position;
                    isTouched = true;
                    selectedObj.layer = LayerMask.NameToLayer("ARSelected");
                    Debug.LogWarning("[Debug] : {selectedObj.name}");
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
                if (hit.collider.gameObject.name == "Wolf_5")
                {
                    Debug.LogWarning($"[Debug] : {selectedObj.name} hit TargetTree!");

                    // ���� ��ġ ����
                    Vector3 fixedPosition = GetFixedPositionForObject(selectedObj.name);
                    selectedObj.transform.localPosition = fixedPosition;

                    // ��� ���� ������Ʈ
                    selectedObj.GetComponent<AppleStatus>().isOnWolf = true;
                    Debug.LogWarning($"[Debug] : {selectedObj.name} is now on wolf: {selectedObj.GetComponent<AppleStatus>().isOnWolf}");

                    Debug.LogWarning($"[Debug] : {selectedObj.name} fixed at {fixedPosition}");

                    // ��� ����� ���뿡 �����Ǿ����� Ȯ��
                    CheckAllApplesOnWolf();
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

            if (!selectedObj.GetComponent<AppleStatus>().isOnWolf)
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
            case "Apple1": return new Vector3(4.02f, 1.91f, -5.12f);
            case "Apple2": return new Vector3(5.02f, 0.95f, -5.10f);
            case "Apple3": return new Vector3(5.69f, 1.78f, -5.03f);
            default: return Vector3.zero; // �⺻ ��ġ�� 0, 0, 0
        }
    }

    // ��� ����� ���뿡 �����Ǿ����� Ȯ��
    private void CheckAllApplesOnWolf()
    {
        bool allApplesOnWolf = GameObject.FindGameObjectsWithTag("Apple").All(apple => apple.GetComponent<AppleStatus>().isOnWolf);
        Debug.LogWarning($"[Debug] : All Apples on Wolf: {allApplesOnWolf}");

        if (allApplesOnWolf)
        {
            isDraggingEnabled = false; // �巡�� ��� ��Ȱ��ȭ

            if (uiTextObject != null)
            {
                uiTextObject.SetActive(false); // ��� ����� �����Ǹ� UI ����
            }

            // �����̼� �簳
            AudioListener.pause = false;
            Debug.LogWarning("[Debug] : ��� ����� ���뿡 �����Ǿ����ϴ�. �����̼� ��� ����.");
        }
    }

    // Ÿ�Ӷ��� ���� �̺�Ʈ �ڵ鷯
    public void OnTimelineStopped(PlayableDirector director)
    {
        if (director == playableDirector)
        {
            Debug.LogWarning("[Debug] : Timeline stopped. Enable dragging and show UI text.");
            AudioListener.pause = true; // �����̼� ����
            isDraggingEnabled = true; // �巡�� ��� Ȱ��ȭ

            if (uiTextObject != null)
            {
                uiTextObject.SetActive(true);
                uiText.text = "������ ��� �ִ� ����� ���뿡�� �����ּ���.";
            }
        }
    }

    private void OnDestroy()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped -= OnTimelineStopped; // �̺�Ʈ ����
        }
    }
}
