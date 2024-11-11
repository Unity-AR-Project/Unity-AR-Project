using UnityEngine;

public class CorgiAnimation : MonoBehaviour
{
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("Run");
    }
    private void OnEnable()
    {
        animator.SetTrigger("Run");
    }
}
