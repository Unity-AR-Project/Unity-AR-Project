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

    // 레이어 이름 상수 정의
    private const string SELECTABLE_LAYER = "ARSelectable";
    private const string SELECTED_LAYER = "ARSelected";
    private const string TARGET_TAG = "Corgi"; // 오브젝트 태그로 변경

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
        // 싱글 터치만 허용
        if (Input.touchCount != 1) return;

        Touch touch = Input.GetTouch(0);

        // UI 요소 위의 터치는 무시
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
                selectedObjCollider.enabled = false; // 콜라이더 비활성화
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
        selectedObjCollider.enabled = true; // 콜라이더 재활성화

        // 필요에 따라 위치 복귀 로직 추가
        //selectedObj.transform.position = initialPosition;

        selectedObj = null;
        selectedObjCollider = null;
        Debug.Log("[SelectObject] Object deselected.");
    }
}
