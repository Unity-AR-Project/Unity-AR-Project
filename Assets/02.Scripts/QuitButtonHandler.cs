using UnityEngine;

public class QuitButtonHandler : MonoBehaviour
{
    /// <summary>
    /// 애플리케이션을 종료하는 메서드입니다.
    /// </summary>
    public void QuitApplication()
    {
        Debug.Log("애플리케이션을 종료합니다.");
        Application.Quit();

        // Unity 에디터에서 테스트 중일 때는 애플리케이션을 종료하지 않고 로그를 출력합니다.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
