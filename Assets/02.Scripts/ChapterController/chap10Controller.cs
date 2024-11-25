using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class chap10Controller : MonoBehaviour , IChapterController
{
    private bool isTouched = false;
    private GameObject selectedObj;
    private Camera arCamera; // AR Camera 연결 필요
    [SerializeField] private LayerMask _selectMask; // 터치 가능한 오브젝트 레이어
    [SerializeField] private LayerMask _groundMask; // 이동 가능한 바닥 레이어

    private Vector3 initialPosition;

    [SerializeField] private PlayableDirector timelineDirector; // 타임라인 연결
    //public GameObject uiText; // UI 텍스트 오브젝트 (안내 메시지)
    private bool isPaused = false; // 일시정지 상태 여부
    void OnEnable()
    {
        // 타임라인 시작
        timelineDirector.time = 0; // 타임라인 시간 초기화
        timelineDirector.Stop();   // 타임라인 정지
        timelineDirector.Play();   // 타임라인 재생
    }


    private void Start()
    {
        // AR 카메라 설정
        if (arCamera == null)
        {
            arCamera = Camera.main;
            if (arCamera == null)
            {
                Debug.LogError("[Debug] : AR Camera not assigned.");
            }
        }

        // UI 텍스트를 시작 시 비활성화
        /* if (uiText != null)
         {
            // uiText.SetActive(true);
         }*/
        UIManager.instance.ShowMessage("돼지 형제들을 나무 위로 올라갔어요!");


        // 타임라인 초기화
        if (timelineDirector != null)
        {
            timelineDirector.Stop(); // 타임라인 초기 정지 상태로 설정
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

        // 터치 시작
        if (touch.phase == TouchPhase.Began)
        {
            Ray ray = arCamera.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _selectMask))
            {
                if (hit.collider.gameObject.name == "Wolf") // 늑대가 선택되었는지 확인
                {
                    selectedObj = hit.collider.gameObject;
                    initialPosition = selectedObj.transform.position;
                    isTouched = true;
                    selectedObj.layer = LayerMask.NameToLayer("ARSelected");
                    Debug.Log($"[Debug] : {selectedObj.name} selected.");
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
                if (hit.collider.gameObject.name == "Chimney") // 굴뚝에 도달했는지 확인
                {
                    Debug.Log($"[Debug] : {selectedObj.name} reached the Chimney!");

                    // 늑대 비활성화
                    selectedObj.SetActive(false);

                    // 타임라인 실행
                    if (timelineDirector != null)
                    {
                        timelineDirector.Play();
                    }

                   /* if (uiText != null)
                    {
                        uiText.SetActive(false); // UI 텍스트 비활성화
                    }*/
                }
                else
                {
                    // 나무가 아닌 곳으로 이동
                    selectedObj.transform.position = hit.point;
                    Debug.Log($"[Debug] : {selectedObj.name} moved to {hit.point}");
                }
            }
        }


        // 터치 종료
        if (touch.phase == TouchPhase.Ended && selectedObj != null)
        {
            isTouched = false;
            selectedObj.layer = LayerMask.NameToLayer("ARSelectable");
            selectedObj.SetActive(false);
        }
    }
    /// <summary>
    /// 타임라인 일시정지/재개 토글
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
