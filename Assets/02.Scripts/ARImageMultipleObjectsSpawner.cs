using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using System.Collections;

[System.Serializable]
public struct ChapterData
{
    public string chapterName;          // é�� �̸� (��: "chap1")
    public GameObject prefab;           // �ش� é�Ϳ� �����ϴ� ������ ������Ʈ
    public string[] imageNames;         // �� é�Ϳ� �����ϴ� �̹��� �̸� �迭
    public bool useSpeechRecognition;   // ���� �ν� ��� ��� ����
}

[RequireComponent(typeof(ARTrackedImageManager))]
public class ARImageMultipleObjectsSpawner : MonoBehaviour
{
    private ARTrackedImageManager _trackedImageManager;
    private GameObject _coverAnchor; // ARAnchor ��� GameObject�� ����

    [Tooltip("�� é�Ϳ� �����ϴ� ������ ����")]
    public ChapterData[] chapters;

    private Dictionary<string, ChapterData> _imageNameToChapter = new Dictionary<string, ChapterData>();
    private Dictionary<string, GameObject> _spawnedPrefabs = new Dictionary<string, GameObject>();
    private HashSet<string> _trackedImages = new HashSet<string>();
    private string _currentChapter = "";
    private IChapterController _currentChapterController = null; // �߰�: ���� é�� ��Ʈ�ѷ�

    //Tracking����
    private TrackingState _previousTrackingState = TrackingState.None;
    private float _trackingStateChangeTime = 0f;
    private float _trackingStateStableDuration = 0.5f; // ���°� ���������� �����Ǿ�� �ϴ� �ð�
    private bool _isTrackingMessageVisible = false;

    //Add
    private bool _isTracking = false;
    private bool _isTimer = false;
    private float _timer = 0;
    private float _maxTimer = 5f;
    public GameObject testCube;

    private bool _isCoverAnchorSet = false; // Ŀ�� ��Ŀ�� �Ϸ�Ǿ����� ����

    void Start()
    {
        // �ʿ��� �ʱ�ȭ �ڵ�
    }

    private void Awake()
    {
        // ARTrackedImageManager ������Ʈ ��������
        _trackedImageManager = GetComponent<ARTrackedImageManager>();

        // chapters �迭�� ����ִ��� Ȯ��
        if (chapters == null || chapters.Length == 0)
        {
            Debug.LogError("Chapters �迭�� ��� �ֽ��ϴ�. Inspector���� é�� �����͸� �����ϼ���.");
            return;
        }

        // �̹��� �̸��� é�� �����Ϳ� �����ϰ� �������� �̸� �����Ͽ� ��Ȱ��ȭ
        foreach (ChapterData chapter in chapters)
        {
            if (string.IsNullOrEmpty(chapter.chapterName))
            {
                Debug.LogWarning("é�� �̸��� null �Ǵ� ��� �ֽ��ϴ�. �ش� é�͸� �ǳʶݴϴ�.");
                continue;
            }

            if (chapter.prefab == null)
            {
                Debug.LogWarning($"é�� '{chapter.chapterName}'�� �������� null�Դϴ�. �ش� é�͸� �ǳʶݴϴ�.");
                continue;
            }

            if (chapter.imageNames == null || chapter.imageNames.Length == 0)
            {
                Debug.LogWarning($"é�� '{chapter.chapterName}'�� �̹��� �̸� �迭�� ��� �ֽ��ϴ�. �ش� é�͸� �ǳʶݴϴ�.");
                continue;
            }

            // ������ ���� �� ��Ȱ��ȭ
            GameObject instantiated = Instantiate(chapter.prefab, Vector3.zero, Quaternion.identity);
            instantiated.name = chapter.chapterName;
            instantiated.SetActive(false);

            // ������ ������ ��ųʸ��� �߰�
            if (!_spawnedPrefabs.ContainsKey(chapter.chapterName))
            {
                _spawnedPrefabs.Add(chapter.chapterName, instantiated);
            }
            else
            {
                Debug.LogWarning($"_spawnedPrefabs ��ųʸ��� '{chapter.chapterName}' Ű�� �̹� �����մϴ�.");
                Destroy(instantiated);
            }

            // �̹��� �̸��� é�� �����Ϳ� ����
            foreach (string imageName in chapter.imageNames)
            {
                if (string.IsNullOrEmpty(imageName))
                {
                    Debug.LogWarning($"é�� '{chapter.chapterName}'�� �̹��� �̸��� null �Ǵ� ��� �ֽ��ϴ�. �ش� �̹����� �ǳʶݴϴ�.");
                    continue;
                }

                if (!_imageNameToChapter.ContainsKey(imageName))
                {
                    _imageNameToChapter.Add(imageName, chapter);
                }
                else
                {
                    Debug.LogWarning($"�̹��� �̸� '{imageName}'�� �̹� �ٸ� é�Ϳ� ���εǾ� �ֽ��ϴ�.");
                }
            }
        }
    }

    private void OnEnable()
    {
        if (_trackedImageManager != null)
        {
            // �̺�Ʈ ����
            _trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
        }
        else
        {
            Debug.LogError("ARTrackedImageManager�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    private void OnDisable()
    {
        if (_trackedImageManager != null)
        {
            // �̺�Ʈ ����
            _trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
        }
    }

    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        // �߰��� �̹��� ó��
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            if (trackedImage.referenceImage.name == "chap0" && !_isCoverAnchorSet)
            {
                CenterPosition(trackedImage);
            }

            // Ŀ�� ��Ŀ�� �������� �ʾ��� ��� �޽��� ǥ��
            if (!_isCoverAnchorSet)
            {
                UIManager.instance.ShowMessage("Ŀ�� �̹����� �ν��ϵ��� ������ �ּ���.");
            }

            LogTrackingState(trackedImage);
            HandleTrackedImage(trackedImage);
        }

        // ������Ʈ�� �̹��� ó��
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            LogTrackingState(trackedImage);
            HandleTrackedImage(trackedImage);
        }

        // ���ŵ� �̹��� ó��
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
    /// TrackingState �̺�Ʈ �޽���
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


        // ���°� ����Ǿ��� ���� ó��
        if (_previousTrackingState != currentTrackingState)
        {
            _previousTrackingState = currentTrackingState;
            _trackingStateChangeTime = Time.time; // ���� ���� �ð� ���
            _isTrackingMessageVisible = false;    // �޽��� ǥ�� ���� �ʱ�ȭ
        }

        // ���°� ���������� �����Ǿ��� �� �޽��� ǥ��
        if (!_isTrackingMessageVisible && Time.time - _trackingStateChangeTime >= _trackingStateStableDuration)
        {
            string message = "";

            if (currentTrackingState == TrackingState.Tracking)
            {
                message = "���� �������� �̹��� ���� ��ġ�ϰ� ��ȣ�ۿ��� �����մϴ�.";
            }
            else if (currentTrackingState == TrackingState.Limited)
            {
                message = "ī�޶�� �̹����� �Ÿ��� ���̰ų� ������ ������ �ּ���.";
            }
            else if (currentTrackingState == TrackingState.None)
            {
                message = "�̹��� ������ �����Ϸ��� ī�޶� �̹����� �����ּ���.";
            }

            // �޽����� ������� ������ ǥ��
            if (!string.IsNullOrEmpty(message))
            {
                UIManager.instance.TreackingStateMessage(message);
                _isTrackingMessageVisible = true;

                // ���� �ð� �Ŀ� �޽��� ����
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
            Debug.LogWarning($"�̹��� �̸� '{imageName}'�� � é�Ϳ��� ���εǾ� ���� �ʽ��ϴ�.");
            return;
        }

        // Ŀ�� ��Ŀ�� �Ϸ���� �ʾ��� ��� ó��
        if (!_isCoverAnchorSet)
        {
            // UIManager�� ���� �ȳ� �޽��� ǥ��
            UIManager.instance.ShowMessage("Ŀ�� �̹����� �ν��Ͽ� ��Ŀ�� ������ �ּ���.");
            return;
        }

        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            if (!_trackedImages.Contains(imageName))
            {
                _trackedImages.Add(imageName);
                Debug.Log($"�̹��� '{imageName}' ���� ����.");
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
                Debug.Log($"�̹��� '{imageName}' ���� �ս�.");

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
        _coverAnchor = new GameObject("CoverAnchor"); // ARAnchor ����
        yield return new WaitForSeconds(0.2f);

        Vector3 bookCenterPosition = trackedImage.transform.position - trackedImage.transform.right * (trackedImage.size.x / 2);
        _coverAnchor.transform.position = bookCenterPosition;
        _coverAnchor.transform.rotation = trackedImage.transform.rotation;

        // ��Ŀ�� �����Ǿ����� ǥ��
        _isCoverAnchorSet = true;

        UIManager.instance.ShowMessage("Ŀ�� �̹����� \n �ν��Ͽ����ϴ�.");

        // UIManager�� ���� �޽��� ����
       // UIManager.instance.HideMessage();
    }

    private void UpdateChapterState(ARTrackedImage trackedImage = null)
    {
        // ������ �̹����� ����Ͽ� Ȱ�� é�� ����
        HashSet<string> activeChapters = new HashSet<string>();

        foreach (string imageName in _trackedImages)
        {
            ChapterData chapter = _imageNameToChapter[imageName];
            activeChapters.Add(chapter.chapterName);
        }

        if (activeChapters.Count > 0)
        {
            string newCurrentChapter = "";

            // ù ��° Ȱ�� é�� ����
            foreach (string chapterName in activeChapters)
            {
                newCurrentChapter = chapterName;
                break;
            }

            if (_currentChapter != newCurrentChapter)
            {
                // ���� é�� ��Ȱ��ȭ
                if (!string.IsNullOrEmpty(_currentChapter))
                {
                    GameObject previousPrefab = _spawnedPrefabs[_currentChapter];
                    previousPrefab.SetActive(false);

                    //Add
                    _isTracking = false;
                    _isTimer = false;
                    _timer = 0;

                    // ���� é�� ��Ʈ�ѷ� �ʱ�ȭ
                    _currentChapterController = null;

                    Debug.Log($"PreChapter '{_currentChapter}' UnActive.");
                }

                // ���ο� é�� Ȱ��ȭ
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

                // ���� é�� ��Ʈ�ѷ� ��������
                _currentChapterController = currentPrefab.GetComponent<IChapterController>();
                if (_currentChapterController == null)
                {
                    Debug.LogWarning($"Chapter controller not found on prefab '{newCurrentChapter}'.");
                }

                // ���� �ν� ��� Ȱ��ȭ
                ChapterData currentChapterData = GetChapterDataByName(newCurrentChapter);
                if (currentChapterData.useSpeechRecognition)
                {
                    Debug.Log("Enabling Speech Recognition");
                    // ���� �ν� ���� �ڵ� �߰�
                }
            }
            else
            {
                Debug.Log($"Keep '{newCurrentChapter}' current chapter.");
                // ���� é���� ��ġ�� ȸ���� ������Ʈ
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
            // Ȱ�� é�Ͱ� ���� ���
            if (!string.IsNullOrEmpty(_currentChapter) && !_isTracking)
            {
                GameObject currentPrefab = _spawnedPrefabs[_currentChapter];
                currentPrefab.SetActive(false);

                Debug.Log($"Chapter '{_currentChapter}' UnActive.");

                // ���� �ν� ��� ��Ȱ��ȭ
                ChapterData currentChapterData = GetChapterDataByName(_currentChapter);
                if (currentChapterData.useSpeechRecognition)
                {
                    // ���� �ν� ���� �ڵ� �߰�
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

        // ��ġ �Է� �����Ͽ� �����̼� �Ͻ�����/�簳
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // UI ��� ���� ��ġ�� ����
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            // ���� é�Ϳ��� ���� �ν� ����� ��� ���̸� ��ġ �Է��� ����
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

        // �����Ϳ��� ���콺 Ŭ������ �׽�Ʈ�ϱ� ���� �ڵ�
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            // UI ��� ���� Ŭ���� ����
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            // ���� é�Ϳ��� ���� �ν� ����� ��� ���̸� ���콺 �Է��� ����
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
