using UnityEngine;

public class Chap2Controller : MonoBehaviour
{

    void Start()
    {
        AnimationManager.instance.BlowWind();  //�׽�Ʈ�� ����(�ٶ��δ� �� �ν� �������� �� ����)
    }


    void Update()
    {
        //if (�ٶ� �δ� �� �ν����� ��) 
        //{
        //    AnimationManager.instance.PlayBlowWind();
        //}

        if (AnimationManager.instance.isBroken == true)
        {
            AnimationManager.instance.PigCryAndRun();
        }
    }
}
