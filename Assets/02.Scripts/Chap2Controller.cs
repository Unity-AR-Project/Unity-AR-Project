using UnityEngine;
using UnityEngine.Playables;

public class Chap2Controller : MonoBehaviour
{
    public PlayableDirector playableDirector; //Ÿ�� �������� ���� PlayableDirector
    void Start()
    {
        //�ٶ��� �ҷ��޶�� UI(�ؽ�Ʈ) �ʿ�. 

        //�ڵ����� ���� X <= �ٶ� �Է¹޴� �� �Ǹ� �ּ� �����ϰ� ��� 
        //if (playableDirector != null)
        //{
        //    playableDirector.Stop(); // �ʱ� ���¿����� ����

        //}
        //�׽�Ʈ�� ���� �ٷ� ���� 
        playableDirector.Play();
    }


    void Update()
    {
        //if(�ٶ� �δ°� �Է¹�����)
        //playableDirector.Play();
    }
}
