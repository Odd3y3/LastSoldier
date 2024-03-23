using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    [SerializeField]
    List<GameObject> campfireObjs = null;
    [SerializeField]
    List<GameObject> arrows = null;

    public void StartGame()
    {
        GameManager.inst.uiManager.ShowWaveText();
        GameManager.inst.inGameManager.StartWave();

        foreach (GameObject obj in campfireObjs)
        {
            obj.SetActive(true);
        }

        gameObject.SetActive(false);
        foreach (GameObject obj in arrows)
        {
            obj.SetActive(false);
        }
    }
}
