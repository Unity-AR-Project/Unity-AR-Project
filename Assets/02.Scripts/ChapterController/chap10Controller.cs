using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class chap10Controller : MonoBehaviour , IChapterController
{
    private bool isTouched = false;
    private GameObject selectedObj;
    private Camera arCamera; // AR Camera ���� �ʿ�
    [SerializeField] private LayerMask _selectMask; // ��ġ ������ ������Ʈ ���̾�
    [SerializeField] private LayerMask _groundMask; // �̵� ������ �ٴ� ���̾�

    private Vector3 initialPosition;

    [SerializeField] private PlayableDirector timelineDirector; // Ÿ�Ӷ��� ����
    //public GameObject uiText; // UI �ؽ�Ʈ ������Ʈ (�ȳ� �޽���)
    private bool isPaused = false; // �Ͻ����� ���� ����
    void OnEnable()
    {
        // Ÿ�Ӷ��� ����
        timelineDirector.time = 0; // Ÿ�Ӷ��� �ð� �ʱ�ȭ
        timelineDirector.Stop();   // Ÿ�Ӷ��� ����
        timelineDirector.Play();   // Ÿ�Ӷ��� ���
    }


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
        /* if (uiText != null)
         {
            // uiText.SetActive(true);
         }*/
        UIManager.instance.ShowMessage("���� �������� ���� ���� �ö󰬾��!");


        // Ÿ�Ӷ��� �ʱ�ȭ
        if (timelineDirector != null)
        {
            timelineDirector.Stop(); // Ÿ�Ӷ��� �ʱ� ���� ���·� ����
        }
        else
        {
            Debug.LogError("[Debug] : PlayableDirector not assigned.");
        }
    }

    private void Update()
    {
        if (Input.touchCount == 0) return;

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

                    // Ÿ�Ӷ��� ����
                    if (timelineDirector != null)
                    {
                        timelineDirector.Play();
                    }

                   /* if (uiText != null)
                    {
                        uiText.SetActive(false); // UI �ؽ�Ʈ ��Ȱ��ȭ
                    }*/
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
            selectedObj.SetActive(false);
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
