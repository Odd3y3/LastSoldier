using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    Image background = null;
    [SerializeField]
    TextMeshProUGUI text = null;
    [SerializeField]
    GameObject retryButtonObj = null;
    [SerializeField]
    GameObject goTitleButtonObj = null;

    Button retryButton;
    Button goTitleButton;

    private void Awake()
    {
        background.color = new Color(0, 0, 0, 0);
        text.color = new Color(1, 0, 0, 0);

        retryButton = retryButtonObj.GetComponent<Button>();
        goTitleButton = goTitleButtonObj.GetComponent<Button>();

        retryButton.interactable = false;
        goTitleButton.interactable = false;

        retryButtonObj.SetActive(false);
        goTitleButtonObj.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(StartingUIAnim());
    }

    IEnumerator StartingUIAnim()
    {
        float time = 0f;
        while(time < 1.0f)
        {
            time += Time.deltaTime;
            background.color = new Color(0, 0, 0, time);
            yield return null;
        }

        time = 0f;
        while(time < 1.0f)
        {
            time += Time.deltaTime;
            text.color = new Color(1, 0, 0, time);
            yield return null;
        }

        time = 0f;
        retryButtonObj.SetActive(true);
        goTitleButtonObj.SetActive(true);
        ColorBlock retryColorBlock = retryButton.colors;
        ColorBlock goTitleColorBlock = goTitleButton.colors;
        while(time < 1.0f)
        {
            time += Time.deltaTime;
            retryColorBlock.disabledColor = new Color(1, 1, 1, time);
            retryButton.colors = retryColorBlock;
            goTitleColorBlock.disabledColor = new Color(1, 1, 1, time);
            goTitleButton.colors = goTitleColorBlock;
            yield return null;
        }
        retryButton.interactable = true;
        goTitleButton.interactable = true;
    }

    /// <summary>
    /// Retry Button¿ª ¥≠∑∂¿ª ∂ß
    /// </summary>
    public void OnRetryButton()
    {
        GameManager.inst.soundManager.StopBGM();
        StartCoroutine(Retrying());
    }

    IEnumerator Retrying()
    {
        yield return new WaitForSeconds(1.0f);

        GameManager.inst.sceneLoader.LoadScene(2);
    }
}
