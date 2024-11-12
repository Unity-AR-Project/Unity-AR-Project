// AR 및 Unity 관련 네임스페이스
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// 각 이미지에 대해 생성할 프리팹 정보 저장을 위한 구조체
[System.Serializable]
public struct PagePrefabs
{
    public string name; // 인식할 이미지의 이름
    public GameObject prefab; // 인식된 이미지에 해당하는 프리팹
}

public class ARImageMultipleObjectsSpawner : MonoBehaviour
{
    private ARTrackedImageManager imgManager; // AR 이미지 추적을 관리하는 ARTrackedImageManager 변수

    public PagePrefabs[] prefabs; // 생성할 프리팹을 저장하는 배열
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>(); // 이름을 키로 사용하는 프리팹 딕셔너리

    public bool isHandling = false;

    private void Awake()
    {
        imgManager = GetComponent<ARTrackedImageManager>(); // ARTrackedImageManager 컴포넌트를 가져옴

        // 모든 프리팹을 순회하면서 인스턴스를 생성
        foreach (PagePrefabs prefab in prefabs)
        {
            // 프리팹을 인스턴스화하고, 초기 위치는 (0, 0, 0)으로 설정 (처음에는 보이지 않도록)
            GameObject instantiated = Instantiate(prefab.prefab, Vector3.zero, Quaternion.identity);
            instantiated.name = prefab.name; // 인스턴스의 이름을 해당 이미지 이름으로 설정
            spawnedPrefabs.Add(instantiated.name, instantiated); // 딕셔너리에 이름과 프리팹을 추가
            instantiated.SetActive(false); // 처음에는 프리팹을 비활성화
        }
    }

    private void Update()
    {
        // 현재는 빈 Update 메소드, 나중에 프레임 단위로 처리할 내용이 있을 경우 작성 가능
    }

    // 스크립트가 활성화될 때 호출되는 메소드
    private void OnEnable()
    {
        imgManager.trackablesChanged.AddListener(OnImageChanged); // AR 이미지 추적 상태가 변경될 때마다 호출될 메소드 등록
    }

    // 스크립트가 비활성화될 때 호출되는 메소드
    private void OnDisable()
    {
        imgManager.trackablesChanged.RemoveListener(OnImageChanged); // AR 이미지 추적 상태 변경 이벤트 리스너 제거
    }

    // trackablesChanged 이벤트 핸들러, 이미지를 추가, 업데이트, 제거할 때 호출됨
    private void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        // 추가된 이미지에 대해 처리
        foreach (ARTrackedImage img in args.added)
        {
            UpdateSpawned(img); // 해당 이미지에 대응하는 프리팹을 업데이트
            Debug.Log($"{img.name} 추가"); // 추가된 이미지 로그 출력
        }

        // 업데이트된 이미지에 대해 처리
        foreach (ARTrackedImage img in args.updated)
        {
            UpdateSpawned(img); // 해당 이미지에 대응하는 프리팹을 업데이트
        }

        // 제거된 이미지에 대해 처리
        foreach (KeyValuePair<TrackableId, ARTrackedImage> img in args.removed)
        {
            // 제거된 이미지에 대응하는 프리팹을 비활성화
            spawnedPrefabs[img.Value.name].SetActive(false);
        }
    }

    // 추적 중인 이미지를 기준으로 프리팹을 업데이트
    private void UpdateSpawned(ARTrackedImage img)
    {
        string name = img.referenceImage.name; // 추적 중인 이미지의 이름
        Debug.Log($"{img.referenceImage.name} 추가"); // 추가된 이미지 이름 로그 출력
        GameObject spawned = spawnedPrefabs[name]; // 딕셔너리에서 해당 이미지 이름에 대응하는 프리팹을 찾음

        Debug.Log("spawned " + spawned.name); // 생성된 프리팹의 이름 로그 출력

        // 이미지가 잘 추적되고 있을 때만 업데이트 진행
        if (img.trackingState == TrackingState.Tracking)
        {
            if(isHandling)
            {
                return;
            }

            // 이미지의 위치와 회전 상태를 로그로 출력
            Debug.Log("Updating image " + img.referenceImage.name + " at " + img.transform.position);

            // 추적 중인 이미지의 위치와 회전을 해당 프리팹에 반영
            spawned.transform.position = img.transform.position;
            spawned.transform.rotation = img.transform.rotation;
            spawned.SetActive(true); // 추적이 잘 되면 프리팹을 활성화
        }
        else
        {
            // 추적 상태가 좋지 않으면 프리팹을 비활성화
            spawned.SetActive(false);
        }
    }
}
