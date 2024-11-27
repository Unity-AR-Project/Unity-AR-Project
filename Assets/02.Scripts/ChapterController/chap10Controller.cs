using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.EventSystems;

public class chap10Controller : MonoBehaviour, IChapterController
{
    // 변수 및 컴포넌트 선언
    private bool isTouched = false;
    private GameObject selectedObj;
    private Camera arCamera; // AR Camera 연결 필요
    [SerializeField] private LayerMask _selectMask; // 터치 가능한 오브젝트 레이어
    [SerializeField] private LayerMask _groundMask; // 이동 가능한 바닥 레이어

    private Vector3 initialPosition;

    [SerializeField] private PlayableDirector playableDirector; // 타임라인 연결

    private bool isPaused = false; // 일시정지 상태 여부
    private const float PauseTime = 2.2f; // 타임라인 중단 시간

    // 레이어 및 태그 상수 정의
    private const string SELECTABLE_LAYER = "ARSelectable";
    private const string SELECTED_LAYER = "ARSelected";
    private const string WOLF_TAG = "Wolf";
    private const string CHIMNEY_TAG = "Chimney";

    //프리팹 초기화
   /* [SerializeField] private GameObject chapter10Prefab; // 챕터 7 프리팹
    [SerializeField] private Transform prefabParent; // 프리팹을 인스턴스화할 부모 오브젝트
    private GameObject chapter10Instance; // 현재 활성화된 챕터 7 인스턴스*/

    void OnEnable()
    {/*
        if (chapter10Instance != null)
        {
            Destroy(chapter10Instance);
        }

        // 챕터 10 프리팹 인스턴스화
        if (chapter10Prefab != null && prefabParent != null)
        {
            chapter10Instance = Instantiate(chapter10Prefab, prefabParent);
            chapter10Instance.tag = "Chapter10Instance"; // 필요 시 태그 설정
            chapter10Instance.SetActive(true);
            Debug.Log("[chap10Controller] Chapter10 prefab instantiated.");
        }
        else
        {
            Debug.LogError("[chap10Controller] Chapter10Prefab or PrefabParent is not assigned.");
        }

        // 타임라인 초기 설정: 재생하지 않고 대기 상태로 설정
        if (playableDirector != null)
        {*/
            // 타임라인 시작
            playableDirector.time = 0; // 타임라인 시간 초기화
            playableDirector.Stop();   // 타임라인 정지
            playableDirector.Play();   // 타임라인 재생
     /*   }
        else
        {
            Debug.LogError("[chap10Controller] PlayableDirector not assigned.");
        }*/
    }

    private void Start()
    {
        // AR 카메라 설정
        if (arCamera == null)
        {
            arCamera = Camera.main;
            if (arCamera == null)
            {
                Debug.LogError("[chap10Controller] AR Camera not assigned.");
            }
        }

        // 타임라인 이벤트 설정
        if (playableDirector != null)
        {
            playableDirector.stopped += OnTimelineStopped; // 타임라인 종료 이벤트 등록
            playableDirector.played += OnTimelinePlayed;   // 타임라인 시작 이벤트 등록
            playableDirector.time = 0; // 타임라인 시간 초기화
            playableDirector.Play();   // 타임라인 재생
            // Signal Track을 통해 타임라인 중단을 처리하므로 Invoke는 제거
        }
        else
        {
            Debug.LogError("[chap10Controller] PlayableDirector not assigned.");
        }

        // 초기 메시지 표시
        UIManager.instance.ShowMessage("10챕터 시작되었습니다. 기다려주세요!");

        // 재추적 시 오브젝트 상태 초기화
        GameObject wolf = GameObject.FindWithTag(WOLF_TAG);
        if (wolf != null)
        {
            wolf.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[chap10Controller] Wolf object not found in the scene.");
        }
    }

    private void Update()
    {
        if (!isPaused || Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        // UI 요소 위의 터치는 무시
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
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
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _selectMask))
        {
            if (hit.collider.CompareTag(WOLF_TAG))
            {
                selectedObj = hit.collider.gameObject;
                initialPosition = selectedObj.transform.position;
                isTouched = true;
                selectedObj.layer = LayerMask.NameToLayer(SELECTED_LAYER);
                selectedObj.GetComponent<Collider>().enabled = false; // 콜라이더 비활성화
                Debug.Log($"[chap10Controller] {selectedObj.name} selected.");

                // UI 메시지 표시
                UIManager.instance.ShowMessage("굴뚝으로 늑대를 이동시켜주세요!");
            }
        }
    }

    private void OnTouchMoved(Touch touch)
    {
        if (!isTouched || selectedObj == null) return;

        Ray ray = arCamera.ScreenPointToRay(touch.position);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundMask))
        {
            if (hit.collider.CompareTag(CHIMNEY_TAG))
            {
                Debug.Log($"[chap10Controller] {selectedObj.name} reached the Chimney!");

                // 늑대 비활성화
                selectedObj.SetActive(false);

                // 타임라인 및 UI 재개
                ResumeTimeline();
            }
            else
            {
                // 굴뚝이 아닌 곳으로 이동
                selectedObj.transform.position = hit.point;
                Debug.Log($"[chap10Controller] {selectedObj.name} moved to {hit.point}");
            }
        }
    }

    private void OnTouchEnded(Touch touch)
    {
        if (!isTouched || selectedObj == null) return;

        isTouched = false;
        selectedObj.layer = LayerMask.NameToLayer(SELECTABLE_LAYER);
        selectedObj.GetComponent<Collider>().enabled = true; // 콜라이더 재활성화

        // 오브젝트의 위치를 원래대로 되돌리지 않음 (사용자가 원하는 위치에 오브젝트를 두도록)
        selectedObj = null;

        // UI 메시지 숨김
        UIManager.instance.HideMessage();

        Debug.Log("[chap10Controller] Object deselected.");
    }

    private void ResumeTimeline()
    {
        if (playableDirector != null)
        {
            playableDirector.Play();
            isPaused = false;

            // UI 메시지 숨김
            UIManager.instance.HideMessage();

            Debug.Log("[chap10Controller] Timeline resumed.");
        }
    }

    // Signal을 통해 호출되는 메서드
    public void PauseTimelineAtSpecificTime()
    {
        if (playableDirector != null)
        {
            playableDirector.Pause();
            isPaused = true;

            // UI 메시지 표시
            UIManager.instance.ShowMessage("굴뚝에 늑대를 옮겨주세요!");

            Debug.Log("[chap10Controller] Timeline paused at 2.2 seconds.");
        }
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        if (director == playableDirector)
        {
            Debug.Log("[chap10Controller] Timeline has ended.");
        }
    }

    private void OnTimelinePlayed(PlayableDirector director)
    {
        Debug.Log("[chap10Controller] Timeline started.");
    }

    /// <summary>
    /// 타임라인 일시정지/재개 토글
    /// </summary>
    public void TogglePause()
    {
        if (playableDirector == null) return;

        if (isPaused)
        {
            playableDirector.Play();
            isPaused = false;

            // UI 메시지 숨김
            UIManager.instance.HideMessage();

            Debug.Log("[chap10Controller] Timeline resumed.");
        }
        else
        {
            playableDirector.Pause();
            isPaused = true;

            // UI 메시지 표시
            UIManager.instance.ShowMessage("타임라인이 일시정지되었습니다.");

            Debug.Log("[chap10Controller] Timeline paused.");
        }
    }
}
