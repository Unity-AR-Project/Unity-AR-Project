using UnityEngine;
using UnityEngine.Playables;

public class Chap2Controller : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector playableDirector; //Ÿ�� �������� ���� PlayableDirector
    void Start()
    {
        //�ٶ��� �ҷ��޶�� UI(�ؽ�Ʈ) �ʿ�. 

        //if (playableDirector != null)
        //{
        //    playableDirector.Stop(); // �ʱ� ���¿����� ����

        //}
        // �ٶ� �Է¹޴� �� �Ǹ� �ּ� �����ϰ� Update�� ��� 
        //�׽�Ʈ�� ���� �ٷ� ���� 
        playableDirector.Play();
    }


    void Update()
    {
        //if(�ٶ� �δ°� �Է¹�����)
        //playableDirector.Play();
    }
}
