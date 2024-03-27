using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviour
{
    [SerializeField]
    float totalTime = 300.0f;

    public Transform itemsRoot = null;
    public Transform enemysRoot = null;
    public Transform particleRoot = null;
    public GameObject objectPool = null;

    public EnemySpawner enemySpawner = null;
    public StartButton startButton = null;

    [SerializeField]
    GameClearScene gameClearScene = null;

    private int score = 0;

    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            GameManager.inst.uiManager.scoreText.text = "Score : " + score.ToString();
        }
    }

    private float playTime = 0f;
    public float PlayTime
    {
        get { return playTime; }
        set
        {
            playTime = value;
            int min = (int)playTime / 60;
            int sec = (int)playTime % 60;
            GameManager.inst.uiManager.playTimeText.text = $"{min.ToString("00")} : {sec.ToString("00")}";
        }
    }

    private int waveNum = 0;
    public int WaveNum
    {
        get { return waveNum; }
        set
        {
            waveNum = value;
            GameManager.inst.uiManager.waveText.text = $"Wave {waveNum + 1}";
        }
    }

    public bool IsPlay { get; set; } = false;


    private void Awake()
    {
        GameManager.inst.inGameManager = this;

        Score = 0;
        PlayTime = totalTime;
        WaveNum = 0;
    }

    private void Start()
    {
        StartCoroutine(OpeningTutoritalPopup());

        //��� ���� ���
        GameManager.inst.soundManager.PlayBGM(BGM.InGame);
    }

    /// <summary>
    /// �ε� �� �� �����ð� �Ŀ� Ʃ�丮�� â�� ��
    /// </summary>
    IEnumerator OpeningTutoritalPopup()
    {
        //ó���� ���� ���ϰ�
        GameManager.inst.player.CanUseUI = false;
        GameManager.inst.player.CanMove = false;
        GameManager.inst.uiManager.IsCursorLock = false;

        yield return new WaitForSeconds(1.0f);

        OpenTutorialPopup();
    }

    public void OpenTutorialPopup()
    {
        GameManager.inst.uiManager.tutorialPopupWindow.SetActive(true);
        GameManager.inst.uiManager.IsCursorLock = false;
        StopTime(true);
    }

    public void CloseTutorialPopup()
    {
        GameManager.inst.uiManager.tutorialPopupWindow.SetActive(false);
        StopTime(false);
        GameManager.inst.player.CanUseUI = true;
        GameManager.inst.player.CanMove = true;
        GameManager.inst.uiManager.IsCursorLock = true;
    }

    private void Update()
    {
        if(IsPlay)
        {
            if (PlayTime > 0)
                PlayTime -= Time.deltaTime;
            else
            {
                PlayTime = 0;
                IsPlay = false;
                //Game Clear!
                ClearGame();
            }
        }
    }

    public void StartWave()
    {
        IsPlay = true;
        enemySpawner.StartWave(0);
    }

    /// <summary>
    /// ���� �׿��� ��, ������ ��� �Լ�
    /// </summary>  
    public void AddScore(int score)
    {
        Score += score;
    }


    /// <summary>
    /// Go to Title ��ư�� ������ ��,
    /// </summary>
    public void GotoTitle()
    {
        StartCoroutine(GotoTitleCo());

        //������� ����
        GameManager.inst.soundManager.StopBGM();
    }
    IEnumerator GotoTitleCo()
    {
        StopTime(false);
        GameManager.inst.player.CanUseUI = false;

        //�÷��̾� �浹���� ����
        GameManager.inst.player.IsCollision = false;

        yield return new WaitForSeconds(1.0f);

        SceneManager.LoadSceneAsync(0);
    }

    /// <summary>
    /// ������ ������, ���� ��ų�� �����ϴ� �Լ�(TimeScale�� 0�̳� 1�� �ٲ�)
    /// </summary>
    /// <param name="isStop"> true�� time scale = 1, false�� 0 </param>
    public void StopTime(bool isStop)
    {
        if(isStop)
        {
            Time.timeScale = 0.0f;
            GameManager.inst.player.CanMove = false;
        }
        else
        {
            Time.timeScale = 1.0f;
            GameManager.inst.player.CanMove = true;
        }
    }

    /// <summary>
    /// ���� �ð��� ������, Game Clear
    /// </summary>
    private void ClearGame()
    {
        GameManager.inst.player.CanUseUI = false;
        GameManager.inst.player.CanMove = false;
        GameManager.inst.player.IsCollision = false;
        GameManager.inst.uiManager.IsCursorLock = false;


        //Wave ����, EnemySpawn ����
        enemySpawner.StopWave();

        StartCoroutine(ClearingGame());
    }

    IEnumerator ClearingGame()
    {
        FadeInOutImage fadeinout = FindObjectOfType<FadeInOutImage>();
        fadeinout?.FadeOut();

        yield return new WaitForSeconds(1.0f);

        //enemy ��ü ����
        Destroy(GameManager.inst.inGameManager.enemysRoot.gameObject);

        //ui ����
        GameManager.inst.uiManager.canvas.SetActive(false);

        //ī�޶� �̵� (�ƾ�)
        gameClearScene.gameObject.SetActive(true);
        gameClearScene.ChangeCam();

        fadeinout?.FadeIn();
        gameClearScene.PlayHelicopter();

        //BGM, ��� Sound
        GameManager.inst.soundManager.PlayBGM(BGM.GameClear);

        yield return new WaitForSeconds(1.0f);

        GameManager.inst.uiManager.gameClearUI.gameObject.SetActive(true);
        GameManager.inst.uiManager.gameClearUI.SetScore(Score);
    }
}
