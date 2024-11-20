using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Google Cloud API 키를 관리하는 클래스
/// </summary>
public class ApiKeyManager : MonoBehaviour
{
    [SerializeField] private InputField apiKeyInputField;

    private void Start()
    {
        LoadApiKey();
    }

    /// <summary>
    /// API 키를 저장합니다.
    /// </summary>
    public void SaveApiKey()
    {
        string apiKey = apiKeyInputField.text;
        if (!string.IsNullOrEmpty(apiKey))
        {
            PlayerPrefs.SetString("GoogleCloudApiKey", apiKey);
            PlayerPrefs.Save();
            Debug.Log("API key saved.");
        }
        else
        {
            Debug.LogWarning("Please enter the API key.");
        }
    }

    /// <summary>
    /// 저장된 API 키를 로드합니다.
    /// </summary>
    public void LoadApiKey()
    {
        string apiKey = PlayerPrefs.GetString("GoogleCloudApiKey", "");
        //string apiKey = PlayerPrefs.GetString("GoogleCloudApiKey", "dcc96644ca7c8823d1fb1d8f9dc6d81a327faf9e");

        if (!string.IsNullOrEmpty(apiKey))
        {
            apiKeyInputField.text = apiKey;
            Debug.Log("API key loaded..");
        }
        else
        {
            Debug.Log("No API key saved.");
        }
    }
}
