using UnityEngine;
using UnityEngine.Playables;

public class Chap7Controller : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector playableDirector;

    //private int bolwCount = 1;

    void Start()
    {   //�ٶ� �ҷ��޶�� �ؽ�Ʈ(UI) �ϰ� ����
        //���÷� Start �־��. �ٶ��δ� �� �ν��� �����ϸ� Update �־�� ���� ��� �� ��.
        playableDirector.Play();
    }


    void Update()
    {
        //if(�ٶ��δ� �� �ν�������)
        //{
        // threeBlowWind();
        //}
    }

    // �ٶ��� 2�� �Ҹ� ���� ���󰡰� �ϴ� �Լ�.
    //private IEnumerator threeBlowWind()
    //{
    //    //ó���� ���(�ٶ� �Ұ�)
    //    if(blowCount == 1)
    //    {
    //    playableDirector.Play();
    //    //1��(ex�ٶ� �ѹ� �δ� �ð�)���� ��� 
    //    yield return new WaitForSeconds(1.1f);
    //     Ÿ�Ӷ����� ���� �������� �Ͻ� ����
    //    playableDirector.Pause();
    //    bolwCount = 2;
    //    }
   
    //    �ι��� �ٶ�
    //    //if(blowCount == 2){
    //    // �ٶ��� �ٽ� �δ°� �ν��Ŀ� �ٽ� Ÿ�Ӷ��� �簳
    //    playableDirector.Play();
    //   
    //    yield return new WaitForSeconds(2.0f);
    //   playableDirector.Pause();
    //    blowCount = 3;
    //    }
    //    ������ �ٶ�
    //    //if(Blowcount == 3)
    //{
    //      playableDirector.Play();
    //}
}
