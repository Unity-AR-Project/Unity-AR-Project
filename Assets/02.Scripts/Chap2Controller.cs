using UnityEngine;
using UnityEngine.Playables;

public class Chap2Controller : MonoBehaviour
{
    public PlayableDirector playableDirector; //타임 라인제어 위한 PlayableDirector
    void Start()
    {
        //바람을 불러달라는 UI(텍스트) 필요. 

        //자동으로 시작 X <= 바람 입력받는 것 되면 주석 삭제하고 사용 
        //if (playableDirector != null)
        //{
        //    playableDirector.Stop(); // 초기 상태에서는 정지

        //}
        //테스트를 위해 바로 실행 
        playableDirector.Play();
    }


    void Update()
    {
        //if(바람 부는거 입력받으면)
        //playableDirector.Play();
    }
}
