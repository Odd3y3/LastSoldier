using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearScene : MonoBehaviour
{
    [SerializeField]
    Animator helicopterAnim;
    [SerializeField]
    Camera gameClearCam;
    [SerializeField]
    SoundObject helicopterSoundObject;

    public void ChangeCam()
    {
        Camera.main.tag = "Untagged";
        gameClearCam.tag = "MainCamera";
    }

    /// <summary>
    /// 헬리콥터 Animation 재생 & 사운드 재생
    /// </summary>
    public void PlayHelicopter()
    {
        helicopterAnim.SetTrigger("Move");
        helicopterSoundObject.PlayClip(0);
    }
}
