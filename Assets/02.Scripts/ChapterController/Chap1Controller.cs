using UnityEngine;
using UnityEngine.Playables;

public class Chap1Controller : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector playableDirector;

    private bool isPaused = false; // 일시정지 상태 여부

    void Start()
    {
        playableDirector.Play();

    }

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
