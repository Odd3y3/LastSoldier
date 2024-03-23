using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Enemy
{
    [SerializeField]
    ParticleSystem dragonFlame;
    //공격 범위, 위치 링크
    [SerializeField]
    Transform attackArea = null;

    int[] attackPattern = { 0, 0, 1, 2 };
    int curAttackIdx = 0;

    float baseAttackRange;
    float flameAttackRange;

    protected override void Initialize()
    {
        base.Initialize();

        baseAttackRange = AttackDetectRange;
        flameAttackRange = 7.0f;

        dragonFlame.GetComponent<DragonFlame>().SetFlameDmg(baseCharInfo.attackPoint / 10.0f);
    }

    protected override void Attack()
    {
        base.Attack();

        anim.SetBool("Attack1", false);
        anim.SetBool("Attack2", false);
        anim.SetBool("Attack3", false);
        switch (attackPattern[curAttackIdx])
        {
            case 0:
                LockRotate = false;
                anim.SetBool("Attack1", true);
                break;
            case 1:
                LockRotate = false;
                anim.SetBool("Attack2", true);
                break;
            case 2:
                anim.SetBool("Attack3", true);
                dragonFlame.Play();
                LockRotate = true;
                break;
        }

        curAttackIdx = (curAttackIdx + 1) % attackPattern.Length;
        if (attackPattern[curAttackIdx] == 2)
        {
            //브레스 공격일경우
            AttackDetectRange = flameAttackRange;
        }
        else
        {
            //아닐 경우
            AttackDetectRange = baseAttackRange;
        }
    }

    public override void OnAttack()
    {
        base.OnAttack();

        Collider[] cols = Physics.OverlapSphere(attackArea.position, 1.3f, playerMask);
        foreach (Collider col in cols)
        {
            if (col.TryGetComponent(out HitTarget player))
            {
                player.Damaged(baseCharInfo.attackPoint);
            }
        }
    }
}
