using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

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

    [Tooltip("�޽��� �ؽ�Ʈ ������ (TMP)")]
    [SerializeField] private GameObject messageTextPrefab;

    //[Tooltip("�޴��� �ؽ�Ʈ ������ (TMP)")]
    //[SerializeField] private GameObject pauseText; // �Ͻ����� 

    // View: UI Elements
    private Button pauseButton;
    private GameObject loadingUI;
    private TextMeshProUGUI messageText;
    
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
    /// 
    private void Start()
    {
        //pauseText.text = "������ ���߷��� ȭ���� ��ġ�ϼ���";
       // pauseText.gameObject.SetActive(true);

        //Invoke("HidePauseMessage", 5f); //5���� ����, �ð� ��� ���̱�
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
                Debug.LogError("Message Text Prefab�� TextMeshProUGUI ������Ʈ�� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogError("Message Text Prefab�� �������� �ʾҽ��ϴ�.");
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
            TextMeshProUGUI buttonText = pauseButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = isPaused ? "���" : "�Ͻ�����";
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
    /// �޽����� ȭ�鿡 ǥ���մϴ�.
    /// </summary>
    /// <param name="message">ǥ���� �޽���</param>
    public void ShowMessage(string message)
    {
        if (messageText != null)
        {
            Debug.Log("messageText: " + messageText);
            messageText.gameObject.SetActive(true);
            messageText.text = message;
            // ���� �ð� �Ŀ� �ڵ����� ����
            Invoke("HideMessage", 3f);
        }
    }

    /// <summary>
    /// �޽����� ȭ�鿡�� ����ϴ�.
    /// </summary>
    public void HideMessage()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }
}
