using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

//�̱��� ���� (or GameManager���� ������ �� �ֵ���)
public class UIManager : MonoBehaviour
{
    public GameObject canvas;
    public GameClearUI gameClearUI;
    public ItemInfoUI itemInfoUI;
    public ItemInfoUI itemSlotInfoUI;
    public PlayerInfoUI playerInfoUI;
    public Transform dmgPopupsObj;
    public GameObject gameOverUI;
    public GameObject tutorialPopupWindow;
    public GameObject enemyBarsObj;
    public EscMenuUI escMenuUI;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI playTimeText;
    public TextMeshProUGUI waveText;
    public Transform startButtonUI;
    [SerializeField]
    Animator bossAppearAnim;
    


    //EscMenu â�� ���� �ִ���, (EscŰ ������ ��)
    bool isShowEscMenu = false;
    public bool IsShowEscMenu
    {
        get { return isShowEscMenu; }
        set
        {
            if (value)
            {
                escMenuUI.ShowEscMenuUI(true);
                IsCursorLock = false;
                GameManager.inst.inGameManager.StopTime(true);
            }
            else
            {
                escMenuUI.ShowEscMenuUI(false);
                IsCursorLock = true;
                GameManager.inst.inGameManager.StopTime(false);
            }
            isShowEscMenu = value;
        }
    }
    public void HideEscMenu()
    {
        IsShowEscMenu = false;
    }

    //�κ��丮 â�� ���� �ִ���, (TabŰ ������ ��)
    bool isShowInventory = false;
    public bool IsShowInventory
    {
        get { return isShowInventory; }
        set
        {
            isShowInventory = value;
            if(value)
            {
                playerInfoUI.Show();
            }
            else
            {
                playerInfoUI.Hide();
                itemSlotInfoUI.Hide();
            }
        }
    }

    //Ŀ���� ��� �ִ���(�κ��丮 �����ִµ��� ���콺��Ű ������ ��)
    bool isCursorLock = false;
    public bool IsCursorLock
    {
        get { return isCursorLock; }
        set
        {
            if (value)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            isCursorLock = value;
        }
    }

    public void ShowStartButton(Transform pos)
    {
        startButtonUI.position = Camera.main.WorldToScreenPoint(pos.position + Vector3.up);

        startButtonUI.gameObject.SetActive(true);
    }

    public void HideStartButton()
    {
        startButtonUI.gameObject.SetActive(false);
    }

    public void ShowWaveText()
    {
        waveText.gameObject.SetActive(true);
    }

    private void Awake()
    {
        GameManager.inst.uiManager = this;
    }

    public void PlayUIClickSound()
    {
        GameManager.inst.soundManager.PlayUIClickSound();
    }

    /// <summary>
    /// �÷��̾ �׾��� ��,
    /// </summary>
    public void GameOver()
    {
        gameOverUI.SetActive(true);
    }

    /// <summary>
    /// ������ ���� ���� ��, UI ǥ��
    /// </summary>
    public void AppearBossUI()
    {
        bossAppearAnim.SetTrigger("Appear");
    }
}
