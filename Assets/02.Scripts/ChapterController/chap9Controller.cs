using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;
using UnityEngine.Audio;

public class chap9Controller : MonoBehaviour, IChapterController
{
    [SerializeField] private LayerMask _selectMask; // ���� ������ ������Ʈ ���̾�
    [SerializeField] private LayerMask _groundMask; // �̵� ������ �ٴ� ���̾�

    [Header("Timeline & UI")]
    public PlayableDirector playableDirector; // Ÿ�Ӷ��� ��� ���� PlayableDirector
    public TextMeshProUGUI uiText; // UI �ؽ�Ʈ ������Ʈ (�ȳ� �޽���)

    [Header("Drag Settings")]
    private GameObject selectedObj;
    private Vector3 initialPosition;
    private bool isTouched = false;

    private bool isPaused = false; // Ÿ�Ӷ��� ���� ���� ����
    private const double PauseTime = 15.8; // Ÿ�Ӷ��� ���� ����


   /* //������ �ʱ�ȭ
    [SerializeField] private GameObject chapter9Prefab; // é�� 7 ������
    [SerializeField] private Transform prefabParent; // �������� �ν��Ͻ�ȭ�� �θ� ������Ʈ
    private GameObject chapter9Instance; // ���� Ȱ��ȭ�� é�� 7 �ν��Ͻ�
*/
    void OnEnable()
    {
     /*   if (chapter9Instance != null)
        {
            Destroy(chapter9Instance);
        }

        // é�� 9 ������ �ν��Ͻ�ȭ
        if (chapter9Prefab != null && prefabParent != null)
        {
            chapter9Instance = Instantiate(chapter9Prefab, prefabParent);
            chapter9Instance.tag = "Chapter1Instance"; // �ʿ� �� �±� ����
            chapter9Instance.SetActive(true);
            Debug.Log("[chap9Controller] Chapter9 prefab instantiated.");
        }
        else
        {
            Debug.LogError("[chap9Controller] Chapter9Prefab or PrefabParent is not assigned.");
        }
        if (playableDirector != null)
        {*/
            // Ÿ�Ӷ��� ����
            playableDirector.time = 0; // Ÿ�Ӷ��� �ð� �ʱ�ȭ
            playableDirector.Stop();   // Ÿ�Ӷ��� ����
                                       // �ʱ� �޽��� ǥ��
        UIManager.instance.ShowMessage("9é�� ���۵Ǿ����ϴ�.\n" +
            "��ٷ��ּ���!");

        playableDirector.Play();   // Ÿ�Ӷ��� ���
    /*    }
        else
        {
            Debug.LogError("[chap9Controller] PlayableDirector not assigned.");
        }*/
    }


    private void Start()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped += OnPlayableDirectorStopped;
            Invoke("PauseTimelineAtSpecificTime", (float)PauseTime);
        }

        if (uiText != null)
        {
            uiText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isPaused || Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        // ��ġ ����
        if (touch.phase == TouchPhase.Began)
        {
            Debug.LogWarning("[Debug] : chap9 Start Touch");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

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
                    Debug.LogWarning($"[Debug] : {selectedObj.name}");
                }
            }
        }

        // ��ġ �̵�
        if (touch.phase == TouchPhase.Moved && isTouched && selectedObj != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
            case "Apple1": return new Vector3(13f, 0f, -5.26f);
            case "Apple2": return new Vector3(12.87f, 0f, -1.46f);
            case "Apple3": return new Vector3(14.9f, 1.78f, -3.03f);
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

            if (uiText != null)
            {
                uiText.gameObject.SetActive(false); // ��� ����� �����Ǹ� UI ����
            }

            // Ÿ�Ӷ��� �� ����� �簳
            ResumeTimeline();

            Debug.LogWarning("[Debug] : ��� ����� ���뿡 �����Ǿ����ϴ�. �����̼� ��� ����.");
        }
    }

    private void ResumeTimeline()
    {
        if (playableDirector != null)
        {
            // UI �޽��� ǥ��
            UIManager.instance.ShowMessage("���߾��!");

            playableDirector.Play();
            isPaused = false;

            if (uiText != null)
            {
                uiText.gameObject.SetActive(false);
            }

            Debug.Log("[Debug] : Timeline and narration resumed.");
        }
    }


    private void PauseTimelineAtSpecificTime()
    {
        if (playableDirector != null)
        {
            playableDirector.Pause();
            isPaused = true;


            UIManager.instance.ShowMessage("��� 3���� \n ���뿡�� �����ּ���~");
            
        }
    }

    private void OnPlayableDirectorStopped(PlayableDirector director)
    {
        if (director == playableDirector)
        {
            Debug.Log("[Debug] : Timeline has ended.");
        }
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
