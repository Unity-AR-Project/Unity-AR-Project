using UnityEngine;

public class SelectObject : MonoBehaviour
{
    private bool isTouched = false;
    private GameObject selectedObj;
    private Collider selectedObjCollider;
    [SerializeField] private Camera arCamera;

    [SerializeField] private LayerMask selectMask;
    [SerializeField] private LayerMask groundMask;

    private Vector3 initialPosition;

    // ���̾� �̸� ��� ����
    private const string SELECTABLE_LAYER = "ARSelectable";
    private const string SELECTED_LAYER = "ARSelected";
    private const string TARGET_TAG = "Corgi"; // ������Ʈ �±׷� ����

    private void Start()
    {
        if (arCamera == null)
        {
            arCamera = Camera.main;
            if (arCamera == null)
            {
                Debug.LogError("[SelectObject] AR Camera not assigned.");
            }
        }
    }

    private void Update()
    {
        // �̱� ��ġ�� ���
        if (Input.touchCount != 1) return;

        Touch touch = Input.GetTouch(0);

        // UI ��� ���� ��ġ�� ����
        if (UnityEngine.EventSystems.EventSystem.current != null && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touch.fingerId))
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
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, selectMask))
        {
            if (hit.collider.CompareTag(TARGET_TAG))
            {
                selectedObj = hit.collider.gameObject;
                selectedObjCollider = hit.collider;
                initialPosition = selectedObj.transform.position;
                isTouched = true;
                selectedObj.layer = LayerMask.NameToLayer(SELECTED_LAYER);
                selectedObjCollider.enabled = false; // �ݶ��̴� ��Ȱ��ȭ
                Debug.Log("[SelectObject] Object selected: " + selectedObj.name);
            }
        }
    }

    private void OnTouchMoved(Touch touch)
    {
        if (!isTouched || selectedObj == null) return;

        Ray ray = arCamera.ScreenPointToRay(touch.position);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundMask))
        {
            selectedObj.transform.position = hit.point;
        }
    }

    private void OnTouchEnded(Touch touch)
    {
        if (!isTouched || selectedObj == null) return;

        isTouched = false;
        selectedObj.layer = LayerMask.NameToLayer(SELECTABLE_LAYER);
        selectedObjCollider.enabled = true; // �ݶ��̴� ��Ȱ��ȭ

        // �ʿ信 ���� ��ġ ���� ���� �߰�
        //selectedObj.transform.position = initialPosition;

        selectedObj = null;
        selectedObjCollider = null;
        Debug.Log("[SelectObject] Object deselected.");
    }
}
