using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 아이템 클래스.
/// </summary>
public class Item : MonoBehaviour
{
    Collider col = null;

    [SerializeField]
    private ItemInfo itemInfo;
    public ItemInfo ItemInfo { get { return itemInfo; } }

    [Header("Link")]
    public GameObject itemEffectCommon;
    public GameObject itemEffectUncommon;
    public GameObject itemEffectRare;
    public GameObject itemEffectEpic;
    public GameObject itemEffectLegendary;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        col.enabled = false;
        itemEffectCommon.SetActive(false);
        itemEffectUncommon.SetActive(false);
        itemEffectRare.SetActive(false);
        itemEffectEpic.SetActive(false);
        itemEffectLegendary.SetActive(false);
    }

    /// <summary>
    /// 아이템 데이터를 받으면, 아이템을 보이게 함.
    /// </summary>
    public void SetItemData(ItemInfo itemData)
    {
        itemInfo = itemData;
        switch (itemInfo.Grade)
        {
            case ItemGrade.Common:
                itemEffectCommon.SetActive(true);
                break;
            case ItemGrade.Uncommon:
                itemEffectUncommon.SetActive(true);
                break;
            case ItemGrade.Rare:
                itemEffectRare.SetActive(true);
                break;
            case ItemGrade.Epic:
                itemEffectEpic.SetActive(true);
                break;
            case ItemGrade.Legendary:
                itemEffectLegendary.SetActive(true);
                break;
            default:
                break;
        }

        col.enabled = true;
    }


    public void DestroyItem()
    {
        //Destroy(gameObject);
        ObjectPool.inst.ReleaseObject("Item", gameObject);
    }


    //Static 함수

    /// <summary>
    /// Item 생성하는 함수
    /// </summary>
    /// <returns>생성된 Instance의 Item Component</returns>
    static public Item CreateItem(Vector3 pos)
    {
        GameObject org = Resources.Load<GameObject>("Item");
        GameObject inst = ObjectPool.inst.GetObject("Item", org, GameManager.inst.inGameManager.itemsRoot);
        inst.transform.position = pos;
        
        if (inst != null && inst.TryGetComponent<Item>(out Item item))
        {
            //랜덤한 ItemData 넣기
            item.SetItemData(GameManager.inst.itemManager.GetRandomItemData());
            return item;
        }
        else
            return null;
    }
}