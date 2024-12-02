using UnityEngine;
using UnityEngine.Playables;

public class Chap3Controller : MonoBehaviour, IChapterController
{
    [SerializeField]
    private PlayableDirector playableDirector;

    private bool isPaused = false; // 일시정지 상태 여부

    /*//프리팹 초기화
    [SerializeField] private GameObject chapter3Prefab; // 챕터 7 프리팹
    [SerializeField] private Transform prefabParent; // 프리팹을 인스턴스화할 부모 오브젝트
    private GameObject chapter3Instance; // 현재 활성화된 챕터 7 인스턴스
*/
    void OnEnable()
    {
        /*
        if (chapter3Instance != null)
        {
            Destroy(chapter3Instance);
        }

        // 챕터 3 프리팹 인스턴스화
        if (chapter3Prefab != null && prefabParent != null)
        {
            chapter3Instance = Instantiate(chapter3Prefab, prefabParent);
            chapter3Instance.tag = "Chapter3Instance"; // 필요 시 태그 설정
            chapter3Instance.SetActive(true);
            Debug.Log("[chap3Controller] Chapter3 prefab instantiated.");
        }
        else
        {
            Debug.LogError("[chap3Controller] Chapter1Prefab or PrefabParent is not assigned.");
        }

        // 타임라인 초기 설정: 재생하지 않고 대기 상태로 설정
        if (playableDirector != null)
        {
*/

        // 타임라인 시작
        playableDirector.time = 0; // 타임라인 시간 초기화
        playableDirector.Stop();   // 타임라인 정지
                                   // 초기 메시지 표시
        UIManager.instance.ShowMessage("3챕터 시작되었습니다.\n " +
            "기다려주세요!");
        playableDirector.Play();   // 타임라인 재생
        /*  }
          else
          {
              Debug.LogError("[chap3Controller] PlayableDirector not assigned.");
          }*/
    }

    void Start()
    {
        // playableDirector.Play(); 
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
