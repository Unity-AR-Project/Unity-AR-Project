using UnityEngine;

public class CorgiAnimation : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
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
