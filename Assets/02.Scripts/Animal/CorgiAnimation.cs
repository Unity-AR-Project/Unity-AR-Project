using UnityEngine;

/// <summary>
/// ĳ������ �ִϸ��̼��� �����ϴ� Ŭ����
/// </summary>
public class CorgiAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        // Animator ������Ʈ ��������
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("Animator ������Ʈ�� CorgiAnimation ���� ������Ʈ�� �����ϴ�.");
        }
    }

    private void Start()
    {
        TriggerRunAnimation();
    }

    private void OnEnable()
    {
        TriggerRunAnimation();
    }

    /// <summary>
    /// 'Run' �ִϸ��̼��� Ʈ�����ϴ� �޼���
    /// </summary>
    private void TriggerRunAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Run");
            Debug.Log("Run �ִϸ��̼� Ʈ���� ����");
        }
        else
        {
            Debug.LogWarning("Animator ������Ʈ�� CorgiAnimation ���� ������Ʈ�� �����ϴ�.");
        }
    }
}
