using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// �ν��� �̹����� �̸��� ������ �������� �����ϴ� ����ü
/// </summary>
[System.Serializable]
public struct PagePrefabs
{
    public string name; // �ν��� �̹����� �̸�
    public GameObject prefab; // ������ ��� ������
}

/// <summary>
/// AR �̹��� �ν� �� ���� ������Ʈ�� ���� �� �����ϴ� Ŭ����
/// </summary>
public class ARImageMultipleObjectsSpawner : MonoBehaviour
{
    // ARTrackedImageManager ������Ʈ�� �����ϱ� ���� ����
    private ARTrackedImageManager _imgManager;

    // Inspector���� ������ PagePrefabs �迭
    public PagePrefabs[] prefabs;

    // ������ �������� �̸��� Ű�� �ϴ� ��ųʸ��� ����
    private Dictionary<string, GameObject> _spawnedPrefabs = new Dictionary<string, GameObject>();

    /// <summary>
    /// ���� ������Ʈ�� Ȱ��ȭ�Ǳ� ���� ȣ��Ǵ� �޼���
    /// �������� �ʱ�ȭ�ϰ� ��ųʸ��� �߰�
    /// </summary>
    private void Awake()
    {
        // ARTrackedImageManager ������Ʈ ��������
        _imgManager = GetComponent<ARTrackedImageManager>();

        // prefabs �迭�� ����ִ��� Ȯ��
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogError("prefabs �迭�� ��� �ֽ��ϴ�. Inspector���� �������� �����ϼ���.");
            return;
        }

        // prefabs �迭�� ��ȸ�ϸ� �� �������� �ʱ�ȭ
        foreach (PagePrefabs prefab in prefabs)
        {
            // �������� �̸��� ��ȿ���� Ȯ��
            if (string.IsNullOrEmpty(prefab.name))
            {
                Debug.LogWarning("PagePrefabs�� name�� null �Ǵ� ��� �ֽ��ϴ�. �ش� �������� �ǳʶݴϴ�.");
                continue;
            }

            // ������ ������Ʈ�� �Ҵ�Ǿ� �ִ��� Ȯ��
            if (prefab.prefab == null)
            {
                Debug.LogWarning($"PagePrefabs '{prefab.name}'�� prefab�� null�Դϴ�. �ش� �������� �ǳʶݴϴ�.");
                continue;
            }

            // �������� Instantiate�Ͽ� �����ϰ� ��ųʸ��� �߰�
            GameObject instantiated = Instantiate(prefab.prefab, Vector3.zero, Quaternion.identity);
            instantiated.name = prefab.name;

            // ��ųʸ��� �̹� ������ Ű�� �ִ��� Ȯ���Ͽ� �ߺ��� ����
            if (!_spawnedPrefabs.ContainsKey(instantiated.name))
            {
                _spawnedPrefabs.Add(instantiated.name, instantiated);
                instantiated.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ ���·� ����
            }
            else
            {
                Debug.LogWarning($"_spawnedPrefabs ��ųʸ��� �̹� '{instantiated.name}' Ű�� �����մϴ�. �ߺ��� ���ϱ� ���� �߰����� �ʽ��ϴ�.");
            }
        }
    }

    /// <summary>
    /// ���� ������Ʈ�� Ȱ��ȭ�� �� ȣ��Ǵ� �޼���
    /// trackedImagesChanged �̺�Ʈ�� OnImageChanged �޼��带 ����
    /// </summary>
    private void OnEnable()
    {
        if (_imgManager != null)
        {
            _imgManager.trackedImagesChanged += OnImageChanged;
        }
        else
        {
            Debug.LogError("ARTrackedImageManager�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    /// <summary>
    /// ���� ������Ʈ�� ��Ȱ��ȭ�� �� ȣ��Ǵ� �޼���
    /// trackedImagesChanged �̺�Ʈ���� OnImageChanged �޼��带 ����
    /// </summary>
    private void OnDisable()
    {
        if (_imgManager != null)
        {
            _imgManager.trackedImagesChanged -= OnImageChanged;
        }
    }

    /// <summary>
    /// ARTrackedImagesChangedEventArgs�� ���� �̹����� �߰�, ������Ʈ, ���� �̺�Ʈ�� ó��
    /// </summary>
    /// <param name="args">������ �̹����� ���� ����</param>
    private void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        // �߰��� �̹��� ó��
        foreach (ARTrackedImage img in args.added)
        {
            string imgName = img.referenceImage.name; // ?. ������ ����
            Debug.Log($"Added Image: {(string.IsNullOrEmpty(imgName) ? "null" : imgName)}");
            UpdateSpawned(img);
            Debug.Log($"{(string.IsNullOrEmpty(imgName) ? "Unknown" : imgName)} �߰�");
        }

        // ������Ʈ�� �̹��� ó��
        foreach (ARTrackedImage img in args.updated)
        {
            string imgName = img.referenceImage.name; // ?. ������ ����
            Debug.Log($"Updated Image: {(string.IsNullOrEmpty(imgName) ? "null" : imgName)}");
            UpdateSpawned(img);
        }

        // ���ŵ� �̹��� ó��
        foreach (ARTrackedImage img in args.removed)
        {
            string imgName = img.referenceImage.name; // ?. ������ ����
            Debug.Log($"Removed Image: {(string.IsNullOrEmpty(imgName) ? "null" : imgName)}");

            // �̹��� �̸��� ��ȿ���� Ȯ��
            if (string.IsNullOrEmpty(imgName))
            {
                Debug.LogWarning("Removed ARTrackedImage�� name�� null �Ǵ� ��� �ֽ��ϴ�.");
                continue;
            }

            // ��ųʸ����� �ش� �������� ã�� ��Ȱ��ȭ
            if (_spawnedPrefabs.TryGetValue(imgName, out GameObject prefab))
            {
                prefab.SetActive(false);
                Debug.Log($"Prefab '{imgName}' ��Ȱ��ȭ");
            }
            else
            {
                Debug.LogWarning($"_spawnedPrefabs ��ųʸ��� '{imgName}' Ű�� �������� �ʽ��ϴ�.");
            }
        }
    }

    /// <summary>
    /// �־��� ARTrackedImage�� ���� �ش� �������� Ȱ��ȭ/��Ȱ��ȭ�ϰ� ��ġ�� ȸ���� ������Ʈ
    /// </summary>
    /// <param name="img">������ �̹���</param>
    private void UpdateSpawned(ARTrackedImage img)
    {
        // �̹����� �̸��� ��ȿ���� Ȯ��
        if (string.IsNullOrEmpty(img.referenceImage.name))
        {
            Debug.LogWarning("ARTrackedImage�� name�� null �Ǵ� ��� �ֽ��ϴ�.");
            return;
        }

        string name = img.referenceImage.name;

        // ��ųʸ����� �ش� �̸��� �������� ������
        if (_spawnedPrefabs.TryGetValue(name, out GameObject spawned))
        {
            if (img.trackingState == TrackingState.Tracking)
            {
                // Ʈ��ŷ ������ �� �������� ��ġ�� ȸ���� ������Ʈ�ϰ� Ȱ��ȭ
                spawned.transform.position = img.transform.position;
                spawned.transform.rotation = img.transform.rotation;
                spawned.SetActive(true);
                Debug.Log($"Prefab '{name}' Ȱ��ȭ �� ��ġ/ȸ�� ������Ʈ");
            }
            else
            {
                // Ʈ��ŷ ���°� �ƴ� �� ������ ��Ȱ��ȭ
                spawned.SetActive(false);
                Debug.Log($"Prefab '{name}' ��Ȱ��ȭ (Tracking ���� �ƴ�)");
            }
        }
        else
        {
            Debug.LogWarning($"_spawnedPrefabs ��ųʸ��� '{name}' Ű�� �������� �ʽ��ϴ�.");
        }
    }
}
