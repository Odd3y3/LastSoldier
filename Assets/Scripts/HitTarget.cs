using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class HitTarget : CharInfo
{
    //�������� �¾�����, hpBar ǥ��
    protected bool isAttacked = false;

    Collider _col = null;
    public Collider Col
    {
        get
        {
            if(_col == null)
            {
                _col = GetComponent<Collider>();
                if(_col == null)
                {
                    _col = GetComponentInChildren<Collider>();
                }
            }
            return _col;
        }
    }

    public void Damaged(float dmg)
    {
        if (IsLive)
        {
            if(!isAttacked)
            {
                isAttacked = true;
                if(hpBarSlider.TryGetComponent(out EnemyHpBar bar))
                {
                    bar.ShowHpBar();
                }
            }
            PlayDamagedSound();
            CurHp -= dmg;
        }
    }
    public void Damaged(float dmg, Vector3 hitPos)
    {
        //Enemy�� ������ �޾��� ��,
        Damaged(dmg);
        DamagePopupUI.CreateDmgPopup(dmg, hitPos);
        GameManager.inst.inGameManager.Score += (int)dmg;
    }

    virtual protected void PlayDamagedSound()
    {

    }
}
