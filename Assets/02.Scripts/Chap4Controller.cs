using UnityEngine;
using UnityEngine.Playables;

public class Chap4Controller : MonoBehaviour
{
    private PlayableDirector playableDirector;

    void Start()
    {   //�ٶ� �ҷ��޶�� �ؽ�Ƽ(UI) �ϰ� ���� 
        //���÷� Start �־��. �ٶ��δ� �� �ν��� �����ϸ� Update �־�� ���� ��� �� ��.
        playableDirector.Play();
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
    //    //ó���� ���(�ٶ� �Ұ�)
    //    playableDirector.Play();
    //    //2��(�ٶ� �ѹ� �δ� �ð�)���� ��� 
    //    yield return new WaitForSeconds(2.0f);

    //    // Ÿ�Ӷ����� ���� �������� �Ͻ� ����
    //    playableDirector.Pause();

    //    //if(�ٶ��δ� ���� �ν�������){
    //    // �ٶ��� �ٽ� �δ°� �ν��Ŀ� �ٽ� Ÿ�Ӷ��� �簳
    //    playableDirector.Play();
    //    //}
    //}
}
