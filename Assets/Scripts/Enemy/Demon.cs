using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demon : Enemy
{
    //공격 범위, 위치 링크
    [SerializeField]
    Transform attackArea = null;

    protected override void Attack()
    {
        base.Attack();

        int randValue = Random.Range(0, 3);
        anim.SetBool("Attack1", false);
        anim.SetBool("Attack2", false);
        anim.SetBool("Attack3", false);
        switch (randValue)
        {
            case 0:
                anim.SetBool("Attack1", true);
                break;
            case 1:
                anim.SetBool("Attack2", true);
                break;
            case 2:
                anim.SetBool("Attack3", true);
                break;
        }
    }

    public override void OnAttack()
    {
        base.OnAttack();

        Collider[] cols = Physics.OverlapSphere(attackArea.position, 1f, playerMask);
        foreach (Collider col in cols)
        {
            if (col.TryGetComponent(out HitTarget player))
            {
                player.Damaged(baseCharInfo.attackPoint);
            }
        }
    }
}
