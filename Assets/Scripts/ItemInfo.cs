using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ���
/// </summary>
public enum ItemGrade
{
    Common,     //�Ϲ�
    Uncommon,   //���
    Rare,       //���
    Epic,       //����
    Legendary   //������
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Object/Item Data", order = int.MaxValue)]
public class ItemInfo : ScriptableObject
{
    //������ ������ȣ
    [SerializeField]
    private int itemID = 0;
    public int ItemID { get { return itemID; } }
    
    //������ ������
    [SerializeField]
    private Sprite icon;
    public Sprite Icon { get { return icon; } }

    //������ ���
    [SerializeField]
    private ItemGrade grade;
    public ItemGrade Grade { get {  return grade; } }

    //������ �̸�
    [SerializeField]
    private string itemName;
    public string ItemName { get { return itemName; } }

    //������ ����
    [SerializeField, TextArea(5,8)]
    private string itemDescription;
    public string ItemDescription { get {  return itemDescription; } }

    //---------������ �ɷ�ġ-----------
    //Add�� �տ���, Mul�� ������
    //���ݷ�
    [SerializeField]
    private float dmgAdd = 0f;
    public float DmgAdd { get {  return dmgAdd; } }
    
    [SerializeField]
    private float dmgMul = 1.0f;
    public float DmgMul { get { return dmgMul; } }

    //�̵��ӵ�
    [SerializeField]
    private float movementSpeedMul = 1.0f;
    public float MovementSpeedMul { get { return movementSpeedMul; } }

    //���ݼӵ�(���� �ӵ�)
    [SerializeField]
    private float attackSpeedMul = 1.0f;
    public float AttackSpeedMul { get { return attackSpeedMul; } }
    
    //Zoom�� ���ݼӵ�(���� �ӵ�)
    [SerializeField]
    private float zoomAttackSpeedMul = 1.0f;
    public float ZoomAttackSpeedMul { get { return zoomAttackSpeedMul; } }

    //�ִ� ü��
    [SerializeField]
    private float maxHpAdd = 0.0f;
    public float MaxHpAdd { get { return maxHpAdd; } }
    [SerializeField]
    private float maxHpMul = 1.0f;
    public float MaxHpMul { get { return maxHpMul; } }

    //���� �ڽ�Ʈ
    [SerializeField]
    private float bulletMpCostMul = 1.0f;
    public float BulletMpCostMul { get { return bulletMpCostMul; } }

    //���� ���
    [SerializeField]
    private float regenMpMul = 1.0f;
    public float RegenMpMul {  get { return regenMpMul; } }
    [SerializeField]
    private float regenMpOverloadMul = 1.0f;
    public float RegenMpOverloadMul { get { return regenMpOverloadMul; } }
}
