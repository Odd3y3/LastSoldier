using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eye : Enemy
{
    [SerializeField]
    GameObject projectilePrefab = null;
    [SerializeField]
    Transform bulletSpawnPos = null;

    [SerializeField]
    float bulletRange = 15.0f;
    [SerializeField]
    float bulletSpeed = 10.0f;

    protected override void Attack()
    {
        base.Attack();

        anim.SetBool("Attack1", true);
    }

    public override void OnAttack()
    {
        base.OnAttack();

        if(projectilePrefab != null)
        {
            //GameObject proj = Instantiate(projectilePrefab);
            GameObject proj = ObjectPool.inst.GetObject(projectilePrefab.name, projectilePrefab, GameManager.inst.inGameManager.particleRoot);

            proj.transform.position = bulletSpawnPos.position;

            proj.transform.LookAt(GameManager.inst.player.transform.position + Vector3.up * 1.3f);

            if (proj.TryGetComponent(out Bullet bullet))
            {
                BulletData data = new BulletData();
                data.typeName = projectilePrefab.name;
                data.damage = baseCharInfo.attackPoint;
                data.targetMask = playerMask;
                data.range = bulletRange;
                data.speed = bulletSpeed;
                bullet.Fire(data, bulletSpawnPos);
            }
        }
    }

}
