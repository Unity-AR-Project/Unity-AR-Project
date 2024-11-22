using UnityEngine;

public class QuitButtonHandler : MonoBehaviour
{
    /// <summary>
    /// ���ø����̼��� �����ϴ� �޼����Դϴ�.
    /// </summary>
    public void QuitApplication()
    {
        Debug.Log("���ø����̼��� �����մϴ�.");
        Application.Quit();

        // Unity �����Ϳ��� �׽�Ʈ ���� ���� ���ø����̼��� �������� �ʰ� �α׸� ����մϴ�.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
