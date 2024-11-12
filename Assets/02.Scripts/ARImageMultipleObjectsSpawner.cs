using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.XR.ARCore;

/// <summary>
/// 인식할 이미지의 이름과 생성할 프리팹, 나레이션 오디오를 정의하는 구조체
/// </summary>
[System.Serializable]
public struct PagePrefab
{
    public string name;          // 인식할 이미지의 이름 (예: chap1, chap2, ...)
    public GameObject prefab;    // 해당 이미지에 대응하는 프리팹 오브젝트
    public AudioClip narration;  // 해당 이미지에 대응하는 나레이션 오디오 클립
}

/// <summary>
/// AR 이미지 인식 시 여러 오브젝트를 스폰 및 관리하는 클래스
/// 각 이미지에 맞는 나레이션 오디오를 재생하고, 이미지 변경 시 오디오를 전환
/// 터치 입력을 통해 오디오를 일시정지/재생할 수 있음
/// </summary>
public class ARImageMultipleObjectsSpawner : MonoBehaviour
{
    private ARTrackedImageManager _trackedImageManager; // ARTrackedImageManager 컴포넌트를 참조
    public ARSession arSession;

    [Tooltip("각 챕터 이미지에 대응하는 프리팹과 나레이션 오디오를 설정합니다.")]
    public PagePrefab[] pagePrefabs; // 각 챕터 이미지에 대한 프리팹과 오디오 설정 배열

    private Dictionary<string, GameObject> _spawnedPrefabs = new Dictionary<string, GameObject>(); // 스폰된 프리팹을 관리하는 딕셔너리
    private string _currentChapter = ""; // 현재 활성화된 챕터 이름을 저장


    /// <summary>
    /// 스크립트가 활성화되기 전에 호출되는 메서드
    /// 초기 설정을 담당
    /// </summary>
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

        // pagePrefabs 배열을 순회하며 각 프리팹을 초기화
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
            instantiated.name = pagePrefab.name; // 프리팹의 이름을 설정
            instantiated.SetActive(false); // 초기에는 비활성화 상태로 설정

            // 딕셔너리에 프리팹을 추가, 중복 키 방지
            if (!_spawnedPrefabs.ContainsKey(instantiated.name))
            {
                _spawnedPrefabs.Add(instantiated.name, instantiated);
            }
            else
            {
                Debug.LogWarning($"_spawnedPrefabs 딕셔너리에 '{instantiated.name}' 키가 이미 존재합니다. 중복을 피하기 위해 추가하지 않습니다.");
                Destroy(instantiated); // 중복된 경우 생성된 프리팹 파괴
            }
        }
    }

    /// <summary>
    /// 스크립트가 활성화될 때 호출되는 메서드
    /// ARTrackedImageManager의 추적 이미지 변경 이벤트에 리스너 등록
    /// </summary>
    private void OnEnable()
    {
        if (_trackedImageManager != null)
        {
            RestartARSession();

            // 추적 이미지 변경 시 OnTrackedImagesChanged 메서드 호출
            _trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
        }
        else
        {
            Debug.LogError("ARTrackedImageManager가 할당되지 않았습니다.");
        }
    }

    /// <summary>
    /// 스크립트가 비활성화될 때 호출되는 메서드
    /// ARTrackedImageManager의 추적 이미지 변경 이벤트에서 리스너 해제
    /// </summary>
    private void OnDisable()
    {
        if (_trackedImageManager != null)
        {
            _trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
        }
    }

    /// <summary>
    /// 추적된 이미지 변경 시 호출되는 콜백 메서드
    /// </summary>
    /// <param name="eventArgs">추적된 이미지 변경 사항을 포함하는 이벤트 인자</param>
    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        // 추가된 이미지들 처리
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage); // 프리팹 위치/회전 업데이트 및 활성화
        }

        // 업데이트된 이미지들 처리
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage); // 프리팹 위치/회전 업데이트
        }

        // 제거된 이미지들 처리
        foreach (KeyValuePair<TrackableId, ARTrackedImage> trackedImage in eventArgs.removed)
        {
            string imageName = trackedImage.Value.referenceImage.name;

            // 이미지 이름이 유효한지 확인
            if (string.IsNullOrEmpty(imageName))
            {
                Debug.LogWarning("제거된 ARTrackedImage의 name이 null 또는 비어 있습니다.");
                continue;
            }

            // 딕셔너리에 해당 이름의 프리팹이 있는지 확인
            if (_spawnedPrefabs.ContainsKey(imageName))
            {
                GameObject prefab = _spawnedPrefabs[imageName];
                prefab.SetActive(false); // 프리팹 비활성화
                Debug.Log($"Prefab '{imageName}' 비활성화");

                // 현재 활성화된 챕터가 제거된 이미지라면 오디오 정지
                if (_currentChapter == imageName)
                {
                    AudioSource audioSource = prefab.GetComponent<AudioSource>();
                    if (audioSource != null && audioSource.isPlaying)
                    {
                        audioSource.Stop(); // 오디오 정지
                        Debug.Log($"Prefab '{imageName}'의 오디오 정지");
                    }
                    _currentChapter = ""; // 현재 챕터 초기화
                }
            }
            else
            {
                Debug.LogWarning($"_spawnedPrefabs 딕셔너리에 '{imageName}' 키가 존재하지 않습니다.");
            }
        }
    }

    /// <summary>
    /// 추적된 이미지를 업데이트하고 필요한 경우 나레이션을 재생합니다.
    /// </summary>
    /// <param name="trackedImage">ARTrackedImage 인스턴스</param>
    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string imageName = trackedImage.referenceImage.name; // 추적된 이미지의 이름 가져오기

        // 이미지 이름이 유효한지 확인
        if (string.IsNullOrEmpty(imageName))
        {
            Debug.LogWarning("ARTrackedImage의 name이 null 또는 비어 있습니다.");
            return;
        }

        // 딕셔너리에 해당 이름의 프리팹이 있는지 확인
        if (!_spawnedPrefabs.ContainsKey(imageName))
        {
            Debug.LogWarning($"_spawnedPrefabs 딕셔너리에 '{imageName}' 키가 존재하지 않습니다.");
            return;
        }

        GameObject prefab = _spawnedPrefabs[imageName]; // 해당 프리팹 가져오기

        // 이미지가 추적 중인 경우
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            //흔들림 현상을 줄이기 위해 프리팹 위치를 추적된 이미지 위치에 즉시 이동시키는 대신 Lerp 또는 SmoothDamp 함수를 사용하여 부드럽게 이동
            //prefab.transform.position = Vector3.SmoothDamp(prefab.transform.position, trackedImage.transform.position, ref velocity, smoothTime);
            //prefab.transform.rotation = Quaternion.Slerp(prefab.transform.rotation, trackedImage.transform.rotation, Time.deltaTime * 5f);

            // 프리팹의 위치와 회전을 업데이트
            prefab.transform.position = trackedImage.transform.position;
            prefab.transform.rotation = trackedImage.transform.rotation;
            prefab.SetActive(true); // 프리팹 활성화
            Debug.Log($"Prefab '{imageName}' 활성화 및 위치/회전 업데이트");

            // 현재 챕터가 변경된 경우 이전 챕터의 오디오 정지 및 새로운 챕터의 나레이션 재생
            if (_currentChapter != imageName)
            {
                // 이전 챕터가 있다면 오디오 정지
                if (!string.IsNullOrEmpty(_currentChapter))
                {
                    GameObject previousPrefab = _spawnedPrefabs[_currentChapter];
                    AudioSource previousAudio = previousPrefab.GetComponent<AudioSource>();
                    if (previousAudio != null && previousAudio.isPlaying)
                    {
                        previousAudio.Stop();
                        Debug.Log($"이전 프리팹 '{_currentChapter}'의 오디오 정지");
                    }
                }

                // 새로운 챕터의 나레이션 재생
                PlayNarration(imageName);
            }
        }
        else
        {
            // 이미지 추적이 되지 않는 경우 프리팹 비활성화
            prefab.SetActive(false);
            Debug.Log($"Prefab '{imageName}' 비활성화 (추적 중 아님)");

            // 현재 챕터가 제거된 이미지라면 오디오 정지
            if (_currentChapter == imageName)
            {
                AudioSource audioSource = prefab.GetComponent<AudioSource>();
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                    Debug.Log($"Prefab '{imageName}'의 오디오 정지");
                }
                _currentChapter = ""; // 현재 챕터 초기화
            }
        }
    }

    /// <summary>
    /// 지정된 이미지 이름에 해당하는 나레이션을 재생합니다.
    /// </summary>
    /// <param name="imageName">이미지 이름</param>
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
                        audioSource.clip = pagePrefab.narration; // 오디오 클립 설정
                        audioSource.Play(); // 오디오 재생
                        _currentChapter = imageName; // 현재 챕터 업데이트
                        Debug.Log($"Prefab '{imageName}'의 나레이션 재생 시작");
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
                break; // 해당 챕터를 찾았으므로 루프 종료
            }
        }
    }

    // 기존 이미지가 사라져도 트래킹 상태가 유지되어 프리팹이 안사라지는 부분을 해결하기 위한 함수와 코루틴
    private void RestartARSession()
    {
        StartCoroutine(RestartSessionCoroutine());
    }

    /// <summary>
    /// AR Session을 껐다 키는 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator RestartSessionCoroutine()
    {
        if (arSession != null)
        {
            arSession.enabled = false;  // AR 세션 비활성화
            yield return null;          // 한 프레임 대기
            arSession.enabled = true;   // AR 세션 다시 활성화
            Debug.Log("AR 세션이 재시작되었습니다.");
        }
        else
        {
            Debug.LogWarning("ARSession 컴포넌트가 설정되지 않았습니다.");
        }
    }

    /// <summary>
    /// 매 프레임마다 호출되는 메서드
    /// 터치 입력을 감지하여 오디오 일시정지/재생 기능을 수행
    /// </summary>
    private void Update()
    {
        // 화면에 터치가 하나 이상 있는지 확인
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // 첫 번째 터치 정보 가져오기

            // 터치가 시작된 순간인 경우 처리
            if (touch.phase == TouchPhase.Began)
            {
                // 터치한 위치가 UI 요소 위인 경우 무시
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return;
                }

                // 터치한 위치에 레이캐스트 수행
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    GameObject touchedObject = hit.transform.gameObject; // 터치한 오브젝트 가져오기

                    // 스폰된 모든 프리팹을 순회하며 터치한 오브젝트와 일치하는지 확인
                    foreach (KeyValuePair<string, GameObject> kvp in _spawnedPrefabs)
                    {
                        if (touchedObject == kvp.Value)
                        {
                            // 현재 챕터가 터치한 프리팹과 일치하는지 확인
                            if (_currentChapter == kvp.Key)
                            {
                                AudioSource audioSource = kvp.Value.GetComponent<AudioSource>();
                                if (audioSource != null)
                                {
                                    if (audioSource.isPlaying)
                                    {
                                        audioSource.Pause(); // 오디오 일시정지
                                        Debug.Log($"Prefab '{kvp.Key}'의 오디오 일시정지");
                                    }
                                    else
                                    {
                                        audioSource.Play(); // 오디오 재생
                                        Debug.Log($"Prefab '{kvp.Key}'의 오디오 재생");
                                    }
                                }
                            }
                            break; // 일치하는 프리팹을 찾았으므로 루프 종료
                        }
                    }
                }
            }
        }
    }
}
