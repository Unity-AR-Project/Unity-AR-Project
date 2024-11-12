using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// UI ��Ҹ� �����ϴ� �̱��� Ŭ����
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    [Tooltip("�Ͻ�����/��� ��ư ������")]
    public Button pauseButtonPrefab;

    private Button pauseButtonInstance;

    private bool isPaused = false;

    private void Awake()
    {
        // �̱��� ���� ����
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // �Ͻ����� ��ư ����
            CreatePauseButton();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �Ͻ�����/��� ��ư�� �����ϰ� �����մϴ�.
    /// </summary>
    private void CreatePauseButton()
    {
        if (pauseButtonPrefab == null)
        {
            Debug.LogError("UIManager���� Pause Button Prefab�� �������� �ʾҽ��ϴ�.");
            return;
        }

        // ��ư �ν��Ͻ� ���� �� �θ� ����
        pauseButtonInstance = Instantiate(pauseButtonPrefab, transform);

        // ��ư Ŭ�� �̺�Ʈ ����
        pauseButtonInstance.onClick.AddListener(OnPauseButtonClicked);

        // ��ư �ʱ⿡�� ����
        pauseButtonInstance.gameObject.SetActive(false);
    }

    /// <summary>
    /// �Ͻ����� ��ư�� Ŭ���Ǿ��� �� ȣ��Ǵ� �޼���
    /// </summary>
    private void OnPauseButtonClicked()
    {
        // �����̼� �Ͻ�����/�簳 ���
        SoundManager.instance.ToggleNarrationPause();
        isPaused = !isPaused;

        // ��ư UI ������Ʈ
        UpdatePauseButtonUI();
    }

    /// <summary>
    /// �Ͻ����� ��ư�� UI�� ������Ʈ�մϴ�.
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
    /// �Ͻ����� ��ư�� ǥ���մϴ�.
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
    /// �Ͻ����� ��ư�� ����ϴ�.
    /// </summary>
    public void HidePauseButton()
    {
        if (pauseButtonInstance != null)
        {
            pauseButtonInstance.gameObject.SetActive(false);
        }
    }
}
