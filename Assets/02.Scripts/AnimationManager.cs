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
    public  bool isBlowAndBreak = false; // ���� �μ�������(���ư�����) ����
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
        //�ִϸ����Ͱ� ����ִ��� Ȯ��
        if (pigAnimator == null)
        {
            Debug.LogError("Pig Animator�� ������� �ʾҽ��ϴ�.");
        }
        if (houseAnimator == null)
        {
            Debug.LogError("House Animator�� ������� �ʾҽ��ϴ�.");
        }
        if (wolfAnimator == null)
        {
            Debug.LogError("Wolf Animator�� ������� �ʾҽ��ϴ�.");
        }

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
        wind.Play();
        SoundManager.instance.PlaySFX("blowWind");
    }

    // ������ ��� �������� �ڷ�ƾ
    public IEnumerator PigCryAndRun()
    {
        //if (isBlowAndBreak == true)
        //{
            // ��� �ִϸ��̼�
            pigAnimator.SetTrigger("IsCry");
            // ���� ����(�����ϴ�) �Ҹ� �߰� 

            // ��� �ִϸ��̼� ���̸�ŭ ���
            yield return new WaitForSeconds(pigAnimator.GetCurrentAnimatorStateInfo(0).length);

            // ���� ��������(�޸���) �ִϸ��̼�
            pigAnimator.SetTrigger("IsRun");
            isPigRun = true;
            // �������� ȿ���� 
        //}
    }
    //ù° ���� ���󰥶� �ִϸ��̼� & ȿ���� ����
    public void BlowHouse() 
    {
            houseAnimator.SetTrigger("BlowHousew");
            SoundManager.instance.PlaySFX("blowHouse");
        
    }

    //���� ������ �Ѿư��� �������� �Լ� 
    public void WolfRun()
    { 
            wolfAnimator.SetTrigger("WolfRun");

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