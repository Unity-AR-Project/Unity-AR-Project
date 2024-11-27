using UnityEngine;
using UnityEngine.Playables;

public class Chap6Contoller : MonoBehaviour, IChapterController
{
    [SerializeField]
    private PlayableDirector playableDirector;

    private bool isPaused = false; // Ÿ�Ӷ��� ���� ���¸� �����ϴ� �÷���
    /*                               //������ �ʱ�ȭ
    [SerializeField] private GameObject chapter6Prefab; // é�� 7 ������
    [SerializeField] private Transform prefabParent; // �������� �ν��Ͻ�ȭ�� �θ� ������Ʈ
    private GameObject chapter6Instance; // ���� Ȱ��ȭ�� é�� 7 �ν��Ͻ�*/

    void OnEnable()
    {
        /*if (chapter6Instance != null)
        {
            Destroy(chapter6Instance);
        }

        // é�� 7 ������ �ν��Ͻ�ȭ
        if (chapter6Prefab != null && prefabParent != null)
        {
            chapter6Instance = Instantiate(chapter6Prefab, prefabParent);
            chapter6Instance.tag = "Chapter1Instance"; // �ʿ� �� �±� ����
            chapter6Instance.SetActive(true);
            Debug.Log("[chap1Controller] Chapter1 prefab instantiated.");
        }
        else
        {
            Debug.LogError("[chap6Controller] Chapter6Prefab or PrefabParent is not assigned.");
        }*/

        // Ÿ�Ӷ��� �ʱ� ����: ������� �ʰ� ��� ���·� ����
      /*  if (playableDirector != null)
        {

*/
            // Ÿ�Ӷ��� ����
            playableDirector.time = 0; // Ÿ�Ӷ��� �ð� �ʱ�ȭ
            playableDirector.Stop();   // Ÿ�Ӷ��� ����
            playableDirector.Play();   // Ÿ�Ӷ��� ���
       /* }
        else
        {
            Debug.LogError("[chap6Controller] PlayableDirector not assigned.");
        }*/
    }


    void Start()
    {
       // playableDirector.Play();
    }

    // Update is called once per frame
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

