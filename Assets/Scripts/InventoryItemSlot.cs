using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [SerializeField]
    GameObject iconObj;
    [SerializeField]
    Image frameObj;
    [SerializeField]
    Image imageObj;

    ItemInfo curItem = null;

    const float ITEM_SLOT_UI_OFFSET_X = 50.0f;

    public void ChangeItem(ItemInfo item)
    {
        if(item == null)
        {
            curItem = null;
            iconObj.SetActive(false);
            return;
        }
        else
        {
            curItem = item;
            iconObj.SetActive(true);

            imageObj.sprite = item.Icon;
            switch (item.Grade)
            {
                case ItemGrade.Common:
                    frameObj.color = Color.white;
                    break;
                case ItemGrade.Uncommon:
                    frameObj.color = Color.green;
                    break;
                case ItemGrade.Rare:
                    frameObj.color = Color.cyan;
                    break;
                case ItemGrade.Epic:
                    frameObj.color = Color.magenta;
                    break;
                case ItemGrade.Legendary:
                    frameObj.color = Color.yellow;
                    break;
            }
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(curItem != null)
        {
            GameManager.inst.uiManager.itemSlotInfoUI.Show(curItem);
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if(curItem != null )
        {
            RectTransform rect = GameManager.inst.uiManager.itemSlotInfoUI.GetComponent<RectTransform>();
            
            rect.position = eventData.position;
            rect.anchoredPosition += new Vector2(rect.rect.width * 0.5f + ITEM_SLOT_UI_OFFSET_X, 0);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.inst.uiManager.itemSlotInfoUI.Hide();
    }

}
