using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// ������ Ŭ����.
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
    /// ������ �����͸� ������, �������� ���̰� ��.
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


    //Static �Լ�

    /// <summary>
    /// Item �����ϴ� �Լ�
    /// </summary>
    /// <returns>������ Instance�� Item Component</returns>
    static public Item CreateItem(Vector3 pos)
    {
        GameObject org = Resources.Load<GameObject>("Item");
        GameObject inst = ObjectPool.inst.GetObject("Item", org, GameManager.inst.inGameManager.itemsRoot);
        inst.transform.position = pos;
        
        if (inst != null && inst.TryGetComponent<Item>(out Item item))
        {
            //������ ItemData �ֱ�
            item.SetItemData(GameManager.inst.itemManager.GetRandomItemData());
            return item;
        }
        else
            return null;
    }
}