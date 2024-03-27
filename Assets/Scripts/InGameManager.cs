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

        //배경 음악 재생
        GameManager.inst.soundManager.PlayBGM(BGM.InGame);
    }

    /// <summary>
    /// 로딩 된 후 일정시간 후에 튜토리얼 창이 뜸
    /// </summary>
    IEnumerator OpeningTutoritalPopup()
    {
        //처음에 조작 못하게
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
    /// 몬스터 죽였을 때, 점수를 얻는 함수
    /// </summary>  
    public void AddScore(int score)
    {
        Score += score;
    }


    /// <summary>
    /// Go to Title 버튼을 눌렀을 때,
    /// </summary>
    public void GotoTitle()
    {
        StartCoroutine(GotoTitleCo());

        //배경음악 끄기
        GameManager.inst.soundManager.StopBGM();
    }
    IEnumerator GotoTitleCo()
    {
        StopTime(false);
        GameManager.inst.player.CanUseUI = false;

        //플레이어 충돌판정 제거
        GameManager.inst.player.IsCollision = false;

        yield return new WaitForSeconds(1.0f);

        SceneManager.LoadSceneAsync(0);
    }

    /// <summary>
    /// 게임을 멈출지, 진행 시킬지 설정하는 함수(TimeScale을 0이나 1로 바꿈)
    /// </summary>
    /// <param name="isStop"> true면 time scale = 1, false면 0 </param>
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
    /// 제한 시간이 지나면, Game Clear
    /// </summary>
    private void ClearGame()
    {
        GameManager.inst.player.CanUseUI = false;
        GameManager.inst.player.CanMove = false;
        GameManager.inst.player.IsCollision = false;
        GameManager.inst.uiManager.IsCursorLock = false;


        //Wave 정지, EnemySpawn 정지
        enemySpawner.StopWave();

        StartCoroutine(ClearingGame());
    }

    IEnumerator ClearingGame()
    {
        FadeInOutImage fadeinout = FindObjectOfType<FadeInOutImage>();
        fadeinout?.FadeOut();

        yield return new WaitForSeconds(1.0f);

        //enemy 전체 제거
        Destroy(GameManager.inst.inGameManager.enemysRoot.gameObject);

        //ui 제거
        GameManager.inst.uiManager.canvas.SetActive(false);

        //카메라 이동 (컷씬)
        gameClearScene.gameObject.SetActive(true);
        gameClearScene.ChangeCam();

        fadeinout?.FadeIn();
        gameClearScene.PlayHelicopter();

        //BGM, 헬기 Sound
        GameManager.inst.soundManager.PlayBGM(BGM.GameClear);

        yield return new WaitForSeconds(1.0f);

        GameManager.inst.uiManager.gameClearUI.gameObject.SetActive(true);
        GameManager.inst.uiManager.gameClearUI.SetScore(Score);
    }
}
