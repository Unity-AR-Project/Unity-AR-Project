using UnityEngine;
using UnityEngine.Playables;

public class Chap4Controller : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector playableDirector;

    //private int blowCount = 1;  //바람분 횟수

    void Start()
    {   //바람 불러달라는 텍스티(UI) 하고 음성 
        //예시로 Start 넣어둠. 바람부는 거 인식이 가능하면 Update 넣어둔 것을 사용 할 것.
        playableDirector.Play();
    }


    void Update()
    {
        //if(바람부는 거 인식했을때)
        //{
        // twoBlowWind();
        //}
    }

    // 바람을 2번 불면 집이 날라가게 하는 함수.
    //private IEnumerator twoBlowWind()
    //{
    //    
    //    //처음에 재생(바람 불고)
    //    if(blowCount == 1){
    //    playableDirector.Play();
    //    //1.9초(바람 한번 부는 시간)동안 재생 
    //    yield return new WaitForSeconds(1.9f);
    //    blowCount = 2;
    //    }

    //    // 타임라인을 현재 시점에서 일시 정지
    //    playableDirector.Pause();
    //
    //    //if(blowCount = 2){
    //    // 바람을 다시 부는거 인식후에 다시 타임라인 재개
    //    playableDirector.Play();
    //    //}
    //}
}
