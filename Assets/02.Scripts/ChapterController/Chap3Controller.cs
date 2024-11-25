using UnityEngine;
using UnityEngine.Playables;

public class Chap3Controller : MonoBehaviour, IChapterController
{
    [SerializeField]
    private PlayableDirector playableDirector;
    
    private bool isPaused = false; // �Ͻ����� ���� ����

    void OnEnable()
    {
        // Ÿ�Ӷ��� ����
        playableDirector.time = 0; // Ÿ�Ӷ��� �ð� �ʱ�ȭ
        playableDirector.Stop();   // Ÿ�Ӷ��� ����
        playableDirector.Play();   // Ÿ�Ӷ��� ���
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
