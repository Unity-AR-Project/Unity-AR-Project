using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class Chap4Controller : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector playableDirector;

    //private int blowCount = 1;  //�ٶ��� Ƚ��

    void Start()
    {   //�ٶ� �ҷ��޶�� �ؽ�Ƽ(UI) �ϰ� ���� 
        //���÷� Start �־��. �ٶ��δ� �� �ν��� �����ϸ� Update �־�� ���� ��� �� ��.
;
    }


    void Update()
    {
       
        //if(�ٶ��δ� �� �ν�������)
        //{
        // twoBlowWind();
        //}
    }

    // �ٶ��� 2�� �Ҹ� ���� ���󰡰� �ϴ� �Լ�.
    //private IEnumerator twoBlowWind()
    //{
    //    
    //    //ó���� ���(�ٶ� �Ұ�)
    //    if(blowCount == 1){
    //    playableDirector.Play();
    //    //2��(�ٶ� �ѹ� �δ� �ð�)���� ��� 
    //    yield return new WaitForSeconds(1.9f);
    //    blowCount = 2;
    //    }

    //    // Ÿ�Ӷ����� ���� �������� �Ͻ� ����
    //    playableDirector.Pause();
    //
    //    //if(blowCount = 2){
    //    // �ٶ��� �ٽ� �δ°� �ν��Ŀ� �ٽ� Ÿ�Ӷ��� �簳
    //    playableDirector.Play();
    //    //}
    //}
    //private IEnumerator Test() 
    //{
    //    //�ٶ��� 2�� �Ҹ� ���� ���󰡰� �ϴ� �Լ�.

    //    { 
    //        //ó���� ���(�ٶ� �Ұ�)
    //        if (blowCount == 1)
    //        {
    //            playableDirector.Play();
    //            //2��(�ٶ� �ѹ� �δ� �ð�)���� ��� 
    //            yield return new WaitForSeconds(1.9f);
    //            blowCount = 2;
    //        }

    //        // Ÿ�Ӷ����� ���� �������� �Ͻ� ����
    //        playableDirector.Pause();

    //        if(blowCount == 2){
    //        // �ٶ��� �ٽ� �δ°� �ν��Ŀ� �ٽ� Ÿ�Ӷ��� �簳
    //        playableDirector.Play();
    //        }
    //    }
    //}
    //private IEnumerator waitStart() 
    //{
    //    playableDirector.Play();
    //    yield return new WaitForSeconds(10.2333f);
    //}
}
