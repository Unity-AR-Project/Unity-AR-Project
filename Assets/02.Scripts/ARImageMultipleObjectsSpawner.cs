using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// 인식할 이미지의 이름과 생성할 프리팹을 정의하는 구조체
/// </summary>
[System.Serializable]
public struct PagePrefabs
{
    public string name; // 인식할 이미지의 이름
    public GameObject prefab; // 생성할 장면 프리팹
}

/// <summary>
/// AR 이미지 인식 시 여러 오브젝트를 스폰 및 관리하는 클래스
/// </summary>
public class ARImageMultipleObjectsSpawner : MonoBehaviour
{
    // ARTrackedImageManager 컴포넌트를 참조하기 위한 변수
    private ARTrackedImageManager _imgManager;

    // Inspector에서 설정할 PagePrefabs 배열
    public PagePrefabs[] prefabs;

    // 스폰된 프리팹을 이름을 키로 하는 딕셔너리로 관리
    private Dictionary<string, GameObject> _spawnedPrefabs = new Dictionary<string, GameObject>();

    /// <summary>
    /// 게임 오브젝트가 활성화되기 전에 호출되는 메서드
    /// 프리팹을 초기화하고 딕셔너리에 추가
    /// </summary>
    private void Awake()
    {
        // ARTrackedImageManager 컴포넌트 가져오기
        _imgManager = GetComponent<ARTrackedImageManager>();

        // prefabs 배열이 비어있는지 확인
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogError("prefabs 배열이 비어 있습니다. Inspector에서 프리팹을 설정하세요.");
            return;
        }

        // prefabs 배열을 순회하며 각 프리팹을 초기화
        foreach (PagePrefabs prefab in prefabs)
        {
            // 프리팹의 이름이 유효한지 확인
            if (string.IsNullOrEmpty(prefab.name))
            {
                Debug.LogWarning("PagePrefabs의 name이 null 또는 비어 있습니다. 해당 프리팹을 건너뜁니다.");
                continue;
            }

            // 프리팹 오브젝트가 할당되어 있는지 확인
            if (prefab.prefab == null)
            {
                Debug.LogWarning($"PagePrefabs '{prefab.name}'의 prefab이 null입니다. 해당 프리팹을 건너뜁니다.");
                continue;
            }

            // 프리팹을 Instantiate하여 생성하고 딕셔너리에 추가
            GameObject instantiated = Instantiate(prefab.prefab, Vector3.zero, Quaternion.identity);
            instantiated.name = prefab.name;

            // 딕셔너리에 이미 동일한 키가 있는지 확인하여 중복을 방지
            if (!_spawnedPrefabs.ContainsKey(instantiated.name))
            {
                _spawnedPrefabs.Add(instantiated.name, instantiated);
                instantiated.SetActive(false); // 초기에는 비활성화 상태로 설정
            }
            else
            {
                Debug.LogWarning($"_spawnedPrefabs 딕셔너리에 이미 '{instantiated.name}' 키가 존재합니다. 중복을 피하기 위해 추가하지 않습니다.");
            }
        }
    }

    /// <summary>
    /// 게임 오브젝트가 활성화될 때 호출되는 메서드
    /// trackedImagesChanged 이벤트에 OnImageChanged 메서드를 구독
    /// </summary>
    private void OnEnable()
    {
        if (_imgManager != null)
        {
            _imgManager.trackedImagesChanged += OnImageChanged;
        }
        else
        {
            Debug.LogError("ARTrackedImageManager가 할당되지 않았습니다.");
        }
    }

    /// <summary>
    /// 게임 오브젝트가 비활성화될 때 호출되는 메서드
    /// trackedImagesChanged 이벤트에서 OnImageChanged 메서드를 해제
    /// </summary>
    private void OnDisable()
    {
        if (_imgManager != null)
        {
            _imgManager.trackedImagesChanged -= OnImageChanged;
        }
    }

    /// <summary>
    /// ARTrackedImagesChangedEventArgs를 통해 이미지의 추가, 업데이트, 제거 이벤트를 처리
    /// </summary>
    /// <param name="args">추적된 이미지의 변경 사항</param>
    private void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        // 추가된 이미지 처리
        foreach (ARTrackedImage img in args.added)
        {
            string imgName = img.referenceImage.name; // ?. 연산자 제거
            Debug.Log($"Added Image: {(string.IsNullOrEmpty(imgName) ? "null" : imgName)}");
            UpdateSpawned(img);
            Debug.Log($"{(string.IsNullOrEmpty(imgName) ? "Unknown" : imgName)} 추가");
        }

        // 업데이트된 이미지 처리
        foreach (ARTrackedImage img in args.updated)
        {
            string imgName = img.referenceImage.name; // ?. 연산자 제거
            Debug.Log($"Updated Image: {(string.IsNullOrEmpty(imgName) ? "null" : imgName)}");
            UpdateSpawned(img);
        }

        // 제거된 이미지 처리
        foreach (ARTrackedImage img in args.removed)
        {
            string imgName = img.referenceImage.name; // ?. 연산자 제거
            Debug.Log($"Removed Image: {(string.IsNullOrEmpty(imgName) ? "null" : imgName)}");

            // 이미지 이름이 유효한지 확인
            if (string.IsNullOrEmpty(imgName))
            {
                Debug.LogWarning("Removed ARTrackedImage의 name이 null 또는 비어 있습니다.");
                continue;
            }

            // 딕셔너리에서 해당 프리팹을 찾아 비활성화
            if (_spawnedPrefabs.TryGetValue(imgName, out GameObject prefab))
            {
                prefab.SetActive(false);
                Debug.Log($"Prefab '{imgName}' 비활성화");
            }
            else
            {
                Debug.LogWarning($"_spawnedPrefabs 딕셔너리에 '{imgName}' 키가 존재하지 않습니다.");
            }
        }
    }

    /// <summary>
    /// 주어진 ARTrackedImage에 따라 해당 프리팹을 활성화/비활성화하고 위치와 회전을 업데이트
    /// </summary>
    /// <param name="img">추적된 이미지</param>
    private void UpdateSpawned(ARTrackedImage img)
    {
        // 이미지의 이름이 유효한지 확인
        if (string.IsNullOrEmpty(img.referenceImage.name))
        {
            Debug.LogWarning("ARTrackedImage의 name이 null 또는 비어 있습니다.");
            return;
        }

        string name = img.referenceImage.name;

        // 딕셔너리에서 해당 이름의 프리팹을 가져옴
        if (_spawnedPrefabs.TryGetValue(name, out GameObject spawned))
        {
            if (img.trackingState == TrackingState.Tracking)
            {
                // 트래킹 상태일 때 프리팹의 위치와 회전을 업데이트하고 활성화
                spawned.transform.position = img.transform.position;
                spawned.transform.rotation = img.transform.rotation;
                spawned.SetActive(true);
                Debug.Log($"Prefab '{name}' 활성화 및 위치/회전 업데이트");
            }
            else
            {
                // 트래킹 상태가 아닐 때 프리팹 비활성화
                spawned.SetActive(false);
                Debug.Log($"Prefab '{name}' 비활성화 (Tracking 상태 아님)");
            }
        }
        else
        {
            Debug.LogWarning($"_spawnedPrefabs 딕셔너리에 '{name}' 키가 존재하지 않습니다.");
        }
    }
}
