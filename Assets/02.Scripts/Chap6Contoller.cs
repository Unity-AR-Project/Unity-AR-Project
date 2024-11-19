using UnityEngine;
using UnityEngine.Playables;

public class Chap6Contoller : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector playableDirector;
    void Start()
    {
        playableDirector.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

