using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class chap10Controller : MonoBehaviour
{
    private bool isTouched = false;
    private GameObject selectedObj;
    [SerializeField] private Camera arCamera; // AR Camera 연결 필요
    [SerializeField] private LayerMask _selectMask; // 터치 가능한 오브젝트 레이어
    [SerializeField] private LayerMask _groundMask; // 이동 가능한 바닥 레이어

    private Vector3 initialPosition;

    [SerializeField] private PlayableDirector timelineDirector; // 타임라인 연결
    [SerializeField] private GameObject uiText; // UI 텍스트 오브젝트 (안내 메시지)

    private bool isPaused = false; // 타임라인이 중단되었는지 여부
    private const double PauseTime = 2.2; // 타임라인 중단 시간

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
        if (uiText != null)
        {
            uiText.SetActive(false);
        }

        // 타임라인 초기화
        if (timelineDirector != null)
        {
            timelineDirector.stopped += OnTimelineStopped;
            timelineDirector.Play(); // 타임라인 실행
            Invoke(nameof(PauseTimelineAtSpecificTime), (float)PauseTime);
        }
        else
        {
            Debug.LogError("[Debug] : PlayableDirector not assigned.");
        }

        AudioListener.pause = false;
    }

    private void Update()
    {
        if (!isPaused || Input.touchCount == 0) return;

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

                    // 타임라인 및 오디오 재개
                    ResumeTimeline();
                }
                else
                {
                    // 굴뚝이 아닌 곳으로 이동
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
        }
    }

    private void PauseTimelineAtSpecificTime()
    {
        if (timelineDirector != null)
        {
            timelineDirector.Pause();
            isPaused = true;

            if (uiText != null)
            {
                uiText.SetActive(true);
                uiText.GetComponent<Text>().text = "굴뚝에 늑대를 옮겨주세요!";
            }

            Debug.Log("[Debug] : Timeline paused at 2.2 seconds.");
        }
    }

    private void ResumeTimeline()
    {
        if (timelineDirector != null)
        {
            timelineDirector.Play();
            isPaused = false;
            AudioListener.pause = false; // 오디오 재개
            

            if (uiText != null)
            {
                uiText.SetActive(false);
            }

            Debug.Log("[Debug] : Timeline and narration resumed.");
        }
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        if (director == timelineDirector)
        {
            Debug.Log("[Debug] : Timeline has ended.");
        }
    }

    public void PauseAudio()
    {
        AudioListener.pause = true; //오디오 일시정지
    }
}
