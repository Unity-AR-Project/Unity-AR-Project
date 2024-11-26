using UnityEngine;

/// <summary>
/// ���� ĳ������ �ִϸ��̼��� �����ϴ� Ŭ����
/// </summary>
public class WolfController : MonoBehaviour
{
    private Animator animator;
    private readonly int flyTriggerHash = Animator.StringToHash("WolfRun");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator�� �������� �ʾҽ��ϴ�.");
        }
    }

    /// <summary>
    /// ���밡 ���ư��� �ִϸ��̼��� �����մϴ�.
    /// </summary>
    public void FlyAway()
    {
        if (animator != null)
        {
            animator.SetTrigger(flyTriggerHash);
            Debug.Log("WolfRun !");
        }
    }
}
