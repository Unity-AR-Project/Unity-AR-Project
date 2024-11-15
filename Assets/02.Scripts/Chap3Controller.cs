using UnityEngine;
using UnityEngine.Playables;

public class Chap3Controller : MonoBehaviour
{
    public PlayableDirector playableDirector;

    void Start()
    {
        playableDirector.Play(); 
    }


    void Update()
    {
        
    }
}
