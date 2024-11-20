using UnityEngine;

/// <summary>
/// 늑대 캐릭터의 애니메이션을 제어하는 클래스
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
            Debug.LogError("Animator가 설정되지 않았습니다.");
        }
    }

    /// <summary>
    /// 늑대가 날아가는 애니메이션을 실행합니다.
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
