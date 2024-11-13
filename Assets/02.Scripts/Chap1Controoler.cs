using UnityEngine;

public class Chap1Controoler : MonoBehaviour
{
    public ParticleSystem smoke; //파티클 시스템(연기);
    private bool isSmokePlaying = false; //연기 사운드 재생 여부

    void Start()
    {
        smoke.Play(); //연기 파티클 실행
        isSmokePlaying = true; // 처음엔 재생

    }

    void Update()
    {
        // 파티클이 처음 재생되고 
        if (smoke.isEmitting && isSmokePlaying)
        {
            SoundManager.instance.PlaySFX("smoke");
            isSmokePlaying = false;
        }
    }
}
