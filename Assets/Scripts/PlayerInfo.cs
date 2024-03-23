using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �÷��̾��� �⺻ �ɷ�ġ
/// </summary>
[Serializable]
public struct BasePlayerInfo
{
    public float jumpForce;
    public float bulletRange;
    public float bulletSpeed;
    public float attackSpeed;
    public float zoomAttackSpeed;
    public float bulletMpCost;
    public float maxMp;
    public float regenMp;
    public float regenMpOverload;
}

public class PlayerInfo : HitTarget
{
    [Header("Player Info")]
    //mpBar Color �����
    public Image mpBarColor = null;

    //�÷��̾� �⺻ �ɷ�ġ
    [SerializeField]
    protected BasePlayerInfo basePlayerInfo;

    //�÷��̾� ����
    protected float curJumpForce = 1.0f;              //������
    protected float curBulletRange = 10.0f;           //�Ѿ� ��Ÿ�
    protected float curBulletSpeed = 1.0f;            //�Ѿ� �ӵ�
    protected float curAttackSpeed = 5.0f;            //�ʴ� ���� �߻� �ӵ�
    protected float curZoomAttackSpeed = 7.5f;
    protected float curBulletMpCost = 10.0f;          //�Ѿ� �� ���� �Ҹ�


    protected bool isOverload = false;

    //Mp
    protected float curMaxMp = 100.0f;
    //���� �����
    protected float curRegenMp = 10.0f;
    //�����Ͻ� ���� �����
    protected float curRegenMpOverload = 50.0f;
    float curMp = 100.0f;
    protected float CurMp
    {
        get { return curMp; }
        set
        {
            if (value >= curMaxMp)
            {
                curMp = curMaxMp;

                //������ ����
                MpReleaseOverload();
            }
            else if (value <= 0)
            {
                curMp = 0;

                //+ ������ 0�� ���� ���, ������?
                MpOverload();
            }
            else
                curMp = value;

            if (mpBarSlider != null)
            {
                mpBarSlider.value = curMp / curMaxMp;
            }
            if (mpBarText != null)
            {
                mpBarText.text = $"{curMp:F1} / {curMaxMp:F1}";
            }
        }
    }

    
    Color originMpBarColor = Color.white;

    protected override void Initialize()
    {
        base.Initialize();

        curJumpForce = basePlayerInfo.jumpForce;
        curBulletRange = basePlayerInfo.bulletRange;
        curBulletSpeed = basePlayerInfo.bulletSpeed;
        curAttackSpeed = basePlayerInfo.attackSpeed;
        curZoomAttackSpeed = basePlayerInfo.zoomAttackSpeed;
        curBulletMpCost = basePlayerInfo.bulletMpCost;
        curMaxMp = basePlayerInfo.maxMp;
        curRegenMp = basePlayerInfo.regenMp;
        curRegenMpOverload = basePlayerInfo.regenMpOverload;

        curMp = curMaxMp;
    }


    /// <summary>
    /// ���� �����Ͻ� ����Ǵ� �Լ�.
    /// (������ 0�� �� ���, ���� ������, ���ݺҰ�, mpȸ���� ����)
    /// </summary>
    protected void MpOverload()
    {
        if(!isOverload)
        {
            originMpBarColor = mpBarColor.color;
            mpBarColor.color = new Color(0.3f, 0.3f, 0.3f);
            isOverload = true;
        }
    }

    /// <summary>
    /// ���� ������ ������ ����Ǵ� �Լ�.
    /// (�⺻ ������ 100�� �� ���, ����)
    /// </summary>
    protected void MpReleaseOverload()
    {
        if(isOverload)
        {
            mpBarColor.color = originMpBarColor;
            isOverload = false;
        }
    }

    /// <summary>
    /// ü�� ���
    /// </summary>
    protected void RegenerateHp(float deltaTime)
    {
        if (IsLive)
            CurHp += curRegenHp * deltaTime;
    }
    /// <summary>
    /// ���� ���
    /// </summary>
    protected void RegenerateMp(float deltaTime)
    {
        if (IsLive)
        {
            if (!isOverload)
            {
                CurMp += curRegenMp * deltaTime;
            }
            else
            {
                CurMp += curRegenMpOverload * deltaTime;
            }
        }
    }

    //--------------������ --------------------

    //���� ������
    const int MAX_EQUIPPED_ITEM_COUNT = 24;     //���� ������ ������ �ִ� ��

    [SerializeField]
    List<ItemInfo> equippedItems;

    protected void GetItem(Item item)
    {
        if(equippedItems.Count < MAX_EQUIPPED_ITEM_COUNT)
        {
            equippedItems.Add(item.ItemInfo);
            GameManager.inst.uiManager.playerInfoUI.ChangeItemData(equippedItems);
            ChangePlayerCurInfo();

            item.DestroyItem();
        }
    }

    /// <summary>
    /// �÷��̾� ���� ���� �ϴ� �Լ�.(������ ȹ�� ��, ������ �ٲ� �� ȣ��)
    /// </summary>
    private void ChangePlayerCurInfo()
    {
        float attackPointAdd = 0.0f;
        float attackPointMul = 1.0f;
        float attackSpeedMul = 1.0f;
        float zoomAttackSpeedMul = 1.0f;
        float moveSpeedMul = 1.0f;
        float bulletMpCostMul = 1.0f;
        float regenMpMul = 1.0f;
        float regenMpOverloadMul = 1.0f;

        float maxHpAdd = 0.0f;
        float maxHpMul = 1.0f;

        foreach (ItemInfo item in equippedItems)
        {
            attackPointAdd += item.DmgAdd;
            attackPointMul *= item.DmgMul;
            attackSpeedMul *= item.AttackSpeedMul;
            zoomAttackSpeedMul *= item.ZoomAttackSpeedMul;
            moveSpeedMul *= item.MovementSpeedMul;
            bulletMpCostMul *= item.BulletMpCostMul;
            regenMpMul *= item.RegenMpMul;
            regenMpOverloadMul *= item.RegenMpOverloadMul;
            maxHpAdd += item.MaxHpAdd;
            maxHpMul *= item.MaxHpMul;
        }

        curAttackPoint = (baseCharInfo.attackPoint + attackPointAdd) * attackPointMul;
        curAttackSpeed = basePlayerInfo.attackSpeed * attackSpeedMul;
        curZoomAttackSpeed = basePlayerInfo.zoomAttackSpeed * zoomAttackSpeedMul;
        curMoveSpeed = baseCharInfo.moveSpeed * moveSpeedMul;
        curBulletMpCost = basePlayerInfo.bulletMpCost * bulletMpCostMul;
        curRegenMp = basePlayerInfo.regenMp * regenMpMul;
        curRegenMpOverload = basePlayerInfo.regenMpOverload * regenMpOverloadMul;

        //ü��(���� ü�¿� ������ �ֹǷ� �߰� ���)
        float prevMaxHp = curMaxHp;
        curMaxHp = (baseCharInfo.maxHp + maxHpAdd) * maxHpMul;
        if (curMaxHp - prevMaxHp >= 0.0f)
            CurHp += curMaxHp - prevMaxHp;
    }



    /// <summary>
    /// (����׿�)
    /// �÷��̾� Data�� string���� �ٲ��ִ� �Լ�.
    /// </summary>
    public string GetPlayerInfoData()
    {
        return $"AttackPoint : {curAttackPoint} \n" +
            $"AttackSpeed : {curAttackSpeed} \n" +
            $"ZoomAttackSpeed : {curZoomAttackSpeed} \n" +
            $"BulletSpeed : {curBulletSpeed} \n" +
            $"BulletRange : {curBulletRange} \n" +
            $"BulletCost : {curBulletMpCost} \n" +
            $"MaxHp : {curMaxHp} \n" +
            $"MaxMp : {curMaxMp} \n" +
            $"RegenateHp : {curRegenHp} \n" +
            $"RegenateMp : {curRegenMp} \n" +
            $"RegenateMpOverload : {curRegenMpOverload} \n" +
            $"MoveSpeed : {curMoveSpeed} \n" +
            $"JumpForce : {curJumpForce}";
    }
}
