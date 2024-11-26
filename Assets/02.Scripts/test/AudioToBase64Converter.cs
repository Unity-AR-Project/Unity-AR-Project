using UnityEngine;
using System;
using System.IO;

public class AudioToBase64Converter : MonoBehaviour
{
    
    public string ConvertWavToBase64(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found at: " + filePath);
            return null;
        }

        // WAV 파일을 읽고 Base64로 변환
        byte[] fileBytes = File.ReadAllBytes(filePath);
        string base64String = Convert.ToBase64String(fileBytes);

        Debug.Log("Base64 Encoded Audio: " + base64String);
        return base64String;
    }
}
