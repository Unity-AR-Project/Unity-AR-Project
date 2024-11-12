// AR �� Unity ���� ���ӽ����̽�
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

// �� �̹����� ���� ������ ������ ���� ������ ���� ����ü
[System.Serializable]
public struct PagePrefabs
{
    public string name; // �ν��� �̹����� �̸�
    public GameObject prefab; // �νĵ� �̹����� �ش��ϴ� ������
}

public class ARImageMultipleObjectsSpawner : MonoBehaviour
{
    private ARTrackedImageManager imgManager; // AR �̹��� ������ �����ϴ� ARTrackedImageManager ����

    public PagePrefabs[] prefabs; // ������ �������� �����ϴ� �迭
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>(); // �̸��� Ű�� ����ϴ� ������ ��ųʸ�

    public bool isHandling = false;

    private void Awake()
    {
        imgManager = GetComponent<ARTrackedImageManager>(); // ARTrackedImageManager ������Ʈ�� ������

        // ��� �������� ��ȸ�ϸ鼭 �ν��Ͻ��� ����
        foreach (PagePrefabs prefab in prefabs)
        {
            // �������� �ν��Ͻ�ȭ�ϰ�, �ʱ� ��ġ�� (0, 0, 0)���� ���� (ó������ ������ �ʵ���)
            GameObject instantiated = Instantiate(prefab.prefab, Vector3.zero, Quaternion.identity);
            instantiated.name = prefab.name; // �ν��Ͻ��� �̸��� �ش� �̹��� �̸����� ����
            spawnedPrefabs.Add(instantiated.name, instantiated); // ��ųʸ��� �̸��� �������� �߰�
            instantiated.SetActive(false); // ó������ �������� ��Ȱ��ȭ
        }
    }

    private void Update()
    {
        // ����� �� Update �޼ҵ�, ���߿� ������ ������ ó���� ������ ���� ��� �ۼ� ����
    }

    // ��ũ��Ʈ�� Ȱ��ȭ�� �� ȣ��Ǵ� �޼ҵ�
    private void OnEnable()
    {
        imgManager.trackablesChanged.AddListener(OnImageChanged); // AR �̹��� ���� ���°� ����� ������ ȣ��� �޼ҵ� ���
    }

    // ��ũ��Ʈ�� ��Ȱ��ȭ�� �� ȣ��Ǵ� �޼ҵ�
    private void OnDisable()
    {
        imgManager.trackablesChanged.RemoveListener(OnImageChanged); // AR �̹��� ���� ���� ���� �̺�Ʈ ������ ����
    }

    // trackablesChanged �̺�Ʈ �ڵ鷯, �̹����� �߰�, ������Ʈ, ������ �� ȣ���
    private void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        // �߰��� �̹����� ���� ó��
        foreach (ARTrackedImage img in args.added)
        {
            UpdateSpawned(img); // �ش� �̹����� �����ϴ� �������� ������Ʈ
            Debug.Log($"{img.name} �߰�"); // �߰��� �̹��� �α� ���
        }

        // ������Ʈ�� �̹����� ���� ó��
        foreach (ARTrackedImage img in args.updated)
        {
            UpdateSpawned(img); // �ش� �̹����� �����ϴ� �������� ������Ʈ
        }

        // ���ŵ� �̹����� ���� ó��
        foreach (KeyValuePair<TrackableId, ARTrackedImage> img in args.removed)
        {
            // ���ŵ� �̹����� �����ϴ� �������� ��Ȱ��ȭ
            spawnedPrefabs[img.Value.name].SetActive(false);
        }
    }

    // ���� ���� �̹����� �������� �������� ������Ʈ
    private void UpdateSpawned(ARTrackedImage img)
    {
        string name = img.referenceImage.name; // ���� ���� �̹����� �̸�
        Debug.Log($"{img.referenceImage.name} �߰�"); // �߰��� �̹��� �̸� �α� ���
        GameObject spawned = spawnedPrefabs[name]; // ��ųʸ����� �ش� �̹��� �̸��� �����ϴ� �������� ã��

        Debug.Log("spawned " + spawned.name); // ������ �������� �̸� �α� ���

        // �̹����� �� �����ǰ� ���� ���� ������Ʈ ����
        if (img.trackingState == TrackingState.Tracking)
        {
            if(isHandling)
            {
                return;
            }

            // �̹����� ��ġ�� ȸ�� ���¸� �α׷� ���
            Debug.Log("Updating image " + img.referenceImage.name + " at " + img.transform.position);

            // ���� ���� �̹����� ��ġ�� ȸ���� �ش� �����տ� �ݿ�
            spawned.transform.position = img.transform.position;
            spawned.transform.rotation = img.transform.rotation;
            spawned.SetActive(true); // ������ �� �Ǹ� �������� Ȱ��ȭ
        }
        else
        {
            // ���� ���°� ���� ������ �������� ��Ȱ��ȭ
            spawned.SetActive(false);
        }
    }
}
