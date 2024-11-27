using UnityEngine;
using UnityEngine.Playables;

public class Chap1Controller : MonoBehaviour, IChapterController
{
    [SerializeField]
    private PlayableDirector playableDirector;

    private bool isPaused = false; // �Ͻ����� ���� ����
    
    /*//������ �ʱ�ȭ
    [SerializeField] private GameObject chapter1Prefab; // é�� 7 ������
    [SerializeField] private Transform prefabParent; // �������� �ν��Ͻ�ȭ�� �θ� ������Ʈ
    private GameObject chapter1Instance; // ���� Ȱ��ȭ�� é�� 7 �ν��Ͻ�*/

    void OnEnable()
    {
        /*if (chapter1Instance != null)
        {
            Destroy(chapter1Instance);
        }

        // é�� 1 ������ �ν��Ͻ�ȭ
        if (chapter1Prefab != null && prefabParent != null)
        {
            chapter1Instance = Instantiate(chapter1Prefab, prefabParent);
            chapter1Instance.tag = "Chapter1Instance"; // �ʿ� �� �±� ����
            chapter1Instance.SetActive(true);
            Debug.Log("[chap1Controller] Chapter1 prefab instantiated.");
        }
        else
        {
            Debug.LogError("[chap1Controller] Chapter1Prefab or PrefabParent is not assigned.");
        }

        // Ÿ�Ӷ��� �ʱ� ����: ������� �ʰ� ��� ���·� ����
        if (playableDirector != null)
        {*/
            

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
