using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

/// <summary>
/// 인식할 이미지의 이름과 생성할 프리팹을 정의하는 구조체
/// </summary>
[System.Serializable]
public struct PagePrefab
{
    public string name;          // 인식할 이미지의 이름 (chap1, chap2, ..., chap10)
    public GameObject prefab;    // 해당 이미지에 대응하는 프리팹
    public AudioClip narration;  // 해당 이미지에 대응하는 나레이션 오디오 클립
}

/// <summary>
/// AR 이미지 인식 시 여러 오브젝트를 스폰 및 관리하는 클래스
/// 각 이미지에 맞는 나레이션 오디오를 재생하고, 이미지 변경 시 오디오를 전환
/// 터치 입력을 통해 오디오를 일시정지/재생할 수 있음
/// </summary>
public class ARImageMultipleObjectsSpawner : MonoBehaviour
{
    private ARTrackedImageManager _trackedImageManager;

    [Tooltip("각 챕터별 이미지와 대응하는 프리팹 및 나레이션 오디오를 설정합니다.")]
    public PagePrefab[] pagePrefabs;

    // 스폰된 프리팹을 이름을 키로 하는 딕셔너리로 관리
    private Dictionary<string, GameObject> _spawnedPrefabs = new Dictionary<string, GameObject>();

    // 현재 활성화된 챕터 이름을 추적
    private string _currentChapter = "";

    private void Awake()
    {
        // ARTrackedImageManager 컴포넌트 가져오기
        _trackedImageManager = GetComponent<ARTrackedImageManager>();

        // pagePrefabs 배열이 비어있는지 확인
        if (pagePrefabs == null || pagePrefabs.Length == 0)
        {
            Debug.LogError("pagePrefabs 배열이 비어 있습니다. Inspector에서 프리팹을 설정하세요.");
            return;
        }

        // prefabs 배열을 순회하며 각 프리팹을 초기화
        foreach (PagePrefab pagePrefab in pagePrefabs)
        {
            // 프리팹의 이름이 유효한지 확인
            if (string.IsNullOrEmpty(pagePrefab.name))
            {
                Debug.LogWarning("PagePrefab의 name이 null 또는 비어 있습니다. 해당 프리팹을 건너뜁니다.");
                continue;
            }

            // 프리팹 오브젝트가 할당되어 있는지 확인
            if (pagePrefab.prefab == null)
            {
                Debug.LogWarning($"PagePrefab '{pagePrefab.name}'의 prefab이 null입니다. 해당 프리팹을 건너뜁니다.");
                continue;
            }

            // 프리팹 오브젝트를 Instantiate하여 생성
            GameObject instantiated = Instantiate(pagePrefab.prefab, Vector3.zero, Quaternion.identity);
            instantiated.name = pagePrefab.name;

            // 프리팹을 비활성화 상태로 설정
            instantiated.SetActive(false);

            // 딕셔너리에 이미 동일한 키가 있는지 확인하여 중복을 방지
            if (!_spawnedPrefabs.ContainsKey(instantiated.name))
            {
                _spawnedPrefabs.Add(instantiated.name, instantiated);
            }
            else
            {
                Debug.LogWarning($"_spawnedPrefabs 딕셔너리에 이미 '{instantiated.name}' 키가 존재합니다. 중복을 피하기 위해 추가하지 않습니다.");
                Destroy(instantiated); // 중복된 경우 생성된 프리팹 파괴
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
            Debug.LogError("ARTrackedImageManager가 할당되지 않았습니다.");
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
    /// 이미지 추적 변경 시 호출되는 콜백 메서드
    /// </summary>
    /// <param name="eventArgs">추적된 이미지 변경 사항</param>
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // 추가된 이미지 처리
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }

        // 업데이트된 이미지 처리
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }

        // 제거된 이미지 처리
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            string imageName = trackedImage.referenceImage.name;

            if (string.IsNullOrEmpty(imageName))
            {
                Debug.LogWarning("Removed ARTrackedImage의 name이 null 또는 비어 있습니다.");
                continue;
            }

            if (_spawnedPrefabs.ContainsKey(imageName))
            {
                GameObject prefab = _spawnedPrefabs[imageName];
                prefab.SetActive(false);
                Debug.Log($"Prefab '{imageName}' 비활성화");

                // 해당 프리팹의 나레이션이 재생 중이면 정지
                if (_currentChapter == imageName)
                {
                    AudioSource audioSource = prefab.GetComponent<AudioSource>();
                    if (audioSource != null && audioSource.isPlaying)
                    {
                        audioSource.Stop();
                        Debug.Log($"Prefab '{imageName}'의 오디오 정지");
                    }
                    _currentChapter = "";
                }
            }
            else
            {
                Debug.LogWarning($"_spawnedPrefabs 딕셔너리에 '{imageName}' 키가 존재하지 않습니다.");
            }
        }
    }

    /// <summary>
    /// 추적된 이미지에 대응하는 프리팹을 활성화하고 나레이션을 재생하는 메서드
    /// </summary>
    /// <param name="trackedImage">ARTrackedImage 인스턴스</param>
    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name;

        if (string.IsNullOrEmpty(imageName))
        {
            Debug.LogWarning("ARTrackedImage의 name이 null 또는 비어 있습니다.");
            return;
        }

        if (!_spawnedPrefabs.ContainsKey(imageName))
        {
            Debug.LogWarning($"_spawnedPrefabs 딕셔너리에 '{imageName}' 키가 존재하지 않습니다.");
            return;
        }

        GameObject prefab = _spawnedPrefabs[imageName];

        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            // 프리팹의 위치와 회전을 업데이트하고 활성화
            prefab.transform.position = trackedImage.transform.position;
            prefab.transform.rotation = trackedImage.transform.rotation;
            prefab.SetActive(true);
            Debug.Log($"Prefab '{imageName}' 활성화 및 위치/회전 업데이트");

            // 현재 재생 중인 오디오가 있다면 정지
            if (!string.IsNullOrEmpty(_currentChapter) && _currentChapter != imageName)
            {
                GameObject previousPrefab = _spawnedPrefabs[_currentChapter];
                AudioSource previousAudio = previousPrefab.GetComponent<AudioSource>();
                if (previousAudio != null && previousAudio.isPlaying)
                {
                    previousAudio.Stop();
                    Debug.Log($"기존 프리팹 '{_currentChapter}'의 오디오 정지");
                }
            }

            // 해당 이미지의 나레이션 오디오 재생
            PlayNarration(imageName);
        }
        else
        {
            // 프리팹 비활성화
            prefab.SetActive(false);
            Debug.Log($"Prefab '{imageName}' 비활성화 (Tracking 상태 아님)");

            // 해당 프리팹의 나레이션이 재생 중이면 정지
            if (_currentChapter == imageName)
            {
                AudioSource audioSource = prefab.GetComponent<AudioSource>();
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                    Debug.Log($"Prefab '{imageName}'의 오디오 정지");
                }
                _currentChapter = "";
            }
        }
    }

    /// <summary>
    /// 이미지 이름에 해당하는 나레이션 오디오를 재생하는 메서드
    /// </summary>
    /// <param name="imageName">이미지 이름 (chap1, chap2, ...)</param>
    private void PlayNarration(string imageName)
    {
        foreach (PagePrefab pagePrefab in pagePrefabs)
        {
            if (pagePrefab.name == imageName)
            {
                // 나레이션 오디오가 할당되어 있는지 확인
                if (pagePrefab.narration != null)
                {
                    GameObject prefab = _spawnedPrefabs[imageName];
                    AudioSource audioSource = prefab.GetComponent<AudioSource>();

                    if (audioSource != null)
                    {
                        audioSource.clip = pagePrefab.narration;
                        audioSource.Play();
                        _currentChapter = imageName;
                        Debug.Log($"Prefab '{imageName}'의 나레이션 오디오 재생 시작");
                    }
                    else
                    {
                        Debug.LogWarning($"Prefab '{imageName}'에 AudioSource 컴포넌트가 없습니다.");
                    }
                }
                else
                {
                    Debug.LogWarning($"'{imageName}'에 해당하는 나레이션 오디오가 없습니다.");
                }
                break;
            }
        }
    }

    private void Update()
    {
        // 터치 입력 감지
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // 터치 시작 시 처리
            if (touch.phase == TouchPhase.Began)
            {
                // UI 요소를 터치한 경우 무시
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return;
                }

                // 터치한 위치에 레이캐스트 수행
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    GameObject touchedObject = hit.transform.gameObject;

                    // 터치한 오브젝트가 스폰된 프리팹인지 확인
                    foreach (KeyValuePair<string, GameObject> kvp in _spawnedPrefabs)
                    {
                        if (touchedObject == kvp.Value)
                        {
                            // 현재 재생 중인 오디오가 해당 프리팹의 것인지 확인
                            if (_currentChapter == kvp.Key)
                            {
                                AudioSource audioSource = kvp.Value.GetComponent<AudioSource>();
                                if (audioSource != null)
                                {
                                    if (audioSource.isPlaying)
                                    {
                                        audioSource.Pause();
                                        Debug.Log($"Prefab '{kvp.Key}'의 오디오 일시정지");
                                    }
                                    else
                                    {
                                        // 다른 오디오가 재생 중이면 정지
                                        if (!string.IsNullOrEmpty(_currentChapter) && _currentChapter != kvp.Key)
                                        {
                                            GameObject previousPrefab = _spawnedPrefabs[_currentChapter];
                                            AudioSource previousAudio = previousPrefab.GetComponent<AudioSource>();
                                            if (previousAudio != null && previousAudio.isPlaying)
                                            {
                                                previousAudio.Stop();
                                                Debug.Log($"기존 프리팹 '{_currentChapter}'의 오디오 정지");
                                            }
                                        }

                                        audioSource.Play();
                                        Debug.Log($"Prefab '{kvp.Key}'의 오디오 재생");
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
