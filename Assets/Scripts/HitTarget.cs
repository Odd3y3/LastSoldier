using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class HitTarget : CharInfo
{
    //데미지를 맞았으면, hpBar 표시
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
        //Enemy가 데미지 받았을 때,
        Damaged(dmg);
        DamagePopupUI.CreateDmgPopup(dmg, hitPos);
        GameManager.inst.inGameManager.Score += (int)dmg;
    }

    virtual protected void PlayDamagedSound()
    {

    }
}
