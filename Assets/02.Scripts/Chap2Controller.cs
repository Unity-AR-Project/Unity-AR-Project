using UnityEngine;

public class Chap2Controller : MonoBehaviour
{

    void Start()
    {
        AnimationManager.instance.BlowWind();  //테스트용 실행(바람부는 거 인식 가능했을 때 삭제)
    }


    void Update()
    {
        //if (바람 부는 거 인식했을 때) 
        //{
        //    AnimationManager.instance.PlayBlowWind();
        //}

        if (AnimationManager.instance.isBroken == true)
        {
            AnimationManager.instance.PigCryAndRun();
        }
    }
}
