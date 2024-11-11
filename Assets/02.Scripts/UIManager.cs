/*using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 오디오 재생 상태에 따라 UI 요소(재생/일시정지 아이콘)를 업데이트하는 클래스
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI 요소 설정")]
    public Image playPauseIcon;      // 재생/일시정지 아이콘을 표시할 UI Image 컴포넌트
    public Sprite playSprite;        // 재생 상태를 나타내는 스프라이트
    public Sprite pauseSprite;       // 일시정지 상태를 나타내는 스프라이트

    private ARImageMultipleObjectsSpawner _spawner;

    private void Start()
    {
        // 씬 내에서 ARImageMultipleObjectsSpawner 찾기
        _spawner = FindObjectOfType<ARImageMultipleObjectsSpawner>();
        if (_spawner != null)
        {
            // ARImageMultipleObjectsSpawner의 오디오 상태 변경 이벤트에 메서드 연결
            _spawner.OnAudioStateChanged += UpdatePlayPauseIcon;
        }
        else
        {
            Debug.LogWarning("ARImageMultipleObjectsSpawner를 찾을 수 없습니다.");
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
    /// 오디오 재생 상태에 따라 UI 아이콘을 업데이트하는 메서드
    /// </summary>
    /// <param name="isPlaying">오디오가 현재 재생 중인지 여부</param>
    public void UpdatePlayPauseIcon(bool isPlaying)
    {
        if (playPauseIcon != null && playSprite != null && pauseSprite != null)
        {
            playPauseIcon.sprite = isPlaying ? pauseSprite : playSprite;
        }
        else
        {
            Debug.LogWarning("UIManager의 playPauseIcon, playSprite, 또는 pauseSprite가 설정되지 않았습니다.");
        }
    }
}
*/