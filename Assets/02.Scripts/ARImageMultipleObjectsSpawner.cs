using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// é�� ������ ����ü: é�� �̸�, ������, �̹��� �̸����� ����
/// </summary>
[System.Serializable]
public struct ChapterData
{
    public string chapterName;          // é�� �̸� (��: "chap1")
    public GameObject prefab;           // �ش� é�Ϳ� �����ϴ� ������ ������Ʈ
    public string[] imageNames;         // �� é�Ϳ� �����ϴ� �̹��� �̸� �迭
    public bool useSpeechRecognition;   // ���� �ν� ��� ��� ����
}

/// <summary>
/// ���� �̹����� �ν��Ͽ� �����Ǵ� é���� ������Ʈ�� �����ϰ� �����̼��� ����ϴ� Ŭ����
/// </summary>
[RequireComponent(typeof(ARTrackedImageManager))]
public class ARImageMultipleObjectsSpawner : MonoBehaviour
{

    private ARTrackedImageManager _trackedImageManager;
    //Add
    //public ARSession arSession;
    private ARAnchor _coverAnchor;

    [Tooltip("�� é�Ϳ� �����ϴ� ������ ����")]
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
            _trackedImageManager.trackablesChanged.AddListener( OnTrackedImagesChanged);
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

    /// <summary>
    /// ������ �̹��� ���� �� ȣ��Ǵ� �ݹ� �޼���
    /// </summary>
    /// <param name="eventArgs">�̺�Ʈ ����</param>
    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        // �߰��� �̹��� ó��
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            //Add
            if (trackedImage.referenceImage.name == "chap0" && _coverAnchor == null)
            {
                CenterPosition(trackedImage);
            }

            HandleTrackedImage(trackedImage);
        }

        // ������Ʈ�� �̹��� ó��
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            if (trackedImage.referenceImage.name != "chap0")
            {
                testCube.SetActive(false);
            }
            HandleTrackedImage(trackedImage);
        }

        // ���ŵ� �̹��� ó��
        foreach (KeyValuePair<TrackableId,ARTrackedImage> trackedImage in eventArgs.removed)
        {
            string imageName = trackedImage.Value.referenceImage.name;

            if (_trackedImages.Contains(imageName))
            {
                _trackedImages.Remove(imageName);
                Debug.Log($"�̹��� '{imageName}' ���� �ս�.");

                UpdateChapterState();
            }
        }
    }

    /// <summary>
    /// ���� ���� �̹����� ó���ϴ� �޼���
    /// </summary>
    /// <param name="trackedImage">ARTrackedImage �ν��Ͻ�</param>
    private void HandleTrackedImage(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        if (!_imageNameToChapter.ContainsKey(imageName))
        {
            Debug.LogWarning($"�̹��� �̸� '{imageName}'�� � é�Ϳ��� ���εǾ� ���� �ʽ��ϴ�.");
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

    /// <summary>
    /// �������� �������� å�� �߾ӿ� ��ġ�ϱ� ���� �Լ�
    /// </summary>
    /// <param name="trackedImage"></param>
    private void CenterPosition(ARTrackedImage trackedImage)
    {
        StartCoroutine(CoverAnchorDelay(trackedImage));
    }


    /// <summary>
    /// �ν��� å�� ǥ���� Ʈ��ŷ�ϰ� ǥ���� ���� �߾�(å�� ������ �� å�� ���)�� ��Ŀ�� �����ϴ� �ڷ�ƾ
    /// �ش� ��Ŀ�� ���� ������ �������� ������ ����
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
        prefabPos.text = "ǥ�� Ȯ��";
        prefabRot.text = _coverAnchor.transform.position.ToString();

    }

    /// <summary>
    /// ���� ������ �̹������� ������� é�� ���¸� ������Ʈ�ϴ� �޼���
    /// </summary>
    /// <param name="trackedImage">ARTrackedImage �ν��Ͻ� (�ɼ�)</param>
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

                    SoundManager.instance.StopNarration();

                    Debug.Log($"PreChapter '{_currentChapter}' UnActive.");
                }

                // ���ο� é�� Ȱ��ȭ
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

                // �Ͻ����� ��ư ǥ��
                //UIManager.instance.ShowPauseButton();

                // ���� �ν� ��� Ȱ��ȭ
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
                // ���� é���� ��ġ�� ȸ���� ������Ʈ
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
            // Ȱ�� é�Ͱ� ���� ���
            if (!string.IsNullOrEmpty(_currentChapter))
            {
                GameObject currentPrefab = _spawnedPrefabs[_currentChapter];
                currentPrefab.SetActive(false);

                SoundManager.instance.StopNarration();

                Debug.Log($"Chapter '{_currentChapter}' UnActive.");

                // ���� �ν� ��� ��Ȱ��ȭ
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

                // �Ͻ����� ��ư ����
                //UIManager.instance.HidePauseButton();
            }
        }
    }

    /// <summary>
    /// é�� �̸����� ChapterData�� �������� �޼���
    /// </summary>
    /// <param name="chapterName">é�� �̸�</param>
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



        // ��ġ �Է� �����Ͽ� �����̼� �Ͻ�����/�簳
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // UI ��� ���� ��ġ�� ����
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            if (touch.phase == TouchPhase.Began)
            {
                SoundManager.instance.ToggleNarrationPause();
                //UIManager.instance.UpdatePauseButtonUI();
            }
        }

        // �����Ϳ��� ���콺 Ŭ������ �׽�Ʈ�ϱ� ���� �ڵ�
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            // UI ��� ���� Ŭ���� ����
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            SoundManager.instance.ToggleNarrationPause();
            //UIManager.instance.UpdatePauseButtonUI();
        }
#endif
    }
}
