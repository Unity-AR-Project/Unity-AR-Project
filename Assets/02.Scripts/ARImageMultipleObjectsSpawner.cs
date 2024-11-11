using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

/// <summary>
/// �ν��� �̹����� �̸��� ������ �������� �����ϴ� ����ü
/// </summary>
[System.Serializable]
public struct PagePrefab
{
    public string name;          // �ν��� �̹����� �̸� (chap1, chap2, ..., chap10)
    public GameObject prefab;    // �ش� �̹����� �����ϴ� ������
    public AudioClip narration;  // �ش� �̹����� �����ϴ� �����̼� ����� Ŭ��
}

/// <summary>
/// AR �̹��� �ν� �� ���� ������Ʈ�� ���� �� �����ϴ� Ŭ����
/// �� �̹����� �´� �����̼� ������� ����ϰ�, �̹��� ���� �� ������� ��ȯ
/// ��ġ �Է��� ���� ������� �Ͻ�����/����� �� ����
/// </summary>
public class ARImageMultipleObjectsSpawner : MonoBehaviour
{
    private ARTrackedImageManager _trackedImageManager;

    [Tooltip("�� é�ͺ� �̹����� �����ϴ� ������ �� �����̼� ������� �����մϴ�.")]
    public PagePrefab[] pagePrefabs;

    // ������ �������� �̸��� Ű�� �ϴ� ��ųʸ��� ����
    private Dictionary<string, GameObject> _spawnedPrefabs = new Dictionary<string, GameObject>();

    // ���� Ȱ��ȭ�� é�� �̸��� ����
    private string _currentChapter = "";

    private void Awake()
    {
        // ARTrackedImageManager ������Ʈ ��������
        _trackedImageManager = GetComponent<ARTrackedImageManager>();

        // pagePrefabs �迭�� ����ִ��� Ȯ��
        if (pagePrefabs == null || pagePrefabs.Length == 0)
        {
            Debug.LogError("pagePrefabs �迭�� ��� �ֽ��ϴ�. Inspector���� �������� �����ϼ���.");
            return;
        }

        // prefabs �迭�� ��ȸ�ϸ� �� �������� �ʱ�ȭ
        foreach (PagePrefab pagePrefab in pagePrefabs)
        {
            // �������� �̸��� ��ȿ���� Ȯ��
            if (string.IsNullOrEmpty(pagePrefab.name))
            {
                Debug.LogWarning("PagePrefab�� name�� null �Ǵ� ��� �ֽ��ϴ�. �ش� �������� �ǳʶݴϴ�.");
                continue;
            }

            // ������ ������Ʈ�� �Ҵ�Ǿ� �ִ��� Ȯ��
            if (pagePrefab.prefab == null)
            {
                Debug.LogWarning($"PagePrefab '{pagePrefab.name}'�� prefab�� null�Դϴ�. �ش� �������� �ǳʶݴϴ�.");
                continue;
            }

            // ������ ������Ʈ�� Instantiate�Ͽ� ����
            GameObject instantiated = Instantiate(pagePrefab.prefab, Vector3.zero, Quaternion.identity);
            instantiated.name = pagePrefab.name;

            // �������� ��Ȱ��ȭ ���·� ����
            instantiated.SetActive(false);

            // ��ųʸ��� �̹� ������ Ű�� �ִ��� Ȯ���Ͽ� �ߺ��� ����
            if (!_spawnedPrefabs.ContainsKey(instantiated.name))
            {
                _spawnedPrefabs.Add(instantiated.name, instantiated);
            }
            else
            {
                Debug.LogWarning($"_spawnedPrefabs ��ųʸ��� �̹� '{instantiated.name}' Ű�� �����մϴ�. �ߺ��� ���ϱ� ���� �߰����� �ʽ��ϴ�.");
                Destroy(instantiated); // �ߺ��� ��� ������ ������ �ı�
            }
        }
    }

    private void OnEnable()
    {
        if (_trackedImageManager != null)
        {
            _trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
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
            _trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
        }
    }

    /// <summary>
    /// �̹��� ���� ���� �� ȣ��Ǵ� �ݹ� �޼���
    /// </summary>
    /// <param name="eventArgs">������ �̹��� ���� ����</param>
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // �߰��� �̹��� ó��
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }

        // ������Ʈ�� �̹��� ó��
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }

        // ���ŵ� �̹��� ó��
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            string imageName = trackedImage.referenceImage.name;

            if (string.IsNullOrEmpty(imageName))
            {
                Debug.LogWarning("Removed ARTrackedImage�� name�� null �Ǵ� ��� �ֽ��ϴ�.");
                continue;
            }

            if (_spawnedPrefabs.ContainsKey(imageName))
            {
                GameObject prefab = _spawnedPrefabs[imageName];
                prefab.SetActive(false);
                Debug.Log($"Prefab '{imageName}' ��Ȱ��ȭ");

                // �ش� �������� �����̼��� ��� ���̸� ����
                if (_currentChapter == imageName)
                {
                    AudioSource audioSource = prefab.GetComponent<AudioSource>();
                    if (audioSource != null && audioSource.isPlaying)
                    {
                        audioSource.Stop();
                        Debug.Log($"Prefab '{imageName}'�� ����� ����");
                    }
                    _currentChapter = "";
                }
            }
            else
            {
                Debug.LogWarning($"_spawnedPrefabs ��ųʸ��� '{imageName}' Ű�� �������� �ʽ��ϴ�.");
            }
        }
    }

    /// <summary>
    /// ������ �̹����� �����ϴ� �������� Ȱ��ȭ�ϰ� �����̼��� ����ϴ� �޼���
    /// </summary>
    /// <param name="trackedImage">ARTrackedImage �ν��Ͻ�</param>
    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        if (string.IsNullOrEmpty(imageName))
        {
            Debug.LogWarning("ARTrackedImage�� name�� null �Ǵ� ��� �ֽ��ϴ�.");
            return;
        }

        if (!_spawnedPrefabs.ContainsKey(imageName))
        {
            Debug.LogWarning($"_spawnedPrefabs ��ųʸ��� '{imageName}' Ű�� �������� �ʽ��ϴ�.");
            return;
        }

        GameObject prefab = _spawnedPrefabs[imageName];

        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            // �������� ��ġ�� ȸ���� ������Ʈ�ϰ� Ȱ��ȭ
            prefab.transform.position = trackedImage.transform.position;
            prefab.transform.rotation = trackedImage.transform.rotation;
            prefab.SetActive(true);
            Debug.Log($"Prefab '{imageName}' Ȱ��ȭ �� ��ġ/ȸ�� ������Ʈ");

            // ���� ��� ���� ������� �ִٸ� ����
            if (!string.IsNullOrEmpty(_currentChapter) && _currentChapter != imageName)
            {
                GameObject previousPrefab = _spawnedPrefabs[_currentChapter];
                AudioSource previousAudio = previousPrefab.GetComponent<AudioSource>();
                if (previousAudio != null && previousAudio.isPlaying)
                {
                    previousAudio.Stop();
                    Debug.Log($"���� ������ '{_currentChapter}'�� ����� ����");
                }
            }

            // �ش� �̹����� �����̼� ����� ���
            PlayNarration(imageName);
        }
        else
        {
            // ������ ��Ȱ��ȭ
            prefab.SetActive(false);
            Debug.Log($"Prefab '{imageName}' ��Ȱ��ȭ (Tracking ���� �ƴ�)");

            // �ش� �������� �����̼��� ��� ���̸� ����
            if (_currentChapter == imageName)
            {
                AudioSource audioSource = prefab.GetComponent<AudioSource>();
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                    Debug.Log($"Prefab '{imageName}'�� ����� ����");
                }
                _currentChapter = "";
            }
        }
    }

    /// <summary>
    /// �̹��� �̸��� �ش��ϴ� �����̼� ������� ����ϴ� �޼���
    /// </summary>
    /// <param name="imageName">�̹��� �̸� (chap1, chap2, ...)</param>
    private void PlayNarration(string imageName)
    {
        foreach (PagePrefab pagePrefab in pagePrefabs)
        {
            if (pagePrefab.name == imageName)
            {
                // �����̼� ������� �Ҵ�Ǿ� �ִ��� Ȯ��
                if (pagePrefab.narration != null)
                {
                    GameObject prefab = _spawnedPrefabs[imageName];
                    AudioSource audioSource = prefab.GetComponent<AudioSource>();

                    if (audioSource != null)
                    {
                        audioSource.clip = pagePrefab.narration;
                        audioSource.Play();
                        _currentChapter = imageName;
                        Debug.Log($"Prefab '{imageName}'�� �����̼� ����� ��� ����");
                    }
                    else
                    {
                        Debug.LogWarning($"Prefab '{imageName}'�� AudioSource ������Ʈ�� �����ϴ�.");
                    }
                }
                else
                {
                    Debug.LogWarning($"'{imageName}'�� �ش��ϴ� �����̼� ������� �����ϴ�.");
                }
                break;
            }
        }
    }

    private void Update()
    {
        // ��ġ �Է� ����
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // ��ġ ���� �� ó��
            if (touch.phase == TouchPhase.Began)
            {
                // UI ��Ҹ� ��ġ�� ��� ����
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return;
                }

                // ��ġ�� ��ġ�� ����ĳ��Ʈ ����
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    GameObject touchedObject = hit.transform.gameObject;

                    // ��ġ�� ������Ʈ�� ������ ���������� Ȯ��
                    foreach (KeyValuePair<string, GameObject> kvp in _spawnedPrefabs)
                    {
                        if (touchedObject == kvp.Value)
                        {
                            // ���� ��� ���� ������� �ش� �������� ������ Ȯ��
                            if (_currentChapter == kvp.Key)
                            {
                                AudioSource audioSource = kvp.Value.GetComponent<AudioSource>();
                                if (audioSource != null)
                                {
                                    if (audioSource.isPlaying)
                                    {
                                        audioSource.Pause();
                                        Debug.Log($"Prefab '{kvp.Key}'�� ����� �Ͻ�����");
                                    }
                                    else
                                    {
                                        // �ٸ� ������� ��� ���̸� ����
                                        if (!string.IsNullOrEmpty(_currentChapter) && _currentChapter != kvp.Key)
                                        {
                                            GameObject previousPrefab = _spawnedPrefabs[_currentChapter];
                                            AudioSource previousAudio = previousPrefab.GetComponent<AudioSource>();
                                            if (previousAudio != null && previousAudio.isPlaying)
                                            {
                                                previousAudio.Stop();
                                                Debug.Log($"���� ������ '{_currentChapter}'�� ����� ����");
                                            }
                                        }

                                        audioSource.Play();
                                        Debug.Log($"Prefab '{kvp.Key}'�� ����� ���");
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}
