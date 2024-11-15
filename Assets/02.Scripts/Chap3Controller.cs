using UnityEngine;

public class Chap3Controller : MonoBehaviour
{
    public PlayableDirector playableDirector; //타임 라인제어 위한 PlayableDirector

    void Start()
    {
        playableDirector.Play(); 
    }


    void Update()
    {
        
    }
}
