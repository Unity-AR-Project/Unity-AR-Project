using UnityEngine;

public class Chap1Controoler : MonoBehaviour
{
    public ParticleSystem smoke; //��ƼŬ �ý���(����);
    private bool isSmokePlaying = false; //���� ���� ��� ����

    void Start()
    {
        smoke.Play(); //���� ��ƼŬ ����
        isSmokePlaying = true; // ó���� ���

    }

    void Update()
    {
        // ��ƼŬ�� ó�� ����ǰ� 
        if (smoke.isEmitting && isSmokePlaying)
        {
            SoundManager.instance.PlaySFX("smoke");
            isSmokePlaying = false;
        }
    }
}
