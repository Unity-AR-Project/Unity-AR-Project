using UnityEngine;
using UnityEngine.Playables;

public class Chap6Contoller : MonoBehaviour, IChapterController
{
    [SerializeField]
    private PlayableDirector playableDirector;

    private bool isPaused = false; // 타임라인 멈춤 상태를 추적하는 플래그
    /*                               //프리팹 초기화
    [SerializeField] private GameObject chapter6Prefab; // 챕터 7 프리팹
    [SerializeField] private Transform prefabParent; // 프리팹을 인스턴스화할 부모 오브젝트
    private GameObject chapter6Instance; // 현재 활성화된 챕터 7 인스턴스*/

    void OnEnable()
    {
        /*if (chapter6Instance != null)
        {
            Destroy(chapter6Instance);
        }

        // 챕터 7 프리팹 인스턴스화
        if (chapter6Prefab != null && prefabParent != null)
        {
            chapter6Instance = Instantiate(chapter6Prefab, prefabParent);
            chapter6Instance.tag = "Chapter1Instance"; // 필요 시 태그 설정
            chapter6Instance.SetActive(true);
            Debug.Log("[chap1Controller] Chapter1 prefab instantiated.");
        }
        else
        {
            Debug.LogError("[chap6Controller] Chapter6Prefab or PrefabParent is not assigned.");
        }*/

        // 타임라인 초기 설정: 재생하지 않고 대기 상태로 설정
      /*  if (playableDirector != null)
        {

*/
            // 타임라인 시작
            playableDirector.time = 0; // 타임라인 시간 초기화
            playableDirector.Stop();   // 타임라인 정지
            playableDirector.Play();   // 타임라인 재생
       /* }
        else
        {
            Debug.LogError("[chap6Controller] PlayableDirector not assigned.");
        }*/
    }


    void Start()
    {
       // playableDirector.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }


    /// <summary>
    /// 타임라인 일시정지/재개 토글
    /// </summary>
    public void TogglePause()
    {
        if (isPaused)
        {
            playableDirector.Play();
            isPaused = false;
        }
        else
        {
            playableDirector.Pause();
            isPaused = true;
        }
    }
}

