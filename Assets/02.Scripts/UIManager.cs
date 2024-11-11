/*using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����� ��� ���¿� ���� UI ���(���/�Ͻ����� ������)�� ������Ʈ�ϴ� Ŭ����
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI ��� ����")]
    public Image playPauseIcon;      // ���/�Ͻ����� �������� ǥ���� UI Image ������Ʈ
    public Sprite playSprite;        // ��� ���¸� ��Ÿ���� ��������Ʈ
    public Sprite pauseSprite;       // �Ͻ����� ���¸� ��Ÿ���� ��������Ʈ

    private ARImageMultipleObjectsSpawner _spawner;

    private void Start()
    {
        // �� ������ ARImageMultipleObjectsSpawner ã��
        _spawner = FindObjectOfType<ARImageMultipleObjectsSpawner>();
        if (_spawner != null)
        {
            // ARImageMultipleObjectsSpawner�� ����� ���� ���� �̺�Ʈ�� �޼��� ����
            _spawner.OnAudioStateChanged += UpdatePlayPauseIcon;
        }
        else
        {
            Debug.LogWarning("ARImageMultipleObjectsSpawner�� ã�� �� �����ϴ�.");
        }
    }

    private void OnDestroy()
    {
        if (_spawner != null)
        {
            _spawner.OnAudioStateChanged -= UpdatePlayPauseIcon;
        }
    }

    /// <summary>
    /// ����� ��� ���¿� ���� UI �������� ������Ʈ�ϴ� �޼���
    /// </summary>
    /// <param name="isPlaying">������� ���� ��� ������ ����</param>
    public void UpdatePlayPauseIcon(bool isPlaying)
    {
        if (playPauseIcon != null && playSprite != null && pauseSprite != null)
        {
            playPauseIcon.sprite = isPlaying ? pauseSprite : playSprite;
        }
        else
        {
            Debug.LogWarning("UIManager�� playPauseIcon, playSprite, �Ǵ� pauseSprite�� �������� �ʾҽ��ϴ�.");
        }
    }
}
*/