using UnityEngine;
using UnityEngine.Playables;

public class Chap5Contoller : MonoBehaviour
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
