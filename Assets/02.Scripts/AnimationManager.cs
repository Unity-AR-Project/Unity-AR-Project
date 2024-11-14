using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;

    [Header("Animator")]
    public Animator pigAnimator; // ���� �ִϸ�����
    public Animator houseAnimator; // �� �ִϸ�����
    public Animator wolfAnimator; // ���� �ִϸ�����

    public ParticleSystem wind; //��ƼŬ �ý���(�ٶ�)
    public  bool isBroken = false; // ���� �μ�������(���ư�����) ����
    private int blowCount = 0; // ���밡 �ٶ��� �� Ƚ��
    private const int maxBlowCount = 3; // �ִ� �ٶ� Ƚ��
    public bool isPigRun = false; //������ ���������� ����

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
        wind.Stop();
    }

    private void Update()
    {

        //if (�ٶ� �δ� �� �ν� ���� ��) 
        //{
        //    BlowWind();
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
            Invoke("wind.Stop", 2.0f);
            // �ٶ� ���� �߰�
            StartCoroutine(PigCryAndRun());
        }
        //��° ������ �ι�°�� ������.
        if (blowCount == 2)
        {
            isBroken = true;
            wind.Play();
            Invoke("wind.Stop", 2.0f);
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
            pigAnimator.SetTrigger("IsCry");
            // ���� ����(�����ϴ�) �Ҹ� �߰� 

            // ��� �ִϸ��̼� ���̸�ŭ ���
            yield return new WaitForSeconds(pigAnimator.GetCurrentAnimatorStateInfo(0).length);

            // ���� ��������(�޸���) �ִϸ��̼�
            pigAnimator.SetTrigger("IsRun");
            isPigRun = true;
            // �������� ȿ���� 
        }
    }

    //���� ������ �Ѿư��� �������� �Լ� 
    public void WolfRun()
    {
        if (isPigRun) 
        {
            wolfAnimator.SetTrigger("WolfRun");
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