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
    //�ػ�
    [SerializeField]
    TMPro.TMP_Dropdown resolution;
    //��üȭ��, â���
    [SerializeField]
    Toggle windowMode;
    //Graphic option Apply ��ư
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
    /// Graphic Setting�� Apply�� ������ ��, (Graphic ���� ���� �� ����.)
    /// </summary>
    public void ApplyGraphicSetting()
    {
        int optionNum = resolution.value;
        ScreenResolution screenResolution = screenResolutionOptions[optionNum];
        bool isFullScreen = windowMode.isOn;
        Screen.SetResolution(screenResolution.width, screenResolution.height, isFullScreen);

        //���Ŀ� �����ϱ�
        PlayerPrefs.SetInt("ResolutionValue", optionNum);
        PlayerPrefs.SetInt("IsFullScreen", isFullScreen ? 1 : 0);

        graphicApplyButton.interactable = false;
    }

    /// <summary>
    /// PlayerPrefs�� ����� ������ ��������.(ui�� ����)
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

        //Sound ����

    }

    /// <summary>
    /// BackGround Volume ���� ��
    /// </summary>
    public void SetBGVolume()
    {
        PlayerPrefs.SetFloat("BackgroundVolume", bgSound.value);
        GameManager.inst.soundManager.SetBGMVolume(bgSound.value);
    }
    /// <summary>
    /// Effect Volume ���� ��
    /// </summary>
    public void SetEffectVolume()
    {
        PlayerPrefs.SetFloat("EffectVolume", effectSound.value);
        GameManager.inst.soundManager.SetEffectVolume(effectSound.value);
    }

    /// <summary>
    /// Back Button ������ ��
    /// </summary>
    public void OnBackButtonInSetting()
    {
        if (graphicApplyButton.interactable)
        {
            //��������� ������ �˾� â ����
            savePopup.SetActive(true);
        }
        else
        {
            //���� ���� ������ �׳� â �����
            gameObject.SetActive(false);
        }
    }
}
