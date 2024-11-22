using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    // Singleton Instance
    public static UIManager instance { get; private set; }

    // Model: UI Prefabs
    [Header("UI Prefabs")]
    [Tooltip("�Ͻ�����/��� ��ư ������")]
    [SerializeField] private GameObject pauseButtonPrefab;

    [Tooltip("�ε� UI ������")]
    [SerializeField] private GameObject loadingUIPrefab;

    [Tooltip("���� �޽��� �ؽ�Ʈ ������")]
    [SerializeField] private GameObject errorMessageTextPrefab;

    // View: UI Elements
    private Button pauseButton;
    private GameObject loadingUI;
    private Text errorMessageText;

    // Controller: ���� ����
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
    /// UI ��ҵ��� �ʱ�ȭ�մϴ�.
    /// </summary>
    private void InitializeUIElements()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("���� Canvas�� �������� �ʽ��ϴ�.");
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
                Debug.LogError("Pause Button Prefab�� Button ������Ʈ�� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogError("Pause Button Prefab�� �������� �ʾҽ��ϴ�.");
        }

        // Loading UI Initialization
        if (loadingUIPrefab != null)
        {
            loadingUI = Instantiate(loadingUIPrefab, canvas.transform);
            loadingUI.SetActive(false);
        }
        else
        {
            Debug.LogError("Loading UI Prefab�� �������� �ʾҽ��ϴ�.");
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
                Debug.LogError("Error Message Text Prefab�� Text ������Ʈ�� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogError("Error Message Text Prefab�� �������� �ʾҽ��ϴ�.");
        }
    }

    /// <summary>
    /// Pause Button Ŭ�� �� ȣ��Ǵ� �޼���
    /// </summary>
    private void OnPauseButtonClicked()
    {
        isPaused = !isPaused;
        //SoundManager.instance.ToggleNarrationPause();
        UpdatePauseButtonUI();
    }

    /// <summary>
    /// Pause Button�� UI�� ������Ʈ�մϴ�.
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
    /// Pause Button�� ȭ�鿡 ǥ���մϴ�.
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
    /// Pause Button�� ȭ�鿡�� ����ϴ�.
    /// </summary>
    public void HidePauseButton()
    {
        if (pauseButton != null)
        {
            pauseButton.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// �ε� UI�� ȭ�鿡 ǥ���մϴ�.
    /// </summary>
    /// <param name="message">ǥ���� �޽���</param>
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
    /// �ε� UI�� ȭ�鿡�� ����ϴ�.
    /// </summary>
    public void HideLoadingUI()
    {
        if (loadingUI != null)
        {
            loadingUI.SetActive(false);
        }
    }

    /// <summary>
    /// ���� �޽����� ȭ�鿡 ǥ���մϴ�.
    /// </summary>
    /// <param name="message">ǥ���� �޽���</param>
    public void ShowErrorMessage(string message)
    {
        if (errorMessageText != null)
        {
            errorMessageText.gameObject.SetActive(true);
            errorMessageText.text = message;
            Invoke("HideErrorMessage", 3f); // 3�� �Ŀ� �ڵ����� ����
        }
    }

    /// <summary>
    /// ���� �޽����� ȭ�鿡�� ����ϴ�.
    /// </summary>
    private void HideErrorMessage()
    {
        if (errorMessageText != null)
        {
            errorMessageText.gameObject.SetActive(false);
        }
    }
}
