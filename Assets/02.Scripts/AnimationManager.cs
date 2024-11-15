using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;

    [Header("Animator")]
    public Animator pigAnimator; // 돼지 애니메이터
    public Animator houseAnimator; // 집 애니메이터
    public Animator wolfAnimator; // 늑대 애니메이터

    public ParticleSystem wind; //파티클 시스템(바람)
    public  bool isBlowAndBreak = false; // 집이 부셔졌는지(날아갔는지) 여부
    public bool isPigRun = false; //돼지가 도망가는지 여부

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
        //애니메이터가 들어있는지 확인
        if (pigAnimator == null)
        {
            Debug.LogError("Pig Animator가 연결되지 않았습니다.");
        }
        if (houseAnimator == null)
        {
            Debug.LogError("House Animator가 연결되지 않았습니다.");
        }
        if (wolfAnimator == null)
        {
            Debug.LogError("Wolf Animator가 연결되지 않았습니다.");
        }

        wind.Stop();
    }

    private void Update()
    {

        //if (바람 부는 거 인식 했을 때) 
        //{
        //    BlowWind();
        //}
        
    }

    // 늑대가 바람을 부는 함수
    public void BlowWind()
    {
        wind.Play();
        SoundManager.instance.PlaySFX("blowWind");
    }

    // 돼지가 울고 도망가는 코루틴
    public IEnumerator PigCryAndRun()
    {
        //if (isBlowAndBreak == true)
        //{
            // 우는 애니메이션
            pigAnimator.SetTrigger("IsCry");
            // 돼지 울음(슬퍼하는) 소리 추가 

            // 우는 애니메이션 길이만큼 대기
            yield return new WaitForSeconds(pigAnimator.GetCurrentAnimatorStateInfo(0).length);

            // 돼지 도망가는(달리는) 애니메이션
            pigAnimator.SetTrigger("IsRun");
            isPigRun = true;
            // 도망가는 효과음 
        //}
    }
    //첫째 집이 날라갈때 애니메이션 & 효과음 실행
    public void BlowHouse() 
    {
            houseAnimator.SetTrigger("BlowHousew");
            SoundManager.instance.PlaySFX("blowHouse");
        
    }

    //늑대 돼지들 쫓아가고 도망가는 함수 
    public void WolfRun()
    { 
            wolfAnimator.SetTrigger("WolfRun");

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