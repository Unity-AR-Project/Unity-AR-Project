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
        // 마이크 사용을 시작하는 코드 추가
    }

    private void PermissionDenied(string permission)
    {
        Debug.Log("Microphone Permission Denied.");
        // 권한이 없을 때의 처리 코드 추가
    }

    private void PermissionDeniedAndDontAskAgain(string permission)
    {
        Debug.Log("Microphone permission denied and do not ask again selected.");
        // 권한 설정을 안내하는 코드 추가
    }
}
