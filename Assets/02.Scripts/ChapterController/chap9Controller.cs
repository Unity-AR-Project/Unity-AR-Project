using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class chap9Controller : MonoBehaviour
{
    private bool isTouched = false;
    private GameObject selectedObj;
    [SerializeField] private Camera arCamera;

    [SerializeField] private LayerMask _selectMask;
    [SerializeField] private LayerMask _groundMask;

    private Vector3 initialPosition;

    [SerializeField] private GameObject uiTextObject; // 텍스트가 포함된 UI 오브젝트
    private Text uiText; // UI 텍스트 컴포넌트

    private void Start()
    {
        if (arCamera == null)
        {
            arCamera = Camera.main;  // 기본적으로 "Main Camera"로 할당

            if (arCamera == null)
            {
                Debug.LogError("[Debug] : chap9 AR Camera not assigned.");
            }
        }

        // UI 텍스트 초기화
        if (uiTextObject != null)
        {
            uiText = uiTextObject.GetComponent<Text>();
            if (uiText == null)
            {
                Debug.LogError("No Text component found on the assigned GameObject.");
            }

            uiTextObject.SetActive(true); // 시작 시 UI 활성화
            uiText.text = "돼지가 들고 있는 사과를 늑대에게 던져주세요."; // 초기 텍스트 설정
        }
    }

    private void Update()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);


        // 터치 시작
        if (touch.phase == TouchPhase.Began)
        {
            Debug.LogWarning("[Debug] : chap9 Start Touch");
            Ray ray = arCamera.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _selectMask))
            {
                //사과가 선택되었는지 확인
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

        // 터치 이동
        if (touch.phase == TouchPhase.Moved && isTouched && selectedObj != null)
        {
            Ray ray = arCamera.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _groundMask))
            {
                if (hit.collider.gameObject.name == "Wolf_5")
                {
                    Debug.LogWarning($"[Debug] : {selectedObj.name} hit TargetTree!");

                    // 고정 위치 설정
                    Vector3 fixedPosition = GetFixedPositionForObject(selectedObj.name);
                    selectedObj.transform.localPosition = fixedPosition;

                    // 돼지 상태 업데이트
                    selectedObj.GetComponent<AppleStatus>().isOnWolf = true;
                    Debug.LogWarning($"[Debug] : {selectedObj.name} is now on tree: {selectedObj.GetComponent<AppleStatus>().isOnWolf}");

                    Debug.LogWarning($"[Debug] : {selectedObj.name} fixed at {fixedPosition}");

                    // 모든 돼지가 나무에 고정되었는지 확인
                    CheckAllApplesOnWolf();
                }
                else
                {
                    // 나무가 아닌 곳으로 이동
                    selectedObj.transform.position = hit.point;
                    Debug.LogWarning($"[Debug] : {selectedObj.name} moved to {hit.point}");
                }
            }
        }

        // 터치 종료
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

    // 돼지 이름에 따라 고정 위치 반환
    private Vector3 GetFixedPositionForObject(string objectName)
    {
        switch (objectName)
        {
            case "Apple1": return new Vector3(4.02f, 1.91f, -5.12f);
            case "Apple2": return new Vector3(5.02f, 0.95f, -5.10f);
            case "Apple3": return new Vector3(5.69f, 1.78f, -5.03f);
            default: return Vector3.zero; // 기본 위치는 0, 0, 0
        }
    }
    // 모든 사과가 늑대에 고정되었는지 확인
    private void CheckAllApplesOnWolf()
    {
        bool allApplesOnWolf = GameObject.FindGameObjectsWithTag("Apple").All(apple => apple.GetComponent<AppleStatus>().isOnWolf);
        Debug.LogWarning($"[Debug] : All Apple on Wolf: {allApplesOnWolf}"); // 상태 확인

        if (allApplesOnWolf)
        {
            if (uiTextObject != null)
            {
                Debug.LogWarning($"[Debug] : Hiding UI Text...");
                uiTextObject.SetActive(false); // 모든 돼지가 고정되면 UI 숨김
            }
            Debug.LogWarning("[Debug] : 모든 사과가 늑대에 고정되었습니다.");
        }
    }
}
