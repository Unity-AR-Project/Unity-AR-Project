using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// UI 요소를 관리하는 싱글톤 클래스
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    [Tooltip("일시정지/재생 버튼 프리팹")]
    public Button pauseButtonPrefab;

    private Button pauseButtonInstance;

    private bool isPaused = false;

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // 일시정지 버튼 생성
            CreatePauseButton();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 일시정지/재생 버튼을 생성하고 설정합니다.
    /// </summary>
    private void CreatePauseButton()
    {
        if (pauseButtonPrefab == null)
        {
            Debug.LogError("UIManager에서 Pause Button Prefab이 설정되지 않았습니다.");
            return;
        }

        // 버튼 인스턴스 생성 및 부모 설정
        pauseButtonInstance = Instantiate(pauseButtonPrefab, transform);

        // 버튼 클릭 이벤트 설정
        pauseButtonInstance.onClick.AddListener(OnPauseButtonClicked);

        // 버튼 초기에는 숨김
        pauseButtonInstance.gameObject.SetActive(false);
    }

    /// <summary>
    /// 일시정지 버튼이 클릭되었을 때 호출되는 메서드
    /// </summary>
    private void OnPauseButtonClicked()
    {
        // 나레이션 일시정지/재개 토글
        SoundManager.instance.ToggleNarrationPause();
        isPaused = !isPaused;

        // 버튼 UI 업데이트
        UpdatePauseButtonUI();
    }

    /// <summary>
    /// 일시정지 버튼의 UI를 업데이트합니다.
    /// </summary>
    public void UpdatePauseButtonUI()
    {
        if (pauseButtonInstance != null)
        {
            Text buttonText = pauseButtonInstance.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = isPaused ? "Play" : "Pause";
            }
        }
    }

    /// <summary>
    /// 일시정지 버튼을 표시합니다.
    /// </summary>
    public void ShowPauseButton()
    {
        if (pauseButtonInstance != null)
        {
            pauseButtonInstance.gameObject.SetActive(true);
            UpdatePauseButtonUI();
        }
    }

    /// <summary>
    /// 일시정지 버튼을 숨깁니다.
    /// </summary>
    public void HidePauseButton()
    {
        if (pauseButtonInstance != null)
        {
            pauseButtonInstance.gameObject.SetActive(false);
        }
    }
}
