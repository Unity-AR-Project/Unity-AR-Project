using UnityEngine;
using UnityEngine.Playables;

public class Chap1Controller : MonoBehaviour
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
