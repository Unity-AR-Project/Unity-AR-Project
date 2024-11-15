using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UIElements;
    
/// <summary>
/// é�� ������ ����ü: é�� �̸�, ������, �̹��� �̸����� ����
/// </summary>
[System.Serializable]
public struct ChapterData
{
    public string chapterName;          // é�� �̸� (��: "chap1")
    public GameObject prefab;           // �ش� é�Ϳ� �����ϴ� ������ ������Ʈ
    public string[] imageNames;         // �� é�Ϳ� �����ϴ� �̹��� �̸� �迭
}

/// <summary>
/// ���� �̹����� �ν��Ͽ� �����Ǵ� é���� ������Ʈ�� �����ϰ� �����̼��� ����ϴ� Ŭ����
/// </summary>
[RequireComponent(typeof(ARTrackedImageManager))]
public class ARImageMultipleObjectsSpawner : MonoBehaviour
{
    private ARTrackedImageManager _trackedImageManager;
    public ARSession arSession;
    private ARAnchor coverAnchor;

    [Tooltip("�� é�Ϳ� �����ϴ� ������ ����")]
    public ChapterData[] chapters;

    private Dictionary<string, ChapterData> _imageNameToChapter = new Dictionary<string, ChapterData>();
    private Dictionary<string, GameObject> _spawnedPrefabs = new Dictionary<string, GameObject>();
    private HashSet<string> _trackedImages = new HashSet<string>();
    private string _currentChapter = "";

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

        // ��ųʸ� ����
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
            // ���� �̹��� ���� �� OnTrackedImagesChanged �޼��� ȣ��
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
            _trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
        }
    }

    public GameObject testCube;
    /// <summary>
    /// ������ �̹��� ���� �� ȣ��Ǵ� �ݹ� �޼���
    /// </summary>
    /// <param name="eventArgs">�̺�Ʈ ����</param>
    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        // �߰��� �̹��� ó��
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            if (trackedImage.referenceImage.name == "chap0" && coverAnchor == null)
            {
                StartCoroutine(CoverAnchorDelay(trackedImage));
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
        foreach (KeyValuePair<TrackableId, ARTrackedImage> trackedImage in eventArgs.removed)
        {
            string imageName = trackedImage.Value.referenceImage.name;

            if (_trackedImages.Contains(imageName))
            {
                _trackedImages.Remove(imageName);
                Debug.Log($"�̹��� '{imageName}' ���� �ս�.");

                UpdateChapterState(trackedImage.Value);
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
                _trackedImages.Remove(imageName);
                Debug.Log($"�̹��� '{imageName}' ���� �ս�.");

                UpdateChapterState(trackedImage);
            }
        }
    }

    private IEnumerator CoverAnchorDelay(ARTrackedImage trackedImage)
    {
        coverAnchor = new GameObject("CoverAnchor").AddComponent<ARAnchor>();
        yield return new WaitForSeconds(0.1f);

        Vector3 leftEdgePosition = trackedImage.transform.position - trackedImage.transform.right * (trackedImage.size.x / 2);
        coverAnchor.transform.position = leftEdgePosition;
        coverAnchor.transform.rotation = trackedImage.transform.rotation;
        testCube.transform.position = coverAnchor.transform.position;
        testCube.transform.rotation = coverAnchor.transform.rotation;
        prefabPos.text = "ǥ�� Ȯ��";
        prefabRot.text = coverAnchor.transform.position.ToString();
    }

    public Text prefabPos;
    public Text prefabRot;
    /// <summary>
    /// ���� ������ �̹������� ������� é�� ���¸� ������Ʈ�ϴ� �޼���
    /// </summary>
    private void UpdateChapterState(ARTrackedImage trackedImage)
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

                    SoundManager.instance.StopNarration();
                    

                    Debug.Log($"é�� '{_currentChapter}' ��Ȱ��ȭ.");
                }

                // ���ο� é�� Ȱ��ȭ
                GameObject currentPrefab = _spawnedPrefabs[newCurrentChapter];

                //������ ��ġ �� ȸ��
                currentPrefab.transform.position = coverAnchor.transform.position;
                currentPrefab.transform.rotation = coverAnchor.transform.rotation;
                currentPrefab.SetActive(true);
                prefabPos.text = "Anchor Position: " + coverAnchor.transform.position.ToString();
                prefabRot.text = "Anchor Rotation: " + coverAnchor.transform.rotation.ToString();
                //currentPrefab.SetActive(true);

                SoundManager.instance.PlayNarration(newCurrentChapter);
                SoundManager.instance.PlaySFX("door");
                Debug.Log($"é�� '{newCurrentChapter}' Ȱ��ȭ.");

                _currentChapter = newCurrentChapter;

                // �Ͻ����� ��ư ǥ��
                UIManager.instance.ShowPauseButton();
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

                Debug.Log($"é�� '{_currentChapter}' ��Ȱ��ȭ.");

                _currentChapter = "";

                // �Ͻ����� ��ư ����
                UIManager.instance.HidePauseButton();
            }
        }
    }

    // ���� �̹����� ������� Ʈ��ŷ ���°� �����Ǿ� �������� �Ȼ������ �κ��� �ذ��ϱ� ���� �Լ��� �ڷ�ƾ
    private void RestartARSession()
    {
        StartCoroutine(RestartSessionCoroutine());
    }

    /// <summary>
    /// AR Session�� ���� Ű�� �ڷ�ƾ
    /// </summary>
    /// <returns></returns>
    private IEnumerator RestartSessionCoroutine()
    {
        if (arSession != null)
        {
            arSession.enabled = false;  // AR ���� ��Ȱ��ȭ
            yield return null;          // �� ������ ���
            arSession.enabled = true;   // AR ���� �ٽ� Ȱ��ȭ
            Debug.Log("AR ������ ����۵Ǿ����ϴ�.");
        }
        else
        {
            Debug.LogWarning("ARSession ������Ʈ�� �������� �ʾҽ��ϴ�.");
        }
    }

    private void Update()
    {
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
                UIManager.instance.UpdatePauseButtonUI();
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
            // �׽�Ʈ�� ���� sfx ���
            SoundManager.instance.PlaySFX("door");

            UIManager.instance.UpdatePauseButtonUI();
        }
#endif
    }
}
