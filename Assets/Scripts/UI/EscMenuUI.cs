using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscMenuUI : MonoBehaviour
{
    [SerializeField]
    GameObject settingWindow;
    [SerializeField]
    GameObject gotoTitleWindow;

    /// <summary>
    /// EscMenuUI를 보이거나 숨기게 하는 함수
    /// </summary>
    /// <param name="flag">true면 보이게함. false면 안보이게 함</param>
    public void ShowEscMenuUI(bool flag)
    {
        if (flag)
        {
            gameObject.SetActive(true);
        }
        else
        {
            settingWindow.SetActive(false);
            gotoTitleWindow.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
