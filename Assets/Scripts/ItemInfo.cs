using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 등급
/// </summary>
public enum ItemGrade
{
    Common,     //일반
    Uncommon,   //고급
    Rare,       //희귀
    Epic,       //에픽
    Legendary   //레전드
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Object/Item Data", order = int.MaxValue)]
public class ItemInfo : ScriptableObject
{
    //아이템 고유번호
    [SerializeField]
    private int itemID = 0;
    public int ItemID { get { return itemID; } }
    
    //아이템 아이콘
    [SerializeField]
    private Sprite icon;
    public Sprite Icon { get { return icon; } }

    //아이템 등급
    [SerializeField]
    private ItemGrade grade;
    public ItemGrade Grade { get {  return grade; } }

    //아이템 이름
    [SerializeField]
    private string itemName;
    public string ItemName { get { return itemName; } }

    //아이템 설명
    [SerializeField, TextArea(5,8)]
    private string itemDescription;
    public string ItemDescription { get {  return itemDescription; } }

    //---------아이템 능력치-----------
    //Add는 합연산, Mul은 곱연산
    //공격력
    [SerializeField]
    private float dmgAdd = 0f;
    public float DmgAdd { get {  return dmgAdd; } }
    
    [SerializeField]
    private float dmgMul = 1.0f;
    public float DmgMul { get { return dmgMul; } }

    //이동속도
    [SerializeField]
    private float movementSpeedMul = 1.0f;
    public float MovementSpeedMul { get { return movementSpeedMul; } }

    //공격속도(연사 속도)
    [SerializeField]
    private float attackSpeedMul = 1.0f;
    public float AttackSpeedMul { get { return attackSpeedMul; } }
    
    //Zoom시 공격속도(연사 속도)
    [SerializeField]
    private float zoomAttackSpeedMul = 1.0f;
    public float ZoomAttackSpeedMul { get { return zoomAttackSpeedMul; } }

    //최대 체력
    [SerializeField]
    private float maxHpAdd = 0.0f;
    public float MaxHpAdd { get { return maxHpAdd; } }
    [SerializeField]
    private float maxHpMul = 1.0f;
    public float MaxHpMul { get { return maxHpMul; } }

    //마나 코스트
    [SerializeField]
    private float bulletMpCostMul = 1.0f;
    public float BulletMpCostMul { get { return bulletMpCostMul; } }

    //마나 재생
    [SerializeField]
    private float regenMpMul = 1.0f;
    public float RegenMpMul {  get { return regenMpMul; } }
    [SerializeField]
    private float regenMpOverloadMul = 1.0f;
    public float RegenMpOverloadMul { get { return regenMpOverloadMul; } }
}
