using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    private void Start()
    {
        //BGM Ʋ��
        GameManager.inst.soundManager.PlayBGM(BGM.Title);
    }

    public void PlayUIClickSound()
    {
        GameManager.inst.soundManager.PlayUIClickSound();
    }

    public void PlayUICancelSound()
    {
        GameManager.inst.soundManager.PlayUICancelSound();
    }

    public void StopBGM()
    {
        GameManager.inst.soundManager.StopBGM();
    }
}
