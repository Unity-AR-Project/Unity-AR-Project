using UnityEngine;

public class CorgiAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
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
