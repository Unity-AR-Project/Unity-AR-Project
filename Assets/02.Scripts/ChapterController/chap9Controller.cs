using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;
using UnityEngine.Audio;

public class chap9Controller : MonoBehaviour, IChapterController
{
    [SerializeField] private LayerMask _selectMask; // 선택 가능한 오브젝트 레이어
    [SerializeField] private LayerMask _groundMask; // 이동 가능한 바닥 레이어

    [Header("Timeline & UI")]
    public PlayableDirector playableDirector; // 타임라인 제어를 위한 PlayableDirector
    public TextMeshProUGUI uiText; // UI 텍스트 오브젝트 (안내 메시지)

    [Header("Drag Settings")]
    private GameObject selectedObj;
    private Vector3 initialPosition;
    private bool isTouched = false;

    private bool isPaused = false; // 타임라인 멈춤 상태 추적
    private const double PauseTime = 15.8; // 타임라인 멈출 시점


   /* //프리팹 초기화
    [SerializeField] private GameObject chapter9Prefab; // 챕터 7 프리팹
    [SerializeField] private Transform prefabParent; // 프리팹을 인스턴스화할 부모 오브젝트
    private GameObject chapter9Instance; // 현재 활성화된 챕터 7 인스턴스
*/
    void OnEnable()
    {
     /*   if (chapter9Instance != null)
        {
            Destroy(chapter9Instance);
        }

        // 챕터 9 프리팹 인스턴스화
        if (chapter9Prefab != null && prefabParent != null)
        {
            chapter9Instance = Instantiate(chapter9Prefab, prefabParent);
            chapter9Instance.tag = "Chapter1Instance"; // 필요 시 태그 설정
            chapter9Instance.SetActive(true);
            Debug.Log("[chap9Controller] Chapter9 prefab instantiated.");
        }
        else
        {
            Debug.LogError("[chap9Controller] Chapter9Prefab or PrefabParent is not assigned.");
        }
        if (playableDirector != null)
        {*/
            // 타임라인 시작
            playableDirector.time = 0; // 타임라인 시간 초기화
            playableDirector.Stop();   // 타임라인 정지
                                       // 초기 메시지 표시
        UIManager.instance.ShowMessage("9챕터 시작되었습니다.\n" +
            "기다려주세요!");

        playableDirector.Play();   // 타임라인 재생
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

        // 터치 시작
        if (touch.phase == TouchPhase.Began)
        {
            Debug.LogWarning("[Debug] : chap9 Start Touch");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _selectMask))
            {
                // 사과가 선택되었는지 확인
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

        // 터치 이동
        if (touch.phase == TouchPhase.Moved && isTouched && selectedObj != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _groundMask))
            {
                if (hit.collider.gameObject.name == "Wolf_5")
                {
                    Debug.LogWarning($"[Debug] : {selectedObj.name} hit TargetTree!");

                    // 고정 위치 설정
                    Vector3 fixedPosition = GetFixedPositionForObject(selectedObj.name);
                    selectedObj.transform.localPosition = fixedPosition;

                    // 사과 상태 업데이트
                    selectedObj.GetComponent<AppleStatus>().isOnWolf = true;
                    Debug.LogWarning($"[Debug] : {selectedObj.name} is now on wolf: {selectedObj.GetComponent<AppleStatus>().isOnWolf}");

                    Debug.LogWarning($"[Debug] : {selectedObj.name} fixed at {fixedPosition}");

                    // 모든 사과가 늑대에 고정되었는지 확인
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
            case "Apple1": return new Vector3(13f, 0f, -5.26f);
            case "Apple2": return new Vector3(12.87f, 0f, -1.46f);
            case "Apple3": return new Vector3(14.9f, 1.78f, -3.03f);
            default: return Vector3.zero; // 기본 위치는 0, 0, 0
        }
    }

    // 모든 사과가 늑대에 고정되었는지 확인
    private void CheckAllApplesOnWolf()
    {
        bool allApplesOnWolf = GameObject.FindGameObjectsWithTag("Apple").All(apple => apple.GetComponent<AppleStatus>().isOnWolf);
        Debug.LogWarning($"[Debug] : All Apples on Wolf: {allApplesOnWolf}");

        if (allApplesOnWolf)
        {

            if (uiText != null)
            {
                uiText.gameObject.SetActive(false); // 모든 사과가 고정되면 UI 숨김
            }

            // 타임라인 및 오디오 재개
            ResumeTimeline();

            Debug.LogWarning("[Debug] : 모든 사과가 늑대에 고정되었습니다. 나레이션 재생 시작.");
        }
    }

    private void ResumeTimeline()
    {
        if (playableDirector != null)
        {
            // UI 메시지 표시
            UIManager.instance.ShowMessage("잘했어요!");

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


            UIManager.instance.ShowMessage("사과 3개를 \n 늑대에게 던져주세요~");
            
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
    /// 타임라인 일시정지/재개 토글
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
