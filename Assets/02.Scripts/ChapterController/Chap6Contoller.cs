using UnityEngine;
using UnityEngine.Playables;

public class Chap6Contoller : MonoBehaviour, IChapterController
{
    [SerializeField]
    private PlayableDirector playableDirector;

    private bool isPaused = false; // 타임라인 멈춤 상태를 추적하는 플래그
    void Start()
    {
        playableDirector.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }


    /// <summary>
    /// 타임라인 일시정지/재개 토글
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

