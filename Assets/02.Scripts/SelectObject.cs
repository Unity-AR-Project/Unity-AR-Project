using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SelectObject : MonoBehaviour
{
    private bool isTouched = false; // 오브젝트가 터치되었는지 여부를 확인하는 플래그
    private GameObject selectedObj; // 선택된 오브젝트를 저장할 변수
    [SerializeField] private Camera arCamera; // AR 카메라 (Inspector에서 할당)

    [SerializeField] ARImageMultipleObjectsSpawner arImageMultipleObjectsSpawner;

    [SerializeField] LayerMask _selectMask;
    [SerializeField] LayerMask _groundMask;
    [SerializeField] LayerMask _targetMask;  // ARProp 레이어 마스크 (나무 레이어)

    private Vector3 initialPosition; // 초기 위치를 저장할 변수 추가
    private float treeDetectionDistance = 2f; // 나무와 얼마나 가까워져야 고정될지 설정
    private bool isCorgiOnTree = false; // 코기가 나무에 고정되었는지 확인하는 변수

    private void Start()
    {
        if (arCamera == null)
        {
            Debug.LogError("[Debug] : AR Camera not assigned."); // AR 카메라가 할당되지 않았을 경우 에러 출력
        }
    }

    private void Update()
    {
        if (Input.touchCount == 0) return; // 터치가 없으면 함수 종료

        Touch touch = Input.GetTouch(0); // 첫 번째 터치 입력을 가져옴
        Debug.LogWarning($"[Debug] : {nameof(SelectObject)}] Touched.");


        // 터치가 시작될 때
        if (touch.phase == TouchPhase.Began)
        {
            Ray ray = arCamera.ScreenPointToRay(touch.position); // 터치 위치로부터 Ray 생성
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _selectMask)) // Ray가 Collider와 충돌했는지 확인
            {
                if (hit.collider.name.Contains("corgi")) // 충돌한 오브젝트 이름에 "corgi"가 포함되었는지 확인
                {
                    selectedObj = hit.collider.gameObject; // 선택된 오브젝트로 저장
                    initialPosition = selectedObj.transform.position; // 초기 위치 저장
                    isTouched = true; // 오브젝트가 터치됨을 표시
                    selectedObj.layer = LayerMask.NameToLayer("ARSelected"); // 오브젝트의 레이어를 ARSelected로 설정
                    Debug.LogWarning($"[Debug] : corgi Layer : {LayerMask.LayerToName(selectedObj.layer)}");
                }
            }
        }

        // 터치가 이동 중이고 오브젝트가 선택되었을 때
        if (touch.phase == TouchPhase.Moved && isTouched)
        {
            Ray ray = arCamera.ScreenPointToRay(touch.position); // 터치 위치로부터 Ray 생성
            RaycastHit hit;

            // Raycast로 Ground를 먼저 감지
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _groundMask))
            {
                selectedObj.transform.position = hit.point;
                //// Raycast로 충돌한 오브젝트가 "TargetTree"라는 이름인 경우
                //if (hit.collider.gameObject.name == "TargetTree")
                //{
                //    Debug.LogWarning("[Debug] : TargetTree hit!");
                //    // 나무의 위치와 정면 방향을 가져옵니다
                //    Vector3 treePosition = hit.collider.gameObject.transform.position;
                //    Vector3 treeForward = hit.collider.gameObject.transform.forward;

                //    // 코기를 나무의 정면에 고정시킵니다 (treeDetectionDistance는 고정 거리)
                //    selectedObj.transform.position = treePosition + treeForward * treeDetectionDistance;

                //    // 코기를 나무의 자식으로 설정하여 고정 상태로 만듭니다
                //    selectedObj.transform.SetParent(hit.collider.transform);

                //    // 코기가 나무에 고정되었음을 표시
                //    isCorgiOnTree = true;

                //    // 로그 출력 (디버깅용)
                //    Debug.LogWarning("[Debug] : Corgi has been fixed to the front of the tree!");
                //}
                //else
                //{
                //    // 나무가 아닌 다른 오브젝트에는 코기를 자유롭게 이동
                //    selectedObj.transform.position = hit.point;
                //}
            }
        }

        // 터치가 끝날 때
        if (touch.phase == TouchPhase.Ended)
        {
            if (selectedObj != null)
            {
                isTouched = false; // 오브젝트 선택 해제
                selectedObj.layer = LayerMask.NameToLayer("ARSelectable"); // 터치가 끝나면 오브젝트의 레이어를 ARSelectable로 설정
                selectedObj.transform.position = initialPosition; // 오브젝트를 초기 위치로 되돌리기
                selectedObj.transform.SetParent(null); // 부모를 해제하여 자유롭게 이동
                selectedObj = null;
                Debug.LogWarning("[Debug] : object deselected");
            }
        }
    }
}