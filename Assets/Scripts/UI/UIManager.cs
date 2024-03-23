using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

//싱글톤 예정 (or GameManager에서 접근할 수 있도록)
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
    


    //EscMenu 창이 켜져 있는지, (Esc키 눌렀을 때)
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

    //인벤토리 창이 켜져 있는지, (Tab키 눌렀을 때)
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

    //커서가 잠겨 있는지(인벤토리 켜져있는동안 마우스휠키 눌렀을 때)
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
    /// 플레이어가 죽었을 때,
    /// </summary>
    public void GameOver()
    {
        gameOverUI.SetActive(true);
    }

    /// <summary>
    /// 보스가 등장 했을 때, UI 표시
    /// </summary>
    public void AppearBossUI()
    {
        bossAppearAnim.SetTrigger("Appear");
    }
}
