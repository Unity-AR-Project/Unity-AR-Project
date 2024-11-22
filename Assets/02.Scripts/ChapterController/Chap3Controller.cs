using UnityEngine;
using UnityEngine.Playables;

public class Chap3Controller : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector playableDirector;

    void Start()
    {
        playableDirector.Play(); 
    }


    void Update()
    {

    }
}
