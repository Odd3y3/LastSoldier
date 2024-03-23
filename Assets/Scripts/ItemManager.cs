using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    //전체 Item 리스트
    [SerializeField]
    ItemInfo[] itemList = null;

    //등급별 확률
    [SerializeField]
    float itemProb_Common = 30.0f;
    [SerializeField]
    float itemProb_Uncommon = 30.0f;
    [SerializeField]
    float itemProb_Rare = 20.0f;
    [SerializeField]
    float itemProb_Epic = 14.0f;
    [SerializeField]
    float itemProb_Legendary = 6.0f;


    Dictionary<ItemGrade, List<ItemInfo>> ItemDict = null;

    private void Awake()
    {
        ItemDict = new Dictionary<ItemGrade, List<ItemInfo>>();
        ItemDict[ItemGrade.Common] = new List<ItemInfo>();
        ItemDict[ItemGrade.Uncommon] = new List<ItemInfo>();
        ItemDict[ItemGrade.Rare] = new List<ItemInfo>();
        ItemDict[ItemGrade.Epic] = new List<ItemInfo>();
        ItemDict[ItemGrade.Legendary] = new List<ItemInfo>();

        foreach (ItemInfo item in itemList)
        {
            ItemDict[item.Grade].Add(item);
        }

    }

    /// <summary>
    /// 확률 값을 기반으로 랜덤으로 ItemInfo를 반환해주는 함수
    /// </summary>
    public ItemInfo GetRandomItemData()
    {
        //확률에 따라 랜덤 등급 선택
        ItemGrade grade = GetRandomGrade();

        //그 등급에서 랜덤으로 선택
        int num = Random.Range(0, ItemDict[grade].Count);
        return ItemDict[grade][num];
    }


    /// <summary>
    /// 확률 값을 기반으로 랜덤한 ItemGrade를 반환해주는 함수
    /// </summary>
    private ItemGrade GetRandomGrade()
    {
        float totalItemProb = itemProb_Common + itemProb_Uncommon + itemProb_Rare + itemProb_Epic + itemProb_Legendary;
        float randNum = Random.Range(0, totalItemProb);

        if (randNum < itemProb_Legendary)
            return ItemGrade.Legendary;
        else if (randNum < itemProb_Legendary + itemProb_Epic)
            return ItemGrade.Epic;
        else if (randNum < itemProb_Legendary + itemProb_Epic + itemProb_Rare)
            return ItemGrade.Rare;
        else if (randNum < itemProb_Legendary + itemProb_Epic + itemProb_Rare + itemProb_Uncommon)
            return ItemGrade.Uncommon;
        else
            return ItemGrade.Common;
    }
}
