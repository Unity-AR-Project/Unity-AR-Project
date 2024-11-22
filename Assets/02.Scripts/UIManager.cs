using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    // Singleton Instance
    public static UIManager instance { get; private set; }

    // Model: UI Prefabs
    [Header("UI Prefabs")]
    [Tooltip("일시정지/재생 버튼 프리팹")]
    [SerializeField] private GameObject pauseButtonPrefab;

    [Tooltip("로딩 UI 프리팹")]
    [SerializeField] private GameObject loadingUIPrefab;

    [Tooltip("에러 메시지 텍스트 프리팹")]
    [SerializeField] private GameObject errorMessageTextPrefab;

    // View: UI Elements
    private Button pauseButton;
    private GameObject loadingUI;
    private Text errorMessageText;

    // Controller: 상태 관리
    private bool isPaused = false;

    private void Awake()
    {
        // Singleton Pattern Implementation
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // UI Elements Initialization
            InitializeUIElements();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// UI 요소들을 초기화합니다.
    /// </summary>
    private void InitializeUIElements()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("씬에 Canvas가 존재하지 않습니다.");
            return;
        }

        // Pause Button Initialization
        if (pauseButtonPrefab != null)
        {
            GameObject pauseButtonObj = Instantiate(pauseButtonPrefab, canvas.transform);
            pauseButton = pauseButtonObj.GetComponent<Button>();
            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(OnPauseButtonClicked);
                pauseButton.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Pause Button Prefab에 Button 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("Pause Button Prefab이 설정되지 않았습니다.");
        }

        // Loading UI Initialization
        if (loadingUIPrefab != null)
        {
            loadingUI = Instantiate(loadingUIPrefab, canvas.transform);
            loadingUI.SetActive(false);
        }
        else
        {
            Debug.LogError("Loading UI Prefab이 설정되지 않았습니다.");
        }

        // Error Message Text Initialization
        if (errorMessageTextPrefab != null)
        {
            GameObject errorMessageObj = Instantiate(errorMessageTextPrefab, canvas.transform);
            errorMessageText = errorMessageObj.GetComponent<Text>();
            if (errorMessageText != null)
            {
                errorMessageText.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Error Message Text Prefab에 Text 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("Error Message Text Prefab이 설정되지 않았습니다.");
        }
    }

    /// <summary>
    /// Pause Button 클릭 시 호출되는 메서드
    /// </summary>
    private void OnPauseButtonClicked()
    {
        isPaused = !isPaused;
        //SoundManager.instance.ToggleNarrationPause();
        UpdatePauseButtonUI();
    }

    /// <summary>
    /// Pause Button의 UI를 업데이트합니다.
    /// </summary>
    public void UpdatePauseButtonUI()
    {
        if (pauseButton != null)
        {
            Text buttonText = pauseButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = isPaused ? "Play" : "Pause";
            }
        }
    }

    /// <summary>
    /// Pause Button을 화면에 표시합니다.
    /// </summary>
    public void ShowPauseButton()
    {
        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(true);
            UpdatePauseButtonUI();
        }
    }

    /// <summary>
    /// Pause Button을 화면에서 숨깁니다.
    /// </summary>
    public void HidePauseButton()
    {
        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 로딩 UI를 화면에 표시합니다.
    /// </summary>
    /// <param name="message">표시할 메시지</param>
    public void ShowLoadingUI(string message)
    {
        if (loadingUI != null)
        {
            loadingUI.SetActive(true);
            Text loadingText = loadingUI.GetComponentInChildren<Text>();
            if (loadingText != null)
            {
                loadingText.text = message;
            }
        }
    }

    /// <summary>
    /// 로딩 UI를 화면에서 숨깁니다.
    /// </summary>
    public void HideLoadingUI()
    {
        if (loadingUI != null)
        {
            loadingUI.SetActive(false);
        }
    }

    /// <summary>
    /// 에러 메시지를 화면에 표시합니다.
    /// </summary>
    /// <param name="message">표시할 메시지</param>
    public void ShowErrorMessage(string message)
    {
        if (errorMessageText != null)
        {
            errorMessageText.gameObject.SetActive(true);
            errorMessageText.text = message;
            Invoke("HideErrorMessage", 3f); // 3초 후에 자동으로 숨김
        }
    }

    /// <summary>
    /// 에러 메시지를 화면에서 숨깁니다.
    /// </summary>
    private void HideErrorMessage()
    {
        if (errorMessageText != null)
        {
            errorMessageText.gameObject.SetActive(false);
        }
    }
}
