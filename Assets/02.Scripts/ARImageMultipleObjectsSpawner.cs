using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.XR.ARCore;

/// <summary>
/// �ν��� �̹����� �̸��� ������ ������, �����̼� ������� �����ϴ� ����ü
/// </summary>
[System.Serializable]
public struct PagePrefab
{
    public string name;          // �ν��� �̹����� �̸� (��: chap1, chap2, ...)
    public GameObject prefab;    // �ش� �̹����� �����ϴ� ������ ������Ʈ
    public AudioClip narration;  // �ش� �̹����� �����ϴ� �����̼� ����� Ŭ��
}

/// <summary>
/// AR �̹��� �ν� �� ���� ������Ʈ�� ���� �� �����ϴ� Ŭ����
/// �� �̹����� �´� �����̼� ������� ����ϰ�, �̹��� ���� �� ������� ��ȯ
/// ��ġ �Է��� ���� ������� �Ͻ�����/����� �� ����
/// </summary>
public class ARImageMultipleObjectsSpawner : MonoBehaviour
{
    private ARTrackedImageManager _trackedImageManager; // ARTrackedImageManager ������Ʈ�� ����
    public ARSession arSession;

    [Tooltip("�� é�� �̹����� �����ϴ� �����հ� �����̼� ������� �����մϴ�.")]
    public PagePrefab[] pagePrefabs; // �� é�� �̹����� ���� �����հ� ����� ���� �迭

    private Dictionary<string, GameObject> _spawnedPrefabs = new Dictionary<string, GameObject>(); // ������ �������� �����ϴ� ��ųʸ�
    private string _currentChapter = ""; // ���� Ȱ��ȭ�� é�� �̸��� ����


    /// <summary>
    /// ��ũ��Ʈ�� Ȱ��ȭ�Ǳ� ���� ȣ��Ǵ� �޼���
    /// �ʱ� ������ ���
    /// </summary>
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

        // pagePrefabs �迭�� ��ȸ�ϸ� �� �������� �ʱ�ȭ
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
            instantiated.name = pagePrefab.name; // �������� �̸��� ����
            instantiated.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ ���·� ����

            // ��ųʸ��� �������� �߰�, �ߺ� Ű ����
            if (!_spawnedPrefabs.ContainsKey(instantiated.name))
            {
                _spawnedPrefabs.Add(instantiated.name, instantiated);
            }
            else
            {
                Debug.LogWarning($"_spawnedPrefabs ��ųʸ��� '{instantiated.name}' Ű�� �̹� �����մϴ�. �ߺ��� ���ϱ� ���� �߰����� �ʽ��ϴ�.");
                Destroy(instantiated); // �ߺ��� ��� ������ ������ �ı�
            }
        }
    }

    /// <summary>
    /// ��ũ��Ʈ�� Ȱ��ȭ�� �� ȣ��Ǵ� �޼���
    /// ARTrackedImageManager�� ���� �̹��� ���� �̺�Ʈ�� ������ ���
    /// </summary>
    private void OnEnable()
    {
        if (_trackedImageManager != null)
        {
            RestartARSession();

            // ���� �̹��� ���� �� OnTrackedImagesChanged �޼��� ȣ��
            _trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
        }
        else
        {
            Debug.LogError("ARTrackedImageManager�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    /// <summary>
    /// ��ũ��Ʈ�� ��Ȱ��ȭ�� �� ȣ��Ǵ� �޼���
    /// ARTrackedImageManager�� ���� �̹��� ���� �̺�Ʈ���� ������ ����
    /// </summary>
    private void OnDisable()
    {
        if (_trackedImageManager != null)
        {
            _trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
        }
    }

    /// <summary>
    /// ������ �̹��� ���� �� ȣ��Ǵ� �ݹ� �޼���
    /// </summary>
    /// <param name="eventArgs">������ �̹��� ���� ������ �����ϴ� �̺�Ʈ ����</param>
    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        // �߰��� �̹����� ó��
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage); // ������ ��ġ/ȸ�� ������Ʈ �� Ȱ��ȭ
        }

        // ������Ʈ�� �̹����� ó��
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage); // ������ ��ġ/ȸ�� ������Ʈ
        }

        // ���ŵ� �̹����� ó��
        foreach (KeyValuePair<TrackableId, ARTrackedImage> trackedImage in eventArgs.removed)
        {
            string imageName = trackedImage.Value.referenceImage.name;

            // �̹��� �̸��� ��ȿ���� Ȯ��
            if (string.IsNullOrEmpty(imageName))
            {
                Debug.LogWarning("���ŵ� ARTrackedImage�� name�� null �Ǵ� ��� �ֽ��ϴ�.");
                continue;
            }

            // ��ųʸ��� �ش� �̸��� �������� �ִ��� Ȯ��
            if (_spawnedPrefabs.ContainsKey(imageName))
            {
                GameObject prefab = _spawnedPrefabs[imageName];
                prefab.SetActive(false); // ������ ��Ȱ��ȭ
                Debug.Log($"Prefab '{imageName}' ��Ȱ��ȭ");

                // ���� Ȱ��ȭ�� é�Ͱ� ���ŵ� �̹������ ����� ����
                if (_currentChapter == imageName)
                {
                    AudioSource audioSource = prefab.GetComponent<AudioSource>();
                    if (audioSource != null && audioSource.isPlaying)
                    {
                        audioSource.Stop(); // ����� ����
                        Debug.Log($"Prefab '{imageName}'�� ����� ����");
                    }
                    _currentChapter = ""; // ���� é�� �ʱ�ȭ
                }
            }
            else
            {
                Debug.LogWarning($"_spawnedPrefabs ��ųʸ��� '{imageName}' Ű�� �������� �ʽ��ϴ�.");
            }
        }
    }

    /// <summary>
    /// ������ �̹����� ������Ʈ�ϰ� �ʿ��� ��� �����̼��� ����մϴ�.
    /// </summary>
    /// <param name="trackedImage">ARTrackedImage �ν��Ͻ�</param>
    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name; // ������ �̹����� �̸� ��������

        // �̹��� �̸��� ��ȿ���� Ȯ��
        if (string.IsNullOrEmpty(imageName))
        {
            Debug.LogWarning("ARTrackedImage�� name�� null �Ǵ� ��� �ֽ��ϴ�.");
            return;
        }

        // ��ųʸ��� �ش� �̸��� �������� �ִ��� Ȯ��
        if (!_spawnedPrefabs.ContainsKey(imageName))
        {
            Debug.LogWarning($"_spawnedPrefabs ��ųʸ��� '{imageName}' Ű�� �������� �ʽ��ϴ�.");
            return;
        }

        GameObject prefab = _spawnedPrefabs[imageName]; // �ش� ������ ��������

        // �̹����� ���� ���� ���
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            //��鸲 ������ ���̱� ���� ������ ��ġ�� ������ �̹��� ��ġ�� ��� �̵���Ű�� ��� Lerp �Ǵ� SmoothDamp �Լ��� ����Ͽ� �ε巴�� �̵�
            //prefab.transform.position = Vector3.SmoothDamp(prefab.transform.position, trackedImage.transform.position, ref velocity, smoothTime);
            //prefab.transform.rotation = Quaternion.Slerp(prefab.transform.rotation, trackedImage.transform.rotation, Time.deltaTime * 5f);

            // �������� ��ġ�� ȸ���� ������Ʈ
            prefab.transform.position = trackedImage.transform.position;
            prefab.transform.rotation = trackedImage.transform.rotation;
            prefab.SetActive(true); // ������ Ȱ��ȭ
            Debug.Log($"Prefab '{imageName}' Ȱ��ȭ �� ��ġ/ȸ�� ������Ʈ");

            // ���� é�Ͱ� ����� ��� ���� é���� ����� ���� �� ���ο� é���� �����̼� ���
            if (_currentChapter != imageName)
            {
                // ���� é�Ͱ� �ִٸ� ����� ����
                if (!string.IsNullOrEmpty(_currentChapter))
                {
                    GameObject previousPrefab = _spawnedPrefabs[_currentChapter];
                    AudioSource previousAudio = previousPrefab.GetComponent<AudioSource>();
                    if (previousAudio != null && previousAudio.isPlaying)
                    {
                        previousAudio.Stop();
                        Debug.Log($"���� ������ '{_currentChapter}'�� ����� ����");
                    }
                }

                // ���ο� é���� �����̼� ���
                PlayNarration(imageName);
            }
        }
        else
        {
            // �̹��� ������ ���� �ʴ� ��� ������ ��Ȱ��ȭ
            prefab.SetActive(false);
            Debug.Log($"Prefab '{imageName}' ��Ȱ��ȭ (���� �� �ƴ�)");

            // ���� é�Ͱ� ���ŵ� �̹������ ����� ����
            if (_currentChapter == imageName)
            {
                AudioSource audioSource = prefab.GetComponent<AudioSource>();
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                    Debug.Log($"Prefab '{imageName}'�� ����� ����");
                }
                _currentChapter = ""; // ���� é�� �ʱ�ȭ
            }
        }
    }

    /// <summary>
    /// ������ �̹��� �̸��� �ش��ϴ� �����̼��� ����մϴ�.
    /// </summary>
    /// <param name="imageName">�̹��� �̸�</param>
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
                        audioSource.clip = pagePrefab.narration; // ����� Ŭ�� ����
                        audioSource.Play(); // ����� ���
                        _currentChapter = imageName; // ���� é�� ������Ʈ
                        Debug.Log($"Prefab '{imageName}'�� �����̼� ��� ����");
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
                break; // �ش� é�͸� ã�����Ƿ� ���� ����
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

    /// <summary>
    /// �� �����Ӹ��� ȣ��Ǵ� �޼���
    /// ��ġ �Է��� �����Ͽ� ����� �Ͻ�����/��� ����� ����
    /// </summary>
    private void Update()
    {
        // ȭ�鿡 ��ġ�� �ϳ� �̻� �ִ��� Ȯ��
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // ù ��° ��ġ ���� ��������

            // ��ġ�� ���۵� ������ ��� ó��
            if (touch.phase == TouchPhase.Began)
            {
                // ��ġ�� ��ġ�� UI ��� ���� ��� ����
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return;
                }

                // ��ġ�� ��ġ�� ����ĳ��Ʈ ����
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    GameObject touchedObject = hit.transform.gameObject; // ��ġ�� ������Ʈ ��������

                    // ������ ��� �������� ��ȸ�ϸ� ��ġ�� ������Ʈ�� ��ġ�ϴ��� Ȯ��
                    foreach (KeyValuePair<string, GameObject> kvp in _spawnedPrefabs)
                    {
                        if (touchedObject == kvp.Value)
                        {
                            // ���� é�Ͱ� ��ġ�� �����հ� ��ġ�ϴ��� Ȯ��
                            if (_currentChapter == kvp.Key)
                            {
                                AudioSource audioSource = kvp.Value.GetComponent<AudioSource>();
                                if (audioSource != null)
                                {
                                    if (audioSource.isPlaying)
                                    {
                                        audioSource.Pause(); // ����� �Ͻ�����
                                        Debug.Log($"Prefab '{kvp.Key}'�� ����� �Ͻ�����");
                                    }
                                    else
                                    {
                                        audioSource.Play(); // ����� ���
                                        Debug.Log($"Prefab '{kvp.Key}'�� ����� ���");
                                    }
                                }
                            }
                            break; // ��ġ�ϴ� �������� ã�����Ƿ� ���� ����
                        }
                    }
                }
            }
        }
    }
}
