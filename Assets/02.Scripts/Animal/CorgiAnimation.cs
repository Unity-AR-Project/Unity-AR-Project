using UnityEngine;

/// <summary>
/// 캐릭터의 애니메이션을 제어하는 클래스
/// </summary>
public class CorgiAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        // Animator 컴포넌트 가져오기
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("Animator 컴포넌트가 CorgiAnimation 게임 오브젝트에 없습니다.");
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
    /// 'Run' 애니메이션을 트리거하는 메서드
    /// </summary>
    private void TriggerRunAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Run");
            Debug.Log("Run 애니메이션 트리거 설정");
        }
        else
        {
            Debug.LogWarning("Animator 컴포넌트가 CorgiAnimation 게임 오브젝트에 없습니다.");
        }
    }
}
