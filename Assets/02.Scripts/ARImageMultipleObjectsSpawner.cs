using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using System.Collections;

[System.Serializable]
public struct ChapterData
{
    public string chapterName;          // 챕터 이름 (예: "chap1")
    public GameObject prefab;           // 해당 챕터에 대응하는 프리팹 오브젝트
    public string[] imageNames;         // 이 챕터에 대응하는 이미지 이름 배열
    public bool useSpeechRecognition;   // 음성 인식 기능 사용 여부
}

[RequireComponent(typeof(ARTrackedImageManager))]
public class ARImageMultipleObjectsSpawner : MonoBehaviour
{
    private ARTrackedImageManager _trackedImageManager;
    private GameObject _coverAnchor; // ARAnchor 대신 GameObject로 변경

    [Tooltip("각 챕터에 대응하는 데이터 설정")]
    public ChapterData[] chapters;

    private Dictionary<string, ChapterData> _imageNameToChapter = new Dictionary<string, ChapterData>();
    private Dictionary<string, GameObject> _spawnedPrefabs = new Dictionary<string, GameObject>();
    private HashSet<string> _trackedImages = new HashSet<string>();
    private string _currentChapter = "";
    private IChapterController _currentChapterController = null; // 추가: 현재 챕터 컨트롤러

    //Tracking상태
    private TrackingState _previousTrackingState = TrackingState.None;
    private float _trackingStateChangeTime = 0f;
    private float _trackingStateStableDuration = 0.5f; // 상태가 안정적으로 유지되어야 하는 시간
    private bool _isTrackingMessageVisible = false;

    //Add
    private bool _isTracking = false;
    private bool _isTimer = false;
    private float _timer = 0;
    private float _maxTimer = 5f;
    public GameObject testCube;

    private bool _isCoverAnchorSet = false; // 커버 앵커가 완료되었는지 여부

    void Start()
    {
        // 필요한 초기화 코드
    }

    private void Awake()
    {
        // ARTrackedImageManager 컴포넌트 가져오기
        _trackedImageManager = GetComponent<ARTrackedImageManager>();

        // chapters 배열이 비어있는지 확인
        if (chapters == null || chapters.Length == 0)
        {
            Debug.LogError("Chapters 배열이 비어 있습니다. Inspector에서 챕터 데이터를 설정하세요.");
            return;
        }

        // 이미지 이름을 챕터 데이터에 매핑하고 프리팹을 미리 생성하여 비활성화
        foreach (ChapterData chapter in chapters)
        {
            if (string.IsNullOrEmpty(chapter.chapterName))
            {
                Debug.LogWarning("챕터 이름이 null 또는 비어 있습니다. 해당 챕터를 건너뜁니다.");
                continue;
            }

            if (chapter.prefab == null)
            {
                Debug.LogWarning($"챕터 '{chapter.chapterName}'의 프리팹이 null입니다. 해당 챕터를 건너뜁니다.");
                continue;
            }

            if (chapter.imageNames == null || chapter.imageNames.Length == 0)
            {
                Debug.LogWarning($"챕터 '{chapter.chapterName}'의 이미지 이름 배열이 비어 있습니다. 해당 챕터를 건너뜁니다.");
                continue;
            }

            // 프리팹 생성 및 비활성화
            GameObject instantiated = Instantiate(chapter.prefab, Vector3.zero, Quaternion.identity);
            instantiated.name = chapter.chapterName;
            instantiated.SetActive(false);

            // 스폰된 프리팹 딕셔너리에 추가
            if (!_spawnedPrefabs.ContainsKey(chapter.chapterName))
            {
                _spawnedPrefabs.Add(chapter.chapterName, instantiated);
            }
            else
            {
                Debug.LogWarning($"_spawnedPrefabs 딕셔너리에 '{chapter.chapterName}' 키가 이미 존재합니다.");
                Destroy(instantiated);
            }

            // 이미지 이름을 챕터 데이터에 매핑
            foreach (string imageName in chapter.imageNames)
            {
                if (string.IsNullOrEmpty(imageName))
                {
                    Debug.LogWarning($"챕터 '{chapter.chapterName}'의 이미지 이름이 null 또는 비어 있습니다. 해당 이미지를 건너뜁니다.");
                    continue;
                }

                if (!_imageNameToChapter.ContainsKey(imageName))
                {
                    _imageNameToChapter.Add(imageName, chapter);
                }
                else
                {
                    Debug.LogWarning($"이미지 이름 '{imageName}'이 이미 다른 챕터에 매핑되어 있습니다.");
                }
            }
        }
    }

    private void OnEnable()
    {
        if (_trackedImageManager != null)
        {
            // 이벤트 구독
            _trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
        }
        else
        {
            Debug.LogError("ARTrackedImageManager가 할당되지 않았습니다.");
        }
    }

    private void OnDisable()
    {
        if (_trackedImageManager != null)
        {
            // 이벤트 해제
            _trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
        }
    }

    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        // 추가된 이미지 처리
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            if (trackedImage.referenceImage.name == "chap0" && !_isCoverAnchorSet)
            {
                CenterPosition(trackedImage);
            }

            // 커버 앵커가 설정되지 않았을 경우 메시지 표시
            if (!_isCoverAnchorSet)
            {
                UIManager.instance.ShowMessage("커버 이미지를 인식하도록 설정해 주세요.");
            }

            LogTrackingState(trackedImage);
            HandleTrackedImage(trackedImage);
        }

        // 업데이트된 이미지 처리
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            LogTrackingState(trackedImage);
            HandleTrackedImage(trackedImage);
        }

        // 제거된 이미지 처리
        foreach (KeyValuePair<TrackableId, ARTrackedImage> trackedImage in eventArgs.removed)
        {
            string imageName = trackedImage.Value.referenceImage.name;

            if (_trackedImages.Contains(imageName))
            {
                _trackedImages.Remove(imageName);
                Debug.Log($"ImageTrack '{imageName}' tracking loss.");

                UpdateChapterState();
            }
        }
    }

    /// <summary>
    /// TrackingState 이벤트 메시지
    /// </summary>
    /// <param name="trackedImage"></param>
    private void LogTrackingState(ARTrackedImage trackedImage)
    {
        TrackingState currentTrackingState = trackedImage.trackingState;


        //TrackingState
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            Debug.Log($"ImageTrack '{trackedImage.referenceImage.name}'It's being tracked.");
        }
        else
        {
            Debug.Log($"ImageTrack '{trackedImage.referenceImage.name}'the tracking status of: {trackedImage.trackingState}");
        }


        // 상태가 변경되었을 때만 처리
        if (_previousTrackingState != currentTrackingState)
        {
            _previousTrackingState = currentTrackingState;
            _trackingStateChangeTime = Time.time; // 상태 변경 시간 기록
            _isTrackingMessageVisible = false;    // 메시지 표시 여부 초기화
        }

        // 상태가 안정적으로 유지되었을 때 메시지 표시
        if (!_isTrackingMessageVisible && Time.time - _trackingStateChangeTime >= _trackingStateStableDuration)
        {
            string message = "";

            if (currentTrackingState == TrackingState.Tracking)
            {
                message = "가상 콘텐츠를 이미지 위에 배치하고 상호작용을 시작합니다.";
            }
            else if (currentTrackingState == TrackingState.Limited)
            {
                message = "카메라와 이미지의 거리를 줄이거나 각도를 조정해 주세요.";
            }
            else if (currentTrackingState == TrackingState.None)
            {
                message = "이미지 추적을 시작하려면 카메라를 이미지에 맞춰주세요.";
            }

            // 메시지가 비어있지 않으면 표시
            if (!string.IsNullOrEmpty(message))
            {
                UIManager.instance.TreackingStateMessage(message);
                _isTrackingMessageVisible = true;

                // 일정 시간 후에 메시지 숨김
                StartCoroutine(HideTrackingMessageAfterDelay(2f));
            }
        }
    }

    private IEnumerator HideTrackingMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UIManager.instance.HideTrackingStateMessage();
        _isTrackingMessageVisible = false;
    }

    private void HandleTrackedImage(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        if (!_imageNameToChapter.ContainsKey(imageName))
        {
            Debug.LogWarning($"이미지 이름 '{imageName}'이 어떤 챕터에도 매핑되어 있지 않습니다.");
            return;
        }

        // 커버 앵커가 완료되지 않았을 경우 처리
        if (!_isCoverAnchorSet)
        {
            // UIManager를 통해 안내 메시지 표시
            UIManager.instance.ShowMessage("커버 이미지를 인식하여 앵커를 설정해 주세요.");
            return;
        }

        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            if (!_trackedImages.Contains(imageName))
            {
                _trackedImages.Add(imageName);
                Debug.Log($"이미지 '{imageName}' 추적 시작.");
            }

            UpdateChapterState(trackedImage);
        }
        else
        {
            if (_trackedImages.Contains(imageName))
            {
                //Add
                _isTimer = true;

                _trackedImages.Remove(imageName);
                Debug.Log($"이미지 '{imageName}' 추적 손실.");

                UpdateChapterState();
            }
        }
    }

    private void CenterPosition(ARTrackedImage trackedImage)
    {
        StartCoroutine(CoverAnchorDelay(trackedImage));
    }

    private IEnumerator CoverAnchorDelay(ARTrackedImage trackedImage)
    {
        _coverAnchor = new GameObject("CoverAnchor"); // ARAnchor 제거
        yield return new WaitForSeconds(0.2f);

        Vector3 bookCenterPosition = trackedImage.transform.position - trackedImage.transform.right * (trackedImage.size.x / 2);
        _coverAnchor.transform.position = bookCenterPosition;
        _coverAnchor.transform.rotation = trackedImage.transform.rotation;

        // 앵커가 설정되었음을 표시
        _isCoverAnchorSet = true;

        UIManager.instance.ShowMessage("커버 이미지를 \n 인식하였습니다.");

        // UIManager를 통해 메시지 숨김
       // UIManager.instance.HideMessage();
    }

    private void UpdateChapterState(ARTrackedImage trackedImage = null)
    {
        // 추적된 이미지에 기반하여 활성 챕터 수집
        HashSet<string> activeChapters = new HashSet<string>();

        foreach (string imageName in _trackedImages)
        {
            ChapterData chapter = _imageNameToChapter[imageName];
            activeChapters.Add(chapter.chapterName);
        }

        if (activeChapters.Count > 0)
        {
            string newCurrentChapter = "";

            // 첫 번째 활성 챕터 선택
            foreach (string chapterName in activeChapters)
            {
                newCurrentChapter = chapterName;
                break;
            }

            if (_currentChapter != newCurrentChapter)
            {
                // 이전 챕터 비활성화
                if (!string.IsNullOrEmpty(_currentChapter))
                {
                    GameObject previousPrefab = _spawnedPrefabs[_currentChapter];
                    previousPrefab.SetActive(false);

                    //Add
                    _isTracking = false;
                    _isTimer = false;
                    _timer = 0;

                    // 현재 챕터 컨트롤러 초기화
                    _currentChapterController = null;

                    Debug.Log($"PreChapter '{_currentChapter}' UnActive.");
                }

                // 새로운 챕터 활성화
                GameObject currentPrefab = _spawnedPrefabs[newCurrentChapter];
                currentPrefab.SetActive(true);

                //Add
                _isTracking = true;
                _isTimer = false;
                _timer = 0;

                if (trackedImage != null)
                {
                    if (_coverAnchor == null)
                    {
                        currentPrefab.transform.position = trackedImage.transform.position;
                        currentPrefab.transform.rotation = trackedImage.transform.rotation;
                    }
                    else
                    {
                        currentPrefab.transform.position = _coverAnchor.transform.position;
                        currentPrefab.transform.rotation = _coverAnchor.transform.rotation;
                    }
                }

                Debug.Log($"NewChpter '{newCurrentChapter}' Active.");

                _currentChapter = newCurrentChapter;

                // 현재 챕터 컨트롤러 가져오기
                _currentChapterController = currentPrefab.GetComponent<IChapterController>();
                if (_currentChapterController == null)
                {
                    Debug.LogWarning($"Chapter controller not found on prefab '{newCurrentChapter}'.");
                }

                // 음성 인식 기능 활성화
                ChapterData currentChapterData = GetChapterDataByName(newCurrentChapter);
                if (currentChapterData.useSpeechRecognition)
                {
                    Debug.Log("Enabling Speech Recognition");
                    // 음성 인식 관련 코드 추가
                }
            }
            else
            {
                Debug.Log($"Keep '{newCurrentChapter}' current chapter.");
                // 현재 챕터의 위치와 회전을 업데이트
                GameObject currentPrefab = _spawnedPrefabs[_currentChapter];
                if (trackedImage != null)
                {
                    if (_coverAnchor == null)
                    {
                        currentPrefab.transform.position = trackedImage.transform.position;
                        currentPrefab.transform.rotation = trackedImage.transform.rotation;
                    }
                    else
                    {
                        currentPrefab.transform.position = _coverAnchor.transform.position;
                        currentPrefab.transform.rotation = _coverAnchor.transform.rotation;
                    }
                }
            }
        }
        else
        {
            // 활성 챕터가 없는 경우
            if (!string.IsNullOrEmpty(_currentChapter) && !_isTracking)
            {
                GameObject currentPrefab = _spawnedPrefabs[_currentChapter];
                currentPrefab.SetActive(false);

                Debug.Log($"Chapter '{_currentChapter}' UnActive.");

                // 음성 인식 기능 비활성화
                ChapterData currentChapterData = GetChapterDataByName(_currentChapter);
                if (currentChapterData.useSpeechRecognition)
                {
                    // 음성 인식 관련 코드 추가
                }

                _currentChapter = "";
            }
        }
    }

    private ChapterData GetChapterDataByName(string chapterName)
    {
        foreach (ChapterData chapter in chapters)
        {
            if (chapter.chapterName == chapterName)
            {
                return chapter;
            }
        }

        return default;
    }

    private void Update()
    {
        //Add
        if (_isTimer)
        {
            _timer += Time.deltaTime;
        }

        if (_isTracking)
        {
            if (_timer > _maxTimer)
            {
                _isTracking = false;
                _timer = 0;
                _isTimer = false;
                UpdateChapterState();
            }
        }

        // 터치 입력 감지하여 나레이션 일시정지/재개
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // UI 요소 위의 터치는 무시
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            // 현재 챕터에서 음성 인식 기능을 사용 중이면 터치 입력을 무시
            ChapterData currentChapterData = GetChapterDataByName(_currentChapter);
            if (currentChapterData.useSpeechRecognition)
                return;

            if (touch.phase == TouchPhase.Began)
            {
                if (_currentChapterController != null)
                {
                    _currentChapterController.TogglePause();
                }
                else
                {
                    Debug.LogWarning("No current chapter controller to toggle pause.");
                }
            }
        }

        // 에디터에서 마우스 클릭으로 테스트하기 위한 코드
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            // UI 요소 위의 클릭은 무시
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            // 현재 챕터에서 음성 인식 기능을 사용 중이면 마우스 입력을 무시
            ChapterData currentChapterData = GetChapterDataByName(_currentChapter);
            if (currentChapterData.useSpeechRecognition)
                return;

            if (_currentChapterController != null)
            {
                _currentChapterController.TogglePause();
            }
            else
            {
                Debug.LogWarning("No current chapter controller to toggle pause.");
            }
        }
#endif
    }
}
