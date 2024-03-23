using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPopupWindow : MonoBehaviour
{
    [SerializeField]
    private GameObject[] pages;

    [SerializeField]
    private GameObject leftButton;
    [SerializeField]
    private GameObject rightButton;

    int curPageNum = 0;

    private void Awake()
    {
        curPageNum = 0;
    }

    private void Start()
    {
        ShowPage(curPageNum);
    }

    public void NextPage()
    {
        curPageNum++;
        ShowPage(curPageNum);
    }

    public void PrevPage()
    {
        curPageNum--;
        ShowPage(curPageNum);
    }

    private void ShowPage(int pageNum)
    {
        for(int i = 0; i < pages.Length; i++)
        {
            if(i == pageNum)
                pages[i].SetActive(true);
            else
                pages[i].SetActive(false);
        }

        if (curPageNum == 0)
            leftButton.SetActive(false);
        else
            leftButton.SetActive(true);

        if (curPageNum == pages.Length - 1)
            rightButton.SetActive(false);
        else
            rightButton.SetActive(true);
    }
}
