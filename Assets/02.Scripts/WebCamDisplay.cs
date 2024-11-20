using UnityEngine;
using UnityEngine.UI;

public class WebCamDisplay : MonoBehaviour
{
    public RawImage rawImage;
    private WebCamTexture webCamTexture;

    void Start()
    {
        // ��� ������ ��ķ ��ġ ��� ��������
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            // ù ��° ��ķ ��ġ ���
            webCamTexture = new WebCamTexture(devices[0].name);
            rawImage.texture = webCamTexture;
            rawImage.material.mainTexture = webCamTexture;
            webCamTexture.Play();
        }
        else
        {
            Debug.LogError("��ķ�� ����Ǿ� ���� �ʽ��ϴ�.");
        }
    }

    void OnDestroy()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }
    }
}
