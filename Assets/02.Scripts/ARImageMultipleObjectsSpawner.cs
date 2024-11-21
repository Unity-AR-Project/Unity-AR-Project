using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 챕터 데이터 구조체: 챕터 이름, 프리팹, 이미지 이름들을 포함
/// </summary>
[System.Serializable]
public struct ChapterData
{
    public string chapterName;          // 챕터 이름 (예: "chap1")
    public GameObject prefab;           // 해당 챕터에 대응하는 프리팹 오브젝트
    public string[] imageNames;         // 이 챕터에 대응하는 이미지 이름 배열
    public bool useSpeechRecognition;   // 음성 인식 기능 사용 여부
}

/// <summary>
/// 여러 이미지를 인식하여 대응되는 챕터의 오브젝트를 관리하고 나레이션을 재생하는 클래스
/// </summary>
[RequireComponent(typeof(ARTrackedImageManager))]
public class ARImageMultipleObjectsSpawner : MonoBehaviour
{

    private ARTrackedImageManager _trackedImageManager;
    //Add
    //public ARSession arSession;
    private ARAnchor _coverAnchor;

    [Tooltip("각 챕터에 대응하는 데이터 설정")]
    public ChapterData[] chapters;

    private Dictionary<string, ChapterData> _imageNameToChapter = new Dictionary<string, ChapterData>();
    private Dictionary<string, GameObject> _spawnedPrefabs = new Dictionary<string, GameObject>();
    private HashSet<string> _trackedImages = new HashSet<string>();
    private string _currentChapter = "";
    //Add
    private bool _isTracking = false;
    private bool _isTimer = false;
    private float _timer = 0;
    private float _maxTimer = 5f;
    public GameObject testCube;
    public Text prefabPos;
    public Text prefabRot;


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
            _trackedImageManager.trackablesChanged.AddListener( OnTrackedImagesChanged);
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

    /// <summary>
    /// 추적된 이미지 변경 시 호출되는 콜백 메서드
    /// </summary>
    /// <param name="eventArgs">이벤트 인자</param>
    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        // 추가된 이미지 처리
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            //Add
            if (trackedImage.referenceImage.name == "chap0" && _coverAnchor == null)
            {
                CenterPosition(trackedImage);
            }

            HandleTrackedImage(trackedImage);
        }

        // 업데이트된 이미지 처리
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            if (trackedImage.referenceImage.name != "chap0")
            {
                testCube.SetActive(false);
            }
            HandleTrackedImage(trackedImage);
        }

        // 제거된 이미지 처리
        foreach (KeyValuePair<TrackableId,ARTrackedImage> trackedImage in eventArgs.removed)
        {
            string imageName = trackedImage.Value.referenceImage.name;

            if (_trackedImages.Contains(imageName))
            {
                _trackedImages.Remove(imageName);
                Debug.Log($"이미지 '{imageName}' 추적 손실.");

                UpdateChapterState();
            }
        }
    }

    /// <summary>
    /// 개별 추적 이미지를 처리하는 메서드
    /// </summary>
    /// <param name="trackedImage">ARTrackedImage 인스턴스</param>
    private void HandleTrackedImage(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        if (!_imageNameToChapter.ContainsKey(imageName))
        {
            Debug.LogWarning($"이미지 이름 '{imageName}'이 어떤 챕터에도 매핑되어 있지 않습니다.");
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

    /// <summary>
    /// 프리팹의 포지션을 책의 중앙에 위치하기 위한 함수
    /// </summary>
    /// <param name="trackedImage"></param>
    private void CenterPosition(ARTrackedImage trackedImage)
    {
        StartCoroutine(CoverAnchorDelay(trackedImage));
    }


    /// <summary>
    /// 인식할 책의 표지를 트래킹하고 표지의 왼쪽 중앙(책을 펼쳤을 때 책의 가운데)에 앵커를 생성하는 코루틴
    /// 해당 앵커는 이후 생성될 프리팹의 포지션 역할
    /// </summary>
    /// <param name="trackedImage"></param>
    /// <returns></returns>
    private IEnumerator CoverAnchorDelay(ARTrackedImage trackedImage)
    {
        _coverAnchor = new GameObject("CoverAnchor").AddComponent<ARAnchor>();
        yield return new WaitForSeconds(0.2f);

        //Add
        Vector3 bookCenterPostion = trackedImage.transform.position - trackedImage.transform.right * (trackedImage.size.x / 2);
        _coverAnchor.transform.position = bookCenterPostion;
        _coverAnchor.transform.rotation = trackedImage.transform.rotation;
        testCube.transform.position = _coverAnchor.transform.position;
        testCube.transform.rotation = _coverAnchor.transform.rotation;
        prefabPos.text = "표지 확인";
        prefabRot.text = _coverAnchor.transform.position.ToString();

    }

    /// <summary>
    /// 현재 추적된 이미지들을 기반으로 챕터 상태를 업데이트하는 메서드
    /// </summary>
    /// <param name="trackedImage">ARTrackedImage 인스턴스 (옵션)</param>
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

                    SoundManager.instance.StopNarration();

                    Debug.Log($"PreChapter '{_currentChapter}' UnActive.");
                }

                // 새로운 챕터 활성화
                GameObject currentPrefab = _spawnedPrefabs[newCurrentChapter];

                //Add
                _isTracking = true;
                _isTimer = false;
                _timer = 0;


                if (trackedImage != null)
                {
                    //Add
                    currentPrefab.transform.localPosition = Vector3.zero;
                    currentPrefab.transform.position = _coverAnchor.transform.position;
                    currentPrefab.transform.rotation = _coverAnchor.transform.rotation;
                    
                }
                currentPrefab.SetActive(true);

                Chap4Controller.instance.ResumeAudio();

                SoundManager.instance.PlayNarration(newCurrentChapter);

                Debug.Log($"NewChpter '{newCurrentChapter}' Active.");

                _currentChapter = newCurrentChapter;

                // 일시정지 버튼 표시
                //UIManager.instance.ShowPauseButton();

                // 음성 인식 기능 활성화
                ChapterData currentChapterData = GetChapterDataByName(newCurrentChapter);
                if (currentChapterData.useSpeechRecognition)
                {
                    Debug.Log("Enabling Speech Recognition");
                    SpeechToTextManager speechManager = SpeechToTextManager.instance;
                    if (speechManager != null)
                    {
                        speechManager.StartSpeechRecognition();
                    }
                    else
                    {
                        Debug.LogWarning("SpeechToTextManager not found.");
                    }
                }
            }
            else
            {
                Debug.Log($"Keep   '{newCurrentChapter}' current chapter .");
                // 현재 챕터의 위치와 회전을 업데이트
                GameObject currentPrefab = _spawnedPrefabs[_currentChapter];
                if (trackedImage != null)
                {
                    currentPrefab.transform.position = trackedImage.transform.position;
                    currentPrefab.transform.rotation = trackedImage.transform.rotation;
                }
            }
        }
        else
        {
            // 활성 챕터가 없는 경우
            if (!string.IsNullOrEmpty(_currentChapter))
            {
                GameObject currentPrefab = _spawnedPrefabs[_currentChapter];
                currentPrefab.SetActive(false);

                SoundManager.instance.StopNarration();

                Debug.Log($"Chapter '{_currentChapter}' UnActive.");

                // 음성 인식 기능 비활성화
                ChapterData currentChapterData = GetChapterDataByName(_currentChapter);
                if (currentChapterData.useSpeechRecognition)
                {
                    SpeechToTextManager speechManager = SpeechToTextManager.instance;
                    if (speechManager != null)
                    {
                        speechManager.StopSpeechRecognition();
                    }
                }

                _currentChapter = "";

                // 일시정지 버튼 숨김
                //UIManager.instance.HidePauseButton();
            }
        }
    }

    /// <summary>
    /// 챕터 이름으로 ChapterData를 가져오는 메서드
    /// </summary>
    /// <param name="chapterName">챕터 이름</param>
    /// <returns>ChapterData</returns>
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

            if (touch.phase == TouchPhase.Began)
            {
                SoundManager.instance.ToggleNarrationPause();
                //UIManager.instance.UpdatePauseButtonUI();
            }
        }

        // 에디터에서 마우스 클릭으로 테스트하기 위한 코드
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            // UI 요소 위의 클릭은 무시
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            SoundManager.instance.ToggleNarrationPause();
            //UIManager.instance.UpdatePauseButtonUI();
        }
#endif
    }
}
