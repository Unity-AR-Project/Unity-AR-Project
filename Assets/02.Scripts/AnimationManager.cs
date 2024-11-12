using System.Collections;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;

    Animator animator;
    private bool isBroken = false; // 집이 부셔졌는지(날아갔는지) 여부
    private int blowCount = 0; // 늑대가 바람을 분 횟수
    private const int maxBlowCount = 3; // 최대 바람 횟수

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

        //if (바람 부는 거 인식 했을 때) 
        //{
        //    PlayBlowWind();
        //}
    }

    // 늑대가 바람을 부는 함수
    void PlayBlowWind()
    {
        if (isBroken) return; // 이미 집이 부셔졌다면 더 이상 진행하지 않음

        blowCount++;
        Debug.Log($"늑대가 바람을 불었습니다. 현재 바람 횟수: {blowCount}");

        // 바람 애니메이션
        animator.SetBool("IsBlow", true);
        // 바람 사운드 추가

        //첫째 집에서는 한번에 무너져야함
        if (blowCount == 1)
        {
            isBroken = true;
            animator.SetBool("IsBlow", false);
            StartCoroutine(PigCryAndRun());
        }
        //둘째 집에서 두번째에 무너짐.
        if (blowCount == 2)
        {
            isBroken = true;
            animator.SetBool("IsBlow", false);
            StartCoroutine(PigCryAndRun());
        }
        //셋째 집은 무너 지지 않음.
        if (blowCount == maxBlowCount)
        {
            isBroken = false;
        }
    }

    // 돼지가 울고 도망가는 코루틴
    IEnumerator PigCryAndRun()
    {
        if (isBroken)
        {
            // 우는 애니메이션
            animator.SetTrigger("IsCry");
            // 돼지 울음(슬퍼하는) 소리 추가 

            // 우는 애니메이션 길이만큼 대기
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

            // 돼지 도망가는(달리는) 애니메이션
            animator.SetTrigger("IsRun");
            // 도망가는 효과음 
        }
    }

    //늑대 구현해야할 애니메이션 & 효과음
    void WolfClips()
    {
        //웃고
        //늑대 웃는 소리 
        //떨어졌을 때 고통받는 애니메이션
        //떨어졌을 때 고통받는 사운드   
        //도망감
        //도망가는 효과음
    }
}