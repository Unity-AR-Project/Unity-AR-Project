using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 사운드를 관리하는 싱글톤 클래스
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }

    /// <summary>
    /// 이름과 AudioClip을 연관짓는 구조체입니다.
    /// 유니티 에디터에서 쉽게 오디오 클립을 할당할 수 있도록 Serializable로 설정했습니다.
    /// </summary>
    [System.Serializable]
    public struct NamedAudioClip
    {
        public string name;      // AudioClip의 이름 식별자
        public AudioClip clip;   // AudioClip 자산
    }

    [Header("Narration Clips")]
    [Tooltip("챕터별 나레이션 오디오 클립 리스트")]
    public NamedAudioClip[] narrationClipList;

    [Header("Sound Effects")]
    [Tooltip("사운드 효과 오디오 클립 리스트")]
    public NamedAudioClip[] sfxClipList;

    // 나레이션과 사운드 효과를 저장할 딕셔너리
    private Dictionary<string, AudioClip> narrationDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();

    private AudioSource narrationSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // AudioSource 컴포넌트 초기화
            narrationSource = gameObject.AddComponent<AudioSource>();
            sfxSource = gameObject.AddComponent<AudioSource>();

            // AudioSource 설정
            narrationSource.playOnAwake = false;
            narrationSource.loop = false;

            sfxSource.playOnAwake = false;
            sfxSource.loop = false;

            // 나레이션 딕셔너리 초기화
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
                        Debug.LogWarning($"나레이션 딕셔너리에 '{namedClip.name}' 키가 이미 존재합니다.");
                    }
                }
                else
                {
                    Debug.LogWarning("NamedAudioClip에 name 또는 clip이 설정되지 않았습니다.");
                }
            }

            // 사운드 효과 딕셔너리 초기화
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
                        Debug.LogWarning($"사운드 효과 딕셔너리에 '{namedClip.name}' 키가 이미 존재합니다.");
                    }
                }
                else
                {
                    Debug.LogWarning("NamedAudioClip에 name 또는 clip이 설정되지 않았습니다.");
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 지정된 챕터의 나레이션을 재생합니다.
    /// </summary>
    /// <param name="chapterName">챕터 이름 (예: "chap1")</param>
    public void PlayNarration(string chapterName)
    {
        if (narrationDict.ContainsKey(chapterName))
        {
            narrationSource.clip = narrationDict[chapterName];
            narrationSource.Play();
        }
        else
        {
            Debug.LogWarning($"'{chapterName}'에 해당하는 나레이션 클립이 없습니다.");
        }
    }

    /// <summary>
    /// 현재 재생 중인 나레이션을 정지합니다.
    /// </summary>
    public void StopNarration()
    {
        if (narrationSource.isPlaying)
        {
            narrationSource.Stop();
        }
    }

    /// <summary>
    /// 나레이션의 일시정지/재개를 토글합니다.
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
    /// 사운드 효과를 재생합니다.
    /// </summary>
    /// <param name="sfxName">사운드 효과 클립 이름</param>
    public void PlaySFX(string sfxName)
    {
        if (sfxClips.ContainsKey(sfxName))
        {
            sfxSource.PlayOneShot(sfxClips[sfxName]);
            Debug.Log("door 테스트 ");
        }
        else
        {
            Debug.LogWarning($"'{sfxName}'에 해당하는 사운드 효과 클립이 없습니다.");
        }
    }
}
