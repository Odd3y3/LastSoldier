using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public struct BulletData
{
    public string typeName;
    public float damage;
    public float speed;
    public float range;
    public LayerMask targetMask;
}

public class Bullet : MonoBehaviour
{
    BulletData bulletData;
    public LayerMask groundMask;
    public GameObject hitEffect;
    public GameObject flashEffect;
    [SerializeField]
    float bulletRadius = 0.1f;
    [SerializeField]
    bool useRaycast = true;

    //�浹�� ray�˻��Ҷ�,
    Vector3 prevBulletPos;

    bool isFire = false;
    bool isBulletOutOfRange = false;
    float curMoveDist = 0.0f;

    private void OnEnable()
    {
        isFire = false;
        isBulletOutOfRange = false;
        curMoveDist = 0.0f;
    }

    private void FixedUpdate()
    {
        if(isFire)
        {
            prevBulletPos = transform.position;
            float deltaMoveDist = bulletData.speed * Time.fixedDeltaTime;
            if(curMoveDist + deltaMoveDist >= bulletData.range)
            {
                deltaMoveDist = bulletData.range - curMoveDist;
                isBulletOutOfRange = true;
            }
            transform.position += transform.forward * deltaMoveDist;
            curMoveDist += deltaMoveDist;

            if(useRaycast && Physics.SphereCast(prevBulletPos, bulletRadius, transform.forward, out RaycastHit hit, deltaMoveDist,
                bulletData.targetMask | groundMask))
            {
                //bullet�� hit ���� ���
                Hit(hit.point, hit.collider);
            }
            else
            {
                //bullet�� hit���� �ʾ��� ���

                //bullet�� range������ ������ ��� ����
                if (isBulletOutOfRange)
                {
                    //Destroy(gameObject);
                    isFire = false;
                    ObjectPool.inst.ReleaseObject(bulletData.typeName, gameObject);
                }
            }
        }
    }

    private void Hit(Vector3 hitPoint, Collider hitCol)
    {
        //bullet�� hit ���� ���

        //hitEffect ����
        if (hitEffect != null)
        {
            //hit Effect ����
            //GameObject hitInst = Instantiate(hitEffect, hitPoint, transform.rotation);
            GameObject hitInst = ObjectPool.inst.GetObject(hitEffect.name, hitEffect, GameManager.inst.inGameManager.particleRoot);
            hitInst.transform.position = hitPoint;
            hitInst.transform.rotation = transform.rotation;

            ParticleSystem hitParticle = hitEffect.GetComponent<ParticleSystem>();
            if (hitParticle != null)
            {
                //Destroy(hitInst, hitParticle.main.duration);
                ObjectPool.inst.ReleaseObject(hitEffect.name, hitInst, hitParticle.main.duration);
            }
            else
            {
                //Destroy(hitInst, 5.0f);
                ObjectPool.inst.ReleaseObject(hitEffect.name, hitInst, 5.0f);
            }
        }

        //Damage ����
        if (((1 << hitCol.gameObject.layer) & bulletData.targetMask) != 0)
        {
            //target�� hit ���� ��,
            HitTarget target = hitCol.GetComponent<HitTarget>();
            if (target != null)
            {
                if(target is PlayerInfo)
                    target.Damaged(bulletData.damage);
                else
                    target.Damaged(bulletData.damage, hitPoint);
            }
        }

        //Destroy(gameObject);
        isFire = false;
        ObjectPool.inst.ReleaseObject(bulletData.typeName, gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!useRaycast && (((1 << other.gameObject.layer) & (bulletData.targetMask | groundMask)) != 0))
        {
            //���� hit�ϰų�, target�� hit�ϸ�
            Hit(transform.position, other);
        }
    }


    public void Fire(BulletData bulletData, Transform flashSpawnPos)
    {
        //�Ѿ� ������ �ް�, isFire = true�� �Ͽ� ������ �߻�
        this.bulletData = bulletData;
        if(flashEffect != null)
            Flash(flashSpawnPos);
        isFire = true;
    }

    void Flash(Transform flashSpawnPos)
    {
        //GameObject flashInst = Instantiate(flashEffect, flashSpawnPos.position, transform.rotation);
        GameObject flashInst = ObjectPool.inst.GetObject(flashEffect.name, flashEffect, GameManager.inst.inGameManager.particleRoot);
        flashInst.transform.position = flashSpawnPos.position;
        flashInst.transform.rotation = transform.rotation;

        ParticleSystem flashParticle = flashInst.GetComponent<ParticleSystem>();
        if(flashParticle != null)
        {
            //Destroy(flashInst, flashParticle.main.duration);
            ObjectPool.inst.ReleaseObject(flashEffect.name, flashInst, flashParticle.main.duration);
        }
        else
        {
            //Destroy(flashInst, 2.0f);
            ObjectPool.inst.ReleaseObject(flashEffect.name, flashInst, 2.0f);
        }
    }
}
