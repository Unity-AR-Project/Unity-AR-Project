using UnityEngine;

public class Chap1Controller : MonoBehaviour
{
    public ParticleSystem smoke; //파티클 시스템(연기);
    private bool isSmokePlaying = false; //소리가 이미 실행 되었는지 확인 하는 변수

    void Start()
    {
        smoke.Play(); //연기 파티클 실행

    }

    void Update()
    {
        // 파티클이 방출 중이고 소리가 아직 실행되지 않았다면
        if (smoke.isEmitting && isSmokePlaying == false)
        {
            SoundManager.instance.PlaySFX("smoke");
            isSmokePlaying = true;  // 소리 실행 후 true로 설정
        }

    }
}
