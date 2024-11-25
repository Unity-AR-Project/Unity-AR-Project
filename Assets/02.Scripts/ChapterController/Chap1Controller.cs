using UnityEngine;
using UnityEngine.Playables;

public class Chap1Controller : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector playableDirector;

    private bool isPaused = false; // �Ͻ����� ���� ����

    void Start()
    {
        playableDirector.Play();

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
