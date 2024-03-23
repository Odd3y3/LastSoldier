using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField]
    Image fadeInOut = null;

    public void StartGame()
    {
        StartCoroutine(StartingGame());
    }
    IEnumerator StartingGame()
    {
        FindObjectOfType<FadeInOutImage>()?.FadeOut();

        yield return new WaitForSeconds(1.0f);

        GameManager.inst.sceneLoader.LoadScene(2);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
