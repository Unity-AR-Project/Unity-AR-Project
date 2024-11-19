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

    private Vector3 initialPosition; // 초기 위치를 저장할 변수 추가

    private void Start()
    {
        if (arCamera == null)
        {
            Debug.LogError("AR Camera not assigned."); // AR 카메라가 할당되지 않았을 경우 에러 출력
        }
    }

    private void Update()
    {
        if (Input.touchCount == 0) return; // 터치가 없으면 함수 종료

        Touch touch = Input.GetTouch(0); // 첫 번째 터치 입력을 가져옴
        Debug.LogWarning($"[{nameof(SelectObject)}] Touched.");


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
                    Debug.LogWarning("object selected");
                }
            }
        }
        // 터치로 corgi 선택 시 콜라이더를 비활성화합니다.
        if (isTouched)
        {
            selectedObj.GetComponent<Collider>().enabled = false; // 선택된 객체의 콜라이더 비활성화
        }

        if (touch.phase == TouchPhase.Moved && isTouched) // 터치가 이동 중이고 오브젝트가 선택되었을 때
        {
            Debug.LogWarning("object selected & touched");
            Ray ray = arCamera.ScreenPointToRay(touch.position); // 터치 위치로부터 Ray 생성
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _groundMask)) // 평면에 대해 Raycast 실행
            {

                // Raycast hit을 정밀하게 처리
                //Vector3 hitPoint = new Vector3(hit.point.x, selectedObj.transform.position.y, hit.point.z);
                //selectedObj.transform.position = hitPoint;
                Debug.LogWarning($"Hit on drag {hit.collider.gameObject}");
                selectedObj.transform.position = hit.point; // 선택된 오브젝트를 평면 위치로 이동
            }
        }

        // 터치가 끝날 때
        if (touch.phase == TouchPhase.Ended)
        {
            if (selectedObj != null)
            {
                selectedObj.GetComponent<Collider>().enabled = true; // 콜라이더 재활성화
                isTouched = false; // 오브젝트 선택 해제
                selectedObj.layer = LayerMask.NameToLayer("ARSelectable"); // 터치가 끝나면 오브젝트의 레이어를 ARSelectable로 설정
                selectedObj.transform.position = initialPosition; // 오브젝트를 초기 위치로 되돌리기
                selectedObj = null;
                Debug.LogWarning("object deselected");
            }
        }
    }
}
