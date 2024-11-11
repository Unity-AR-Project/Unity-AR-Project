using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("-------Sound System-------")]
    public AudioSource BGMaudioSource; //배경음악
    public AudioSource SFXaudioSource; //효과음

    //사운드를 이름으로 관리 할 수 있도록 Dic 사용
    Dictionary<string, AudioClip> BGMClips = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> SFXClips = new Dictionary<string, AudioClip>();

    //Inspector에서 사운드 클립을 추가할 수 있도록 리스트 제공
    [System.Serializable]
    public struct NamedAudio
    {
        public string name;
        public AudioClip clip;
    }

    public NamedAudio[] BGMClipList;
    public NamedAudio[] SFXClipList;

    private Coroutine currentBGMCoroutine; //현재 실행중인 코루틴 추적하는 변수
    private string nextSceneName;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializedAudioClips();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Audio Clip 리스트를 Dic로 변환하여 이름으로 접근 가능하게 만드는 함수
    private void InitializedAudioClips()
    {
        foreach (var bgm in BGMClipList) //배경음
        {
            if (!BGMClips.ContainsKey(bgm.name))
            {
                BGMClips.Add(bgm.name, bgm.clip);
            }
        }

        foreach (var sfx in SFXClipList) //효과음
        {
            if (!SFXClips.ContainsKey(sfx.name))
            {
                SFXClips.Add(sfx.name, sfx.clip);
            }
        }
    }

    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name; //씬 이름을 가져옴
    }

    public void OnSceneLoaded(string sceneName) //씬 이름을 받아서 BGM을 설정하는 함수
    {
        //씬 이름에 따라 배경음을 재생하는 코드
        if (sceneName == "TileScene")
        {
            PlayBGM("Background", 1f);
        }
        else if (sceneName == "TileScene2")
        {
            PlayBGM("", 1f);
        }
        else
        {
            StopBGM();
        }
    }

    public void PlayBGM(string clipName, float fadeDuration = 1f) //배경음 재생함수
    {
        if (BGMClips.ContainsKey(clipName))
        {
            if (currentBGMCoroutine != null)
            {
                StopCoroutine(currentBGMCoroutine); //기존 페이드 코루틴이 있으면 중단하는 함수
            }

            //현재 BGM이 있는 경우 페이드아웃 후 새로운 BGM으로 페이드인
            //람다를 사용해 BGM을 재생
            currentBGMCoroutine = StartCoroutine(FadeOutBGM(fadeDuration, () =>
            {
                BGMaudioSource.clip = BGMClips[clipName];
                BGMaudioSource.Play();
                currentBGMCoroutine = StartCoroutine(FadeInBGM(fadeDuration));
            }));
        }
        else
        {
            Debug.LogWarning("해당 이름의 배경음이 존재하지 않습니다. : " + clipName);
        }
    }

    public void PlaySFX(string clipName) //효과음 재생함수
    {
        if (SFXClips.ContainsKey(clipName))
        {
            SFXaudioSource.PlayOneShot(SFXClips[clipName]);
        }
        else
        {
            Debug.LogWarning("해당 이름의 효과음이 존재하지 않습니다. : " + clipName);
        }
    }

    public void StopBGM() //배경음 멈추는 함수
    {
        BGMaudioSource.Stop();
    }

    public void StopSFX() //효과음 멈추는 함수
    {
        SFXaudioSource.Stop();
    }

    public void SetBGMVolume(float volume) //배경음 볼륨 조절하는 함수
    {
        BGMaudioSource.volume = Mathf.Clamp(volume, 0.0f, 1.0f);
    }
    public void SetSFXVolume(float volume) //효과음 볼륨 조절하는 함수
    {
        SFXaudioSource.volume = Mathf.Clamp(volume, 0.0f, 1.0f);
    }

    //BGM을 페이드아웃 시키는 코루틴
    private IEnumerator FadeOutBGM(float duration, Action onFadeComplete)
    {
        float startVolume = BGMaudioSource.volume; //BGM의 현재볼륨값을 가져옴

        for (float t = 0; t < duration; t += Time.deltaTime) //점점 소리가 작아짐
        {
            BGMaudioSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        BGMaudioSource.volume = 0f;
        onFadeComplete?.Invoke(); //페이드 아웃이 완료되면 다음 작업 실행
    }

    //BGM을 페이드인 시키는 코루틴
    private IEnumerator FadeInBGM(float duration)
    {
        float startVolume = 0f;
        BGMaudioSource.volume = 0f;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            BGMaudioSource.volume = Mathf.Lerp(startVolume, 1f, t / duration);
            //Mathf.Lerp : 두 float 값 사이의 보간된 float 결과
            yield return null;
        }
        BGMaudioSource.volume = 1f;
    }
}