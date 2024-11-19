using UnityEngine;
using UnityEngine.Android;

public class MicrophonePermissionRequester : MonoBehaviour
{
    void Start()
    {


#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            PermissionCallbacks permissionCallbacks = new PermissionCallbacks();
            permissionCallbacks.PermissionGranted += PermissionGranted;
            permissionCallbacks.PermissionDenied += PermissionDenied;
            permissionCallbacks.PermissionDeniedAndDontAskAgain += PermissionDeniedAndDontAskAgain;
            Permission.RequestUserPermission(Permission.Microphone, permissionCallbacks);
        }
#endif
    }


    private void PermissionGranted(string permission)
    {
        Debug.Log("Microphone authorization approved.");
        // ����ũ ����� �����ϴ� �ڵ� �߰�
    }

    private void PermissionDenied(string permission)
    {
        Debug.Log("Microphone Permission Denied.");
        // ������ ���� ���� ó�� �ڵ� �߰�
    }

    private void PermissionDeniedAndDontAskAgain(string permission)
    {
        Debug.Log("Microphone permission denied and do not ask again selected.");
        // ���� ������ �ȳ��ϴ� �ڵ� �߰�
    }
}
