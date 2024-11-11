using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[System.Serializable]
public struct PagePrefabs
{
    public string name; // �ν��� �̹����� �̸�
    public GameObject prefab; // ������ ��� ������
}

public class ARImageMultipleObjectsSpawner : MonoBehaviour
{
    private ARTrackedImageManager imgManager;

    public PagePrefabs[] prefabs;
    //private PagePrefabs[] prefabs = new PagePrefabs[2];

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    private void Awake()
    {
        imgManager = GetComponent<ARTrackedImageManager>();

        foreach (PagePrefabs prefab in prefabs)
        {
            // instantiate the prefab and store to the dictionary
            GameObject instantiated = Instantiate(prefab.prefab, Vector3.zero, Quaternion.identity);
            instantiated.name = prefab.name;
            spawnedPrefabs.Add(instantiated.name, instantiated);
            instantiated.SetActive(false);
        }

        //����� �ν�����â���� �������� �����ϰ� ������ ���߿��� �ڵ�� ã�Ƽ� ����� ����
        //prefabs[0].name = "PigBook";
        //prefabs[0].prefab = Resources.Load<GameObject>("Page_1");
        //prefabs[1].name = "Dog";
        //prefabs[1].prefab = Resources.Load<GameObject>("Page_2");
    }

    //private void Start()
    //{
    //    foreach (PagePrefabs prefab in prefabs)
    //    {
    //        // instantiate the prefab and store to the dictionary
    //        GameObject instantiated = Instantiate(prefab.prefab, Vector3.zero, Quaternion.identity);
    //        instantiated.name = prefab.name;
    //        spawnedPrefabs.Add(instantiated.name, instantiated);
    //        instantiated.SetActive(false);
    //    }
    //}


    //���߿� trackedImagesChanged()�Լ��� ARTrackedImageManager�� OnTrackablesChanged�Լ��� ���� ����
    //����Ƽ6������ trackedImagesChanged()�Լ��� �Ⱦ��� ARTrackedImageManager�� OnTrackablesChanged�Լ��� ����ϱ� ����  
    private void OnEnable()
    {
        imgManager.trackedImagesChanged += OnImageChanged;
    }

    private void OnDisable()
    {
        imgManager.trackedImagesChanged -= OnImageChanged;
    }   

    //���� ����
    //private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    //{
    //    // ���� �߰��� image
    //    foreach (var img in args.added)
    //    {
    //        UpdateSpawned(img);
    //        Debug.Log($"{img.name} �߰�");
    //    }

    //    // ���ŵ� image
    //    foreach (var img in args.updated)
    //    {
    //        UpdateSpawned(img);
    //    }

    //    // ���ŵ� image
    //    foreach (var img in args.removed)
    //    {
    //        // disable the prefab that has the same name than the image
    //        spawnedPrefabs[img.Value.name].SetActive(false);
    //    }
    //}


    /// <summary>
    /// �̹����� ���ŵǸ� �� ��� �������� �ҷ����� �Լ�
    /// </summary>
    /// <param name="args"></param>
    private void OnImageChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (ARTrackedImage img in args.added)
        {
            UpdateSpawned(img);
        }

        foreach (ARTrackedImage img in args.updated)
        {
            UpdateSpawned(img);
        }

        foreach (ARTrackedImage img in args.removed)
        {
            // disable the prefab that has the same name than the image
            spawnedPrefabs[img.referenceImage.name].SetActive(false);
        }
    }

    /// <summary>
    /// ��� ������ ��ü �Լ�
    /// </summary>
    /// <param name="img"></param>
    private void UpdateSpawned(ARTrackedImage img)
    {
        string name = img.referenceImage.name;
        GameObject spawned = spawnedPrefabs[name];

        // �̹��� ������ �ǰ��ִ� ������ �� ������ Ȱ��ȭ �� ��ġ ����
        if (img.trackingState == TrackingState.Tracking)
        {
            spawned.transform.position = img.transform.position;
            spawned.transform.rotation = img.transform.rotation;
            spawned.SetActive(true);
        }
        else
        {
            // �̹��� ������ �ȵǰ� �ִ� ������ �� ������ ��Ȱ��ȭ
            spawned.SetActive(false);
        }
    }
}