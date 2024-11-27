using UnityEngine;
using UnityEngine.Playables;

public class Chap5Contoller : MonoBehaviour, IChapterController
{
    [SerializeField]
    private PlayableDirector playableDirector;

    private bool isPaused = false; // �Ͻ����� ���� ����

   /* //������ �ʱ�ȭ
    [SerializeField] private GameObject chapter5Prefab; // é�� 7 ������
    [SerializeField] private Transform prefabParent; // �������� �ν��Ͻ�ȭ�� �θ� ������Ʈ
    private GameObject chapter5Instance; // ���� Ȱ��ȭ�� é�� 7 �ν��Ͻ�
*/
    void OnEnable()
    {/*
        if (chapter5Instance != null)
        {
            Destroy(chapter5Instance);
        }

        // é�� 7 ������ �ν��Ͻ�ȭ
        if (chapter5Prefab != null && prefabParent != null)
        {
            chapter5Instance = Instantiate(chapter5Prefab, prefabParent);
            chapter5Instance.tag = "Chapter5Instance"; // �ʿ� �� �±� ����
            chapter5Instance.SetActive(true);
            Debug.Log("[chap5Controller] Chapter5 prefab instantiated.");
        }
        else
        {
            Debug.LogError("[chap5Controller] Chapter5Prefab or PrefabParent is not assigned.");
        }

        // Ÿ�Ӷ��� �ʱ� ����: ������� �ʰ� ��� ���·� ����
        if (playableDirector != null)
        {
*/

            // Ÿ�Ӷ��� ����
            playableDirector.time = 0; // Ÿ�Ӷ��� �ð� �ʱ�ȭ
            playableDirector.Stop();   // Ÿ�Ӷ��� ����
            playableDirector.Play();   // Ÿ�Ӷ��� ���
      /*  }
        else
        {
            Debug.LogError("[chap1Controller] PlayableDirector not assigned.");
        }*/
    }


    void Start()
    {
       // playableDirector.Play();
    }

    void Update()
    {
        
    }
    /// <summary>
    /// Ÿ�Ӷ��� �Ͻ�����/�簳 ���
    /// </summary>
    public void TogglePause()
    {
        if (isPaused)
        {
            playableDirector.Play();
            isPaused = false;
        }
        else
        {
            playableDirector.Pause();
            isPaused = true;
        }
    }
}
