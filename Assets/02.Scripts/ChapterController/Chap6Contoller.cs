using UnityEngine;
using UnityEngine.Playables;

public class Chap6Contoller : MonoBehaviour, IChapterController
{
    [SerializeField]
    private PlayableDirector playableDirector;

    private bool isPaused = false; // Ÿ�Ӷ��� ���� ���¸� �����ϴ� �÷���
    void Start()
    {
        playableDirector.Play();
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

