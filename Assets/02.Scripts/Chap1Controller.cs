using UnityEngine;

public class Chap1Controller : MonoBehaviour
{
    public ParticleSystem smoke; //��ƼŬ �ý���(����);
    private bool isSmokePlaying = false; //�Ҹ��� �̹� ���� �Ǿ����� Ȯ�� �ϴ� ����

    void Start()
    {
        smoke.Play(); //���� ��ƼŬ ����

    }

    void Update()
    {
        // ��ƼŬ�� ���� ���̰� �Ҹ��� ���� ������� �ʾҴٸ�
        if (smoke.isEmitting && isSmokePlaying == false)
        {
            SoundManager.instance.PlaySFX("smoke");
            isSmokePlaying = true;  // �Ҹ� ���� �� true�� ����
        }

    }
}
