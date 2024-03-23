using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoUI : MonoBehaviour
{
    [SerializeField]
    private GameObject infoObj;
    [SerializeField]
    private List<InventoryItemSlot> itemSlots;

    public void Show()
    {
        infoObj.SetActive(true);
    }
    public void Hide()
    {
        infoObj.SetActive(false);
    }

    public void ChangeItemData(List<ItemInfo> equippedItems)
    {
        for(int i = 0; i < itemSlots.Count; ++i)
        {
            if(i < equippedItems.Count)
                itemSlots[i].ChangeItem(equippedItems[i]);
            else
                itemSlots[i].ChangeItem(null);
        }
    }
}
