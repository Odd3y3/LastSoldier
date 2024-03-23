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
    /// clips�� �ִ� ���� ���
    /// </summary>
    /// <param name="idx">����ϰ� ���� ������ clips������ idx��</param>
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
