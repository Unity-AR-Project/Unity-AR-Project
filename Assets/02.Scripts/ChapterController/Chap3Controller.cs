using UnityEngine;
using UnityEngine.Playables;

public class Chap3Controller : MonoBehaviour, IChapterController
{
    [SerializeField]
    private PlayableDirector playableDirector;

    private bool isPaused = false; // �Ͻ����� ���� ����

    /*//������ �ʱ�ȭ
    [SerializeField] private GameObject chapter3Prefab; // é�� 7 ������
    [SerializeField] private Transform prefabParent; // �������� �ν��Ͻ�ȭ�� �θ� ������Ʈ
    private GameObject chapter3Instance; // ���� Ȱ��ȭ�� é�� 7 �ν��Ͻ�
*/
    void OnEnable()
    {
        /*
        if (chapter3Instance != null)
        {
            Destroy(chapter3Instance);
        }

        // é�� 3 ������ �ν��Ͻ�ȭ
        if (chapter3Prefab != null && prefabParent != null)
        {
            chapter3Instance = Instantiate(chapter3Prefab, prefabParent);
            chapter3Instance.tag = "Chapter3Instance"; // �ʿ� �� �±� ����
            chapter3Instance.SetActive(true);
            Debug.Log("[chap3Controller] Chapter3 prefab instantiated.");
        }
        else
        {
            Debug.LogError("[chap3Controller] Chapter1Prefab or PrefabParent is not assigned.");
        }

        // Ÿ�Ӷ��� �ʱ� ����: ������� �ʰ� ��� ���·� ����
        if (playableDirector != null)
        {
*/

        // Ÿ�Ӷ��� ����
        playableDirector.time = 0; // Ÿ�Ӷ��� �ð� �ʱ�ȭ
        playableDirector.Stop();   // Ÿ�Ӷ��� ����
                                   // �ʱ� �޽��� ǥ��
        UIManager.instance.ShowMessage("3é�� ���۵Ǿ����ϴ�.\n " +
            "��ٷ��ּ���!");
        playableDirector.Play();   // Ÿ�Ӷ��� ���
        /*  }
          else
          {
              Debug.LogError("[chap3Controller] PlayableDirector not assigned.");
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
