using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� ���带 �����ϴ� �̱��� Ŭ����
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }

    /// <summary>
    /// �̸��� AudioClip�� �������� ����ü�Դϴ�.
    /// ����Ƽ �����Ϳ��� ���� ����� Ŭ���� �Ҵ��� �� �ֵ��� Serializable�� �����߽��ϴ�.
    /// </summary>
    [System.Serializable]
    public struct NamedAudioClip
    {
        public string name;      // AudioClip�� �̸� �ĺ���
        public AudioClip clip;   // AudioClip �ڻ�
    }

    [Header("Narration Clips")]
    [Tooltip("é�ͺ� �����̼� ����� Ŭ�� ����Ʈ")]
    public NamedAudioClip[] narrationClipList;

    [Header("Sound Effects")]
    [Tooltip("���� ȿ�� ����� Ŭ�� ����Ʈ")]
    public NamedAudioClip[] sfxClipList;

    // �����̼ǰ� ���� ȿ���� ������ ��ųʸ�
    private Dictionary<string, AudioClip> narrationDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();

    private AudioSource narrationSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        // �̱��� ���� ����
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // AudioSource ������Ʈ �ʱ�ȭ
            narrationSource = gameObject.AddComponent<AudioSource>();
            sfxSource = gameObject.AddComponent<AudioSource>();

            // AudioSource ����
            narrationSource.playOnAwake = false;
            narrationSource.loop = false;

            sfxSource.playOnAwake = false;
            sfxSource.loop = false;

            // �����̼� ��ųʸ� �ʱ�ȭ
            foreach (NamedAudioClip namedClip in narrationClipList)
            {
                if (!string.IsNullOrEmpty(namedClip.name) && namedClip.clip != null)
                {
                    if (!narrationDict.ContainsKey(namedClip.name))
                    {
                        narrationDict.Add(namedClip.name, namedClip.clip);
                    }
                    else
                    {
                        Debug.LogWarning($"�����̼� ��ųʸ��� '{namedClip.name}' Ű�� �̹� �����մϴ�.");
                    }
                }
                else
                {
                    Debug.LogWarning("NamedAudioClip�� name �Ǵ� clip�� �������� �ʾҽ��ϴ�.");
                }
            }

            // ���� ȿ�� ��ųʸ� �ʱ�ȭ
            foreach (NamedAudioClip namedClip in sfxClipList)
            {
                if (!string.IsNullOrEmpty(namedClip.name) && namedClip.clip != null)
                {
                    if (!sfxClips.ContainsKey(namedClip.name))
                    {
                        sfxClips.Add(namedClip.name, namedClip.clip);
                    }
                    else
                    {
                        Debug.LogWarning($"���� ȿ�� ��ųʸ��� '{namedClip.name}' Ű�� �̹� �����մϴ�.");
                    }
                }
                else
                {
                    Debug.LogWarning("NamedAudioClip�� name �Ǵ� clip�� �������� �ʾҽ��ϴ�.");
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ������ é���� �����̼��� ����մϴ�.
    /// </summary>
    /// <param name="chapterName">é�� �̸� (��: "chap1")</param>
    public void PlayNarration(string chapterName)
    {
        if (narrationDict.ContainsKey(chapterName))
        {
            narrationSource.clip = narrationDict[chapterName];
            narrationSource.Play();
        }
        else
        {
            Debug.LogWarning($"'{chapterName}'�� �ش��ϴ� �����̼� Ŭ���� �����ϴ�.");
        }
    }

    /// <summary>
    /// ���� ��� ���� �����̼��� �����մϴ�.
    /// </summary>
    public void StopNarration()
    {
        if (narrationSource.isPlaying)
        {
            narrationSource.Stop();
        }
    }

    /// <summary>
    /// �����̼��� �Ͻ�����/�簳�� ����մϴ�.
    /// </summary>
    public void ToggleNarrationPause()
    {
        if (narrationSource.isPlaying)
        {
            narrationSource.Pause();
        }
        else
        {
            narrationSource.UnPause();
        }
    }

    /// <summary>
    /// ���� ȿ���� ����մϴ�.
    /// </summary>
    /// <param name="sfxName">���� ȿ�� Ŭ�� �̸�</param>
    public void PlaySFX(string sfxName)
    {
        if (sfxClips.ContainsKey(sfxName))
        {
            sfxSource.PlayOneShot(sfxClips[sfxName]);
            Debug.Log("door �׽�Ʈ ");
        }
        else
        {
            Debug.LogWarning($"'{sfxName}'�� �ش��ϴ� ���� ȿ�� Ŭ���� �����ϴ�.");
        }
    }
}
