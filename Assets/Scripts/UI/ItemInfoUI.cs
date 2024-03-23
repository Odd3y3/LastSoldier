using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemInfoUI : MonoBehaviour
{
    [SerializeField]
    private ItemInfo itemInfo;
    [SerializeField]
    private float descriptioinYOffset = 155.0f;

    [Header("Link")]
    public GameObject infoObject;
    public TextMeshProUGUI itemGradeText;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public Image itemIconFrame;
    public Image itemIconComponent;
    public RectTransform descriptionTransform;

    RectTransform rect;
    Transform itemPos;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        //if(itemInfo != null)
        //{
        //    itemNameText.text = itemInfo.ItemName;
        //    itemDescriptionText.text = itemInfo.ItemDescription;
        //    itemIconComponent.sprite = itemInfo.Icon;
        //}
    }

    private void Update()
    {
        //Text에 맞춰 ItemInfoUI 창 크기 변경
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, itemDescriptionText.bounds.size.y + descriptioinYOffset);
    }

    private void FixedUpdate()
    {
        //Item 위치에 UI가 생성되도록.
        if(itemPos != null)
        {
            transform.position = Vector3.Lerp(transform.position,
                Camera.main.WorldToScreenPoint(itemPos.position + Vector3.up), 0.8f);
        }
    }

    Coroutine delayCoroutine = null;

    public void Show(ItemInfo itemInfo, Transform itemPos)
    {
        this.itemPos = itemPos;
        transform.position = Camera.main.WorldToScreenPoint(itemPos.position + Vector3.up);

        Show(itemInfo);
    }

    public void Show(ItemInfo itemInfo)
    {
        this.itemInfo = itemInfo;

        //item 정보
        itemNameText.text = itemInfo.ItemName;
        itemDescriptionText.text = itemInfo.ItemDescription;
        itemIconComponent.sprite = itemInfo.Icon;

        //item 등급
        switch (itemInfo.Grade)
        {
            case ItemGrade.Common:
                itemGradeText.text = "<일반>";
                itemGradeText.color = Color.white;
                itemNameText.color = Color.white;
                itemIconFrame.color = Color.white;
                break;
            case ItemGrade.Uncommon:
                itemGradeText.text = "<고급>";
                itemGradeText.color = Color.green;
                itemNameText.color = Color.green;
                itemIconFrame.color = Color.green;
                break;
            case ItemGrade.Rare:
                itemGradeText.text = "<희귀>";
                itemGradeText.color = Color.cyan;
                itemNameText.color = Color.cyan;
                itemIconFrame.color = Color.cyan;
                break;
            case ItemGrade.Epic:
                itemGradeText.text = "<에픽>";
                itemGradeText.color = Color.magenta;
                itemNameText.color = Color.magenta;
                itemIconFrame.color = Color.magenta;
                break;
            case ItemGrade.Legendary:
                itemGradeText.text = "<레전드>";
                itemGradeText.color = Color.yellow;
                itemNameText.color = Color.yellow;
                itemIconFrame.color = Color.yellow;
                break;
            default:
                break;
        }

        //딜레이 주기? (보류)
        //delayCoroutine = StartCoroutine(Delay(0.2f, () => { infoObject.SetActive(true); })) ;
        infoObject.SetActive(true);
    }

    //info창 딜레이 생성 (보류)
    IEnumerator Delay(float time, UnityAction action)
    {
        yield return new WaitForSeconds(time);
        action.Invoke();
    }

    public void Hide()
    {
        if(delayCoroutine != null)
            StopCoroutine(delayCoroutine);
        delayCoroutine = null;
        infoObject.SetActive(false);
    }

}
