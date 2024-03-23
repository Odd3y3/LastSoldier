using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어의 기본 능력치
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
    //mpBar Color 변경용
    public Image mpBarColor = null;

    //플레이어 기본 능력치
    [SerializeField]
    protected BasePlayerInfo basePlayerInfo;

    //플레이어 정보
    protected float curJumpForce = 1.0f;              //점프력
    protected float curBulletRange = 10.0f;           //총알 사거리
    protected float curBulletSpeed = 1.0f;            //총알 속도
    protected float curAttackSpeed = 5.0f;            //초당 공격 발사 속도
    protected float curZoomAttackSpeed = 7.5f;
    protected float curBulletMpCost = 10.0f;          //총알 당 마나 소모량


    protected bool isOverload = false;

    //Mp
    protected float curMaxMp = 100.0f;
    //마나 재생률
    protected float curRegenMp = 10.0f;
    //과부하시 마나 재생률
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

                //과부하 해제
                MpReleaseOverload();
            }
            else if (value <= 0)
            {
                curMp = 0;

                //+ 마나가 0이 됬을 경우, 과부하?
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
    /// 마나 과부하시 실행되는 함수.
    /// (마나가 0이 될 경우, 마나 과부하, 공격불가, mp회복률 증가)
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
    /// 마나 과부하 해제시 실행되는 함수.
    /// (기본 마나가 100이 될 경우, 해제)
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
    /// 체력 재생
    /// </summary>
    protected void RegenerateHp(float deltaTime)
    {
        if (IsLive)
            CurHp += curRegenHp * deltaTime;
    }
    /// <summary>
    /// 마나 재생
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

    //--------------아이템 --------------------

    //보유 아이템
    const int MAX_EQUIPPED_ITEM_COUNT = 24;     //보유 가능한 아이템 최대 수

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
    /// 플레이어 정보 갱신 하는 함수.(아이템 획득 등, 정보가 바뀔 때 호출)
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

        //체력(현재 체력에 영향을 주므로 추가 계산)
        float prevMaxHp = curMaxHp;
        curMaxHp = (baseCharInfo.maxHp + maxHpAdd) * maxHpMul;
        if (curMaxHp - prevMaxHp >= 0.0f)
            CurHp += curMaxHp - prevMaxHp;
    }



    /// <summary>
    /// (디버그용)
    /// 플레이어 Data를 string으로 바꿔주는 함수.
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
