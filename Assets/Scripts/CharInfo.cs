using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ĳ������ �⺻ �ɷ�ġ
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
    //ĳ����(�÷��̾�, ���� ��) ���� ����
    [Header("Character Info")]
    
    public Slider hpBarSlider = null;
    public TextMeshProUGUI hpBarText = null;
    public Slider mpBarSlider = null;
    public TextMeshProUGUI mpBarText = null;
    [SerializeField]
    Image bloodScreen = null;

    [SerializeField]
    protected BaseCharInfo baseCharInfo;

    //ĳ���Ͱ� ��� �ִ���
    public bool IsLive
    {
        get { return curHp != 0; }
    }

    //�̵��ӵ�
    protected float curMoveSpeed = 1.0f;
    //���ݷ�
    protected float curAttackPoint = 10.0f;

    //Hp
    protected float curMaxHp = 100.0f;
    //ü�� �����
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

                //+ ĳ���� ����
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

            //hpBar ����
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

    //�Ѿ� ��Ÿ��(��Ÿ���� (1/���ݼӵ�)���� Ŀ���� ���� ����)
    protected float bulletCoolTime = 0.0f;

    /// <summary>
    /// CharInfo ���� �ʱ�ȭ
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
