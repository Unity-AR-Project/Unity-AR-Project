using UnityEngine;
using UnityEngine.Playables;

public class Chap1Controller : MonoBehaviour, IChapterController
{
    [SerializeField]
    private PlayableDirector playableDirector;

    private bool isPaused = false; // 일시정지 상태 여부
    
    /*//프리팹 초기화
    [SerializeField] private GameObject chapter1Prefab; // 챕터 7 프리팹
    [SerializeField] private Transform prefabParent; // 프리팹을 인스턴스화할 부모 오브젝트
    private GameObject chapter1Instance; // 현재 활성화된 챕터 7 인스턴스*/

    void OnEnable()
    {
        /*if (chapter1Instance != null)
        {
            Destroy(chapter1Instance);
        }

        // 챕터 1 프리팹 인스턴스화
        if (chapter1Prefab != null && prefabParent != null)
        {
            chapter1Instance = Instantiate(chapter1Prefab, prefabParent);
            chapter1Instance.tag = "Chapter1Instance"; // 필요 시 태그 설정
            chapter1Instance.SetActive(true);
            Debug.Log("[chap1Controller] Chapter1 prefab instantiated.");
        }
        else
        {
            Debug.LogError("[chap1Controller] Chapter1Prefab or PrefabParent is not assigned.");
        }

        // 타임라인 초기 설정: 재생하지 않고 대기 상태로 설정
        if (playableDirector != null)
        {*/
            

            // 타임라인 시작
            playableDirector.time = 0; // 타임라인 시간 초기화
            playableDirector.Stop();   // 타임라인 정지
            playableDirector.Play();   // 타임라인 재생
      /*  }
        else
        {
            Debug.LogError("[chap1Controller] PlayableDirector not assigned.");
        }*/
    }


    void Start()
    {
    

    }

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
