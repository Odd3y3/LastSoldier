using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum BGM
{
    Title,
    InGame,
    GameOver,
    GameClear
}

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    AudioSource bgmSource;
    [SerializeField]
    AudioSource uiSoundSource;


    [SerializeField]
    AudioClip titleBGM = null;
    [SerializeField]
    AudioClip inGameBGM = null;
    [SerializeField]
    AudioClip gameOverBGM = null;
    [SerializeField]
    AudioClip gameClearBGM = null;
    [SerializeField]
    AudioClip uiClickSound = null;
    [SerializeField]
    AudioClip uiCancelSound = null;
    [SerializeField]
    AudioClip bossAppearSound = null;

    Dictionary<int, AudioSource> audioSourceDict;

    private void Awake()
    {
        if(PlayerPrefs.HasKey("BackgroundVolume"))
            bgmSource.volume = PlayerPrefs.GetFloat("BackgroundVolume");
        if (PlayerPrefs.HasKey("EffectVolume"))
            uiSoundSource.volume = PlayerPrefs.GetFloat("EffectVolume");

        audioSourceDict = new Dictionary<int, AudioSource>();
    }

    /// <summary>
    /// 배경음 조절하는 함수
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmSource.volume = volume;
    }

    /// <summary>
    /// 효과음 조절하는 함수
    /// </summary>
    public void SetEffectVolume(float volume)
    {
        uiSoundSource.volume = volume;

        foreach (AudioSource source in audioSourceDict.Values)
        {
            source.volume = volume;
        }
    }

    public void PlayUIClickSound()
    {
        uiSoundSource.PlayOneShot(uiClickSound);
    }

    public void PlayUICancelSound()
    {
        uiSoundSource.PlayOneShot(uiCancelSound);
    }

    public void PlayBossAppearSound()
    {
        bgmSource.PlayOneShot(bossAppearSound);
    }

    /// <summary>
    /// BGM 재생하는 함수
    /// </summary>
    public void PlayBGM(BGM bgm)
    {
        switch (bgm)
        {
            case BGM.Title:
                bgmSource.clip = titleBGM;
                bgmSource.Play();
                break;
            case BGM.InGame:
                bgmSource.clip = inGameBGM;
                bgmSource.Play();
                break;
            case BGM.GameOver:
                bgmSource.clip = gameOverBGM;
                bgmSource.Play();
                break;
            case BGM.GameClear:
                bgmSource.clip = gameClearBGM;
                bgmSource.Play();
                break;
            default:
                break;
        }
    }

    public void StopBGM()
    {
        StartCoroutine(StopingBGM());
    }

    IEnumerator StopingBGM()
    {
        float volume = bgmSource.volume;
        float time = 1.0f;
        while (time > 0f)
        {
            bgmSource.volume = volume * time;
            time -= Time.deltaTime;

            yield return null;
        }
        bgmSource.Stop();
        bgmSource.volume = volume;
    }

    /// <summary>
    /// AudioSourceDictionary에 등록
    /// </summary>
    /// <param name="source">AudioSource Instance</param>
    /// <param name="instID">InstanceID, GetInstanceID() 로 얻은 값을 사용</param>
    public void AddAudioSource(AudioSource source, int instID)
    {
        audioSourceDict.Add(instID, source);
    }

    /// <summary>
    /// AudioSourceDictionary에서 등록 해제, 삭제
    /// </summary>
    /// <param name="instID">InstanceID, GetInstanceID() 로 얻은 값을 사용</param>
    public void DeleteAudioSource(int instID)
    {
        audioSourceDict.Remove(instID);
    }
}
