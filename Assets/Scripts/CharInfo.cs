using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 캐릭터의 기본 능력치
/// </summary>
[Serializable]
public struct BaseCharInfo
{
    public float maxHp;
    public float moveSpeed;
    public float attackPoint;
    public float regenHp;
}

public class CharInfo : MonoBehaviour
{
    //캐릭터(플레이어, 몬스터 등) 공통 정보
    [Header("Character Info")]
    
    public Slider hpBarSlider = null;
    public TextMeshProUGUI hpBarText = null;
    public Slider mpBarSlider = null;
    public TextMeshProUGUI mpBarText = null;
    [SerializeField]
    Image bloodScreen = null;

    [SerializeField]
    protected BaseCharInfo baseCharInfo;

    //캐릭터가 살아 있는지
    public bool IsLive
    {
        get { return curHp != 0; }
    }

    //이동속도
    protected float curMoveSpeed = 1.0f;
    //공격력
    protected float curAttackPoint = 10.0f;

    //Hp
    protected float curMaxHp = 100.0f;
    //체력 재생률
    protected float curRegenHp = 1.0f;
    float curHp = 100.0f;
    protected float CurHp
    {
        get { return curHp; }
        set
        {
            if (value >= curMaxHp) curHp = curMaxHp;
            else if (value <= 0)
            {
                curHp = 0;

                //+ 캐릭터 죽음
                Dead();
            }
            else
            {
                curHp = value;
            }

            //bloodScreen
            if(bloodScreen != null)
            {
                bloodScreen.color = new Color(0.5f, 0.5f, 0.5f, 1f - (curHp / curMaxHp));
            }

            //hpBar 갱신
            if(hpBarSlider != null)
            {
                hpBarSlider.value = curHp / curMaxHp;
            }
            if(hpBarText != null)
            {
                hpBarText.text = $"{curHp:F1} / {curMaxHp:F1}";
            }
        }
    }

    //총알 쿨타임(쿨타임이 (1/공격속도)보다 커지면 공격 가능)
    protected float bulletCoolTime = 0.0f;

    /// <summary>
    /// CharInfo 정보 초기화
    /// </summary>
    virtual protected void Initialize()
    {
        curMoveSpeed = baseCharInfo.moveSpeed;
        curAttackPoint = baseCharInfo.attackPoint;
        curMaxHp = baseCharInfo.maxHp;
        curRegenHp = baseCharInfo.regenHp;

        curHp = curMaxHp;
    }

    virtual protected void Dead()
    {
        //Debug.Log($"{gameObject.name} Dead");
    }
}
