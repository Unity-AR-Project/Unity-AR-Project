using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

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

    [Tooltip("메시지 텍스트 프리팹 (TMP)")]
    [SerializeField] private GameObject messageTextPrefab;

    [Tooltip("사용자 메뉴얼 프리팹")]
    [SerializeField] private GameObject userManualPrefab; 

    // View: UI Elements
    private Button pauseButton;
    private GameObject loadingUI;
    private TextMeshProUGUI messageText;
    private GameObject userManual; // 사용자 메뉴얼 인스턴스

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
    /// 
    private void Start()
    {
        //pauseText.text = "동작을 멈추려면 화면을 터치하세요";
        // pauseText.gameObject.SetActive(true);

        // 애플리케이션 시작 시 사용자 메뉴얼을 표시합니다.
        ShowUserManual(5f); // 5초 동안 표시
    }

    /// <summary>
    /// 사용자 메뉴얼을 표시하고 일정 시간 후에 숨깁니다.
    /// </summary>
    /// <param name="duration">표시 시간 (초)</param>
    public void ShowUserManual(float duration)
    {
        if (userManual != null)
        {
            userManual.SetActive(true);
            StartCoroutine(HideUserManualAfterDelay(duration));
        }
        else
        {
            Debug.LogWarning("User Manual is not set.");
        }
    }

    private IEnumerator HideUserManualAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (userManual != null)
        {
            userManual.SetActive(false);
        }
    }

    private void HidePauseMessage()
    {
        messageText.gameObject.SetActive(false);
    }

    private void InitializeUIElements()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas does not exist in the scene");
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

        // Message Text Initialization (TMP)
        if (messageTextPrefab != null)
        {
            GameObject messageTextObj = Instantiate(messageTextPrefab, canvas.transform);
            messageText = messageTextObj.GetComponent<TextMeshProUGUI>();
            if (messageText != null)
            {
                messageText.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogError("Message Text Prefab에 TextMeshProUGUI 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogError("Message Text Prefab이 설정되지 않았습니다.");
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
            TextMeshProUGUI buttonText = pauseButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = isPaused ? "재생" : "일시정지";
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
        Debug.Log("ShowMessage Hoo : " + message);
        if (loadingUI != null)
        {
            loadingUI.SetActive(true);
            TextMeshProUGUI loadingText = loadingUI.GetComponentInChildren<TextMeshProUGUI>();
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
    /// 메시지를 화면에 표시합니다.
    /// </summary>
    /// <param name="message">표시할 메시지</param>
    public void ShowMessage(string message)
    {
        if (messageText != null)
        {
            Debug.Log("messageText: " + messageText);
            messageText.gameObject.SetActive(true);
            messageText.text = message;
            // 일정 시간 후에 자동으로 숨김
            Invoke("HideMessage", 3f);
        }
    }

    /// <summary>
    /// 메시지를 화면에서 숨깁니다.
    /// </summary>
    public void HideMessage()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }
}
