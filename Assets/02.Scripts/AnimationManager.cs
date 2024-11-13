using System.Collections;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;

    Animator animator;
    public ParticleSystem wind; //��ƼŬ �ý���(�ٶ�)
    public bool isBroken = false; // ���� �μ�������(���ư�����) ����
    private int blowCount = 0; // ���밡 �ٶ��� �� Ƚ��
    private const int maxBlowCount = 3; // �ִ� �ٶ� Ƚ��

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {

        //if (�ٶ� �δ� �� �ν� ���� ��) 
        //{
        //    PlayBlowWind();
        //}
        
    }

    // ���밡 �ٶ��� �δ� �Լ�
    public void BlowWind()
    {
        if (isBroken) return; // �̹� ���� �μ����ٸ� �� �̻� �������� ����

        blowCount++;
        Debug.Log($"���밡 �ٶ��� �Ҿ����ϴ�. ���� �ٶ� Ƚ��: {blowCount}");


        //ù° �������� �ѹ��� ����������
        if (blowCount == 1)
        {
            isBroken = true;
            wind.Play();
            animator.SetBool("IsBroken", isBroken);
            // �ٶ� ���� �߰�
            StartCoroutine(PigCryAndRun());
        }
        //��° ������ �ι�°�� ������.
        if (blowCount == 2)
        {
            isBroken = true;
            wind.Play();
            animator.SetBool("IsBroken", isBroken);
            // �ٶ� ���� �߰�
            StartCoroutine(PigCryAndRun());
        }
        //��° ���� ���� ���� ����.
        if (blowCount == maxBlowCount)
        {
            isBroken = false;
        }
    }

    // ������ ��� �������� �ڷ�ƾ
    public IEnumerator PigCryAndRun()
    {
        if (isBroken)
        {
            // ��� �ִϸ��̼�
            animator.SetTrigger("IsCry");
            // ���� ����(�����ϴ�) �Ҹ� �߰� 

            // ��� �ִϸ��̼� ���̸�ŭ ���
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

            // ���� ��������(�޸���) �ִϸ��̼� 
            animator.SetTrigger("IsRun");
            // �������� ȿ���� 
        }
    }

    //���� �����ؾ��� �ִϸ��̼� & ȿ����
    void WolfClips()
    {
        //����
        //���� ���� �Ҹ� 
        //�������� �� ����޴� �ִϸ��̼�
        //�������� �� ����޴� ����   
        //������
        //�������� ȿ����
    }
}