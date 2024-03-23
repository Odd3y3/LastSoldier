using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class GameClearUI : MonoBehaviour
{
    [SerializeField]
    GameObject newHighScoreObj = null;
    [SerializeField]
    TextMeshProUGUI highScoreText = null;
    [SerializeField]
    TextMeshProUGUI currentScoreText = null;

    int prevHighScore = 0;

    public void SetScore(int score)
    {
        if (PlayerPrefs.HasKey("HighScore"))
            prevHighScore = PlayerPrefs.GetInt("HighScore");

        highScoreText.text = $"High Score : {prevHighScore}";

        if (score > prevHighScore)
        {
            newHighScoreObj.SetActive(true);
            highScoreText.text = $"High Score : {score}";
            PlayerPrefs.SetInt("HighScore", score);
        }
        currentScoreText.text = $"Score : {score}";
    }
}
