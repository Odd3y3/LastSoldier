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
    /// EscMenuUI�� ���̰ų� ����� �ϴ� �Լ�
    /// </summary>
    /// <param name="flag">true�� ���̰���. false�� �Ⱥ��̰� ��</param>
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
