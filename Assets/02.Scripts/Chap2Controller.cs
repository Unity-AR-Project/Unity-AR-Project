using UnityEngine;

public class Chap2Controller : MonoBehaviour
{

    void Start()
    {
        //�ٶ��� �ҷ��޶�� UI(�ؽ�Ʈ) �ʿ�. 
        AnimationManager.instance.BlowWind();  //�׽�Ʈ�� ����(�ٶ��δ� �� �ν� �������� �� ����)
    }


    void Update()
    {
        //if (�ٶ� �δ� �� �ν����� ��) 
        //{
        //    AnimationManager.instance.PlayBlowWind();
        //}
        if(AnimationManager.instance.isPigRun == true) 
        {
            AnimationManager.instance.WolfRun();
        }
    }
}
