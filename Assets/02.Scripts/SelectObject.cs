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

    [SerializeField] LayerMask _selectedMask;
    [SerializeField] LayerMask _groundMask;

    bool isHandling = false;

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
        Debug.Log($"[{nameof(SelectObject)}] Touched.");


        if (touch.phase == TouchPhase.Began) // 터치가 시작될 때
        {
            Ray ray = arCamera.ScreenPointToRay(touch.position); // 터치 위치로부터 Ray 생성
            RaycastHit hit;


            if (Physics.Raycast(ray, out hit, _selectedMask)) // Ray가 Collider와 충돌했는지 확인
            {
                if (hit.collider.name.Contains("corgi")) // 충돌한 오브젝트 이름에 "corgi"가 포함되었는지 확인
                {
                    selectedObj = hit.collider.gameObject; // 선택된 오브젝트로 저장
                    isTouched = true; // 오브젝트가 터치됨을 표시
                    selectedObj.layer = LayerMask.NameToLayer("ARSelected"); // 오브젝트의 레이어를 ARSelected로 설정
                    Debug.Log(" object selected");
                }
            }
        }

        if (touch.phase == TouchPhase.Moved && isTouched) // 터치가 이동 중이고 오브젝트가 선택되었을 때
        {
            Ray ray = arCamera.ScreenPointToRay(touch.position); // 터치 위치로부터 Ray 생성
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _groundMask)) // 평면에 대해 Raycast 실행
            {

                selectedObj.transform.position = hit.point; // 선택된 오브젝트를 평면 위치로 이동
            }
        }

        if (touch.phase == TouchPhase.Ended) // 터치가 끝날 때
        {

            isTouched = false; // 오브젝트 선택 해제
            if (selectedObj != null)
            {
                selectedObj.layer = LayerMask.NameToLayer("ARSelectable"); // 터치가 끝나면 오브젝트의 레이어를 ARSelectable로 설정
                selectedObj = null;
                Debug.Log(" object deselected");
            }
        }
    }
}
