using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
struct ScreenResolution
{
    public int width, height;
}

public class SettingWindow : MonoBehaviour
{
    //해상도
    [SerializeField]
    TMPro.TMP_Dropdown resolution;
    //전체화면, 창모드
    [SerializeField]
    Toggle windowMode;
    //Graphic option Apply 버튼
    [SerializeField]
    Button graphicApplyButton;
    //BGM Sound
    [SerializeField]
    Slider bgSound;
    //Effect Sound
    [SerializeField]
    Slider effectSound;
    [SerializeField]
    GameObject savePopup;

    [SerializeField]
    ScreenResolution[] screenResolutionOptions;
    /*
    800 600     / 0
    1024 768    / 1
    1280 720    / 2
    1280 800    / 3
    1440 900    / 4
    1680 1050   / 5
    1920 1080   / 6
    */

    private void Awake()
    {
        SetUISettingData();
        ApplyGraphicSetting();
    }

    /// <summary>
    /// Graphic Setting에 Apply를 눌렀을 때, (Graphic 셋팅 적용 후 저장.)
    /// </summary>
    public void ApplyGraphicSetting()
    {
        int optionNum = resolution.value;
        ScreenResolution screenResolution = screenResolutionOptions[optionNum];
        bool isFullScreen = windowMode.isOn;
        Screen.SetResolution(screenResolution.width, screenResolution.height, isFullScreen);

        //이후에 저장하기
        PlayerPrefs.SetInt("ResolutionValue", optionNum);
        PlayerPrefs.SetInt("IsFullScreen", isFullScreen ? 1 : 0);

        graphicApplyButton.interactable = false;
    }

    /// <summary>
    /// PlayerPrefs에 저장된 정보를 가져오기.(ui에 적용)
    /// </summary>
    public void SetUISettingData()
    {
        if(PlayerPrefs.HasKey("ResolutionValue"))
            resolution.value = PlayerPrefs.GetInt("ResolutionValue");
        if (PlayerPrefs.HasKey("IsFullScreen"))
            windowMode.isOn = PlayerPrefs.GetInt("IsFullScreen") == 0 ? false : true;

        if (PlayerPrefs.HasKey("BackgroundVolume"))
            bgSound.value = PlayerPrefs.GetFloat("BackgroundVolume");
        if (PlayerPrefs.HasKey("EffectVolume"))
            effectSound.value = PlayerPrefs.GetFloat("EffectVolume");

        graphicApplyButton.interactable = false;

        //Sound 적용

    }

    /// <summary>
    /// BackGround Volume 변경 시
    /// </summary>
    public void SetBGVolume()
    {
        PlayerPrefs.SetFloat("BackgroundVolume", bgSound.value);
        GameManager.inst.soundManager.SetBGMVolume(bgSound.value);
    }
    /// <summary>
    /// Effect Volume 변경 시
    /// </summary>
    public void SetEffectVolume()
    {
        PlayerPrefs.SetFloat("EffectVolume", effectSound.value);
        GameManager.inst.soundManager.SetEffectVolume(effectSound.value);
    }

    /// <summary>
    /// Back Button 눌렀을 때
    /// </summary>
    public void OnBackButtonInSetting()
    {
        if (graphicApplyButton.interactable)
        {
            //변경사항이 있으면 팝업 창 띄우기
            savePopup.SetActive(true);
        }
        else
        {
            //변경 사항 없으면 그냥 창 지우기
            gameObject.SetActive(false);
        }
    }
}
