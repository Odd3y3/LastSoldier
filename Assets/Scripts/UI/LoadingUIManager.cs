using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUIManager : MonoBehaviour
{
    [SerializeField]
    Slider slider = null;
    [SerializeField]
    TextMeshProUGUI loading = null;
    [SerializeField]
    GameObject pressAnyKey = null;

    private void Start()
    {
        if(loading != null)
        {
            StartCoroutine(LoadingTitleAnimation(0.5f));
        }
    }
    
    IEnumerator LoadingTitleAnimation(float time)
    {
        WaitForSeconds wait = new WaitForSeconds(time);
        string originStr = "Loading";
        int flag = 0;
        while (true)
        {
            string text = originStr;
            for(int i = 0; i < flag; i++)
            {
                text += ".";
            }
            
            if(loading != null)
                loading.text = text;

            flag++;
            flag %= 4;
            yield return wait;
        }
    }

    public void CompleteLoad()
    {
        loading.gameObject.SetActive(false);
        pressAnyKey.SetActive(true);
    }
}
