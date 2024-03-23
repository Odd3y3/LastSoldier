using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : MonoBehaviour
{
    [SerializeField]
    List<AudioClip> clips = new List<AudioClip>();

    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if(PlayerPrefs.HasKey("EffectVolume"))
            audioSource.volume = PlayerPrefs.GetFloat("EffectVolume");
    }

    private void Start()
    {
        GameManager.inst.soundManager.AddAudioSource(audioSource, GetInstanceID());
    }


    /// <summary>
    /// clips에 있는 음원 재생
    /// </summary>
    /// <param name="idx">재생하고 싶은 음원의 clips에서의 idx값</param>
    public void PlayClip(int idx)
    {
        if(idx >= 0 && idx < clips.Count)
        {
            audioSource.PlayOneShot(clips[idx]);
        }
    }

    private void OnDestroy()
    {
        GameManager.inst.soundManager.DeleteAudioSource(GetInstanceID());
    }
}
