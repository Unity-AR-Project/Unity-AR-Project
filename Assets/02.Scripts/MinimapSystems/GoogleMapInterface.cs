using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System;

public class GoogleMapInterface : MonoBehaviour
{
    private const string _BASE_URL = "https://maps.googleapis.com/maps/api/staticmap?";
    private const string _APIKEY = "AIzaSyDZMJ0DfF2dFpj7MjrjUHewTN3dQmskMdg";
    private Texture2D _cachedTexture;

    public void LoadMap(float latitude, float longitude, float zoom, Vector2 size, Action<Texture2D> onComplete)
    {
        StartCoroutine(C_LoadMap(latitude, longitude, zoom, size, onComplete));
    }

    IEnumerator C_LoadMap(float latitude, float longitude, float zoom, Vector2 size, Action<Texture2D> onComplete)
    {
        string url = _BASE_URL + "center=" + latitude + "," + longitude +
            "&zoom=" + zoom.ToString() + "&size=" + size.x.ToString() + "x" + size.y.ToString()
            + "&key=" + _APIKEY;

        Debug.Log($"[{nameof(GoogleMapInterface)}] : Request map texture ... " + url);

        url = UnityWebRequest.UnEscapeURL(url); //Url�� ����  Web ��û
        UnityWebRequest req = UnityWebRequestTexture.GetTexture(url); //Texture�� ���� ��û 

        yield return req.SendWebRequest();  //��û ����

        _cachedTexture = DownloadHandlerTexture.GetContent(req); // ���� Texture�� RAW �̹����� ����
        onComplete.Invoke(_cachedTexture);
    }
}