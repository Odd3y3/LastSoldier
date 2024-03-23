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
        //Text�� ���� ItemInfoUI â ũ�� ����
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, itemDescriptionText.bounds.size.y + descriptioinYOffset);
    }

    private void FixedUpdate()
    {
        //Item ��ġ�� UI�� �����ǵ���.
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

        //item ����
        itemNameText.text = itemInfo.ItemName;
        itemDescriptionText.text = itemInfo.ItemDescription;
        itemIconComponent.sprite = itemInfo.Icon;

        //item ���
        switch (itemInfo.Grade)
        {
            case ItemGrade.Common:
                itemGradeText.text = "<�Ϲ�>";
                itemGradeText.color = Color.white;
                itemNameText.color = Color.white;
                itemIconFrame.color = Color.white;
                break;
            case ItemGrade.Uncommon:
                itemGradeText.text = "<���>";
                itemGradeText.color = Color.green;
                itemNameText.color = Color.green;
                itemIconFrame.color = Color.green;
                break;
            case ItemGrade.Rare:
                itemGradeText.text = "<���>";
                itemGradeText.color = Color.cyan;
                itemNameText.color = Color.cyan;
                itemIconFrame.color = Color.cyan;
                break;
            case ItemGrade.Epic:
                itemGradeText.text = "<����>";
                itemGradeText.color = Color.magenta;
                itemNameText.color = Color.magenta;
                itemIconFrame.color = Color.magenta;
                break;
            case ItemGrade.Legendary:
                itemGradeText.text = "<������>";
                itemGradeText.color = Color.yellow;
                itemNameText.color = Color.yellow;
                itemIconFrame.color = Color.yellow;
                break;
            default:
                break;
        }

        //������ �ֱ�? (����)
        //delayCoroutine = StartCoroutine(Delay(0.2f, () => { infoObject.SetActive(true); })) ;
        infoObject.SetActive(true);
    }

    //infoâ ������ ���� (����)
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
