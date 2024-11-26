using UnityEngine;
using UnityEngine.Playables;

public class Chap3Controller : MonoBehaviour, IChapterController
{
    [SerializeField]
    private PlayableDirector playableDirector;
    
    private bool isPaused = false; // 일시정지 상태 여부

    void OnEnable()
    {
        // 타임라인 시작
        playableDirector.time = 0; // 타임라인 시간 초기화
        playableDirector.Stop();   // 타임라인 정지
        playableDirector.Play();   // 타임라인 재생
    }

    void Start()
    {
       // playableDirector.Play(); 
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
