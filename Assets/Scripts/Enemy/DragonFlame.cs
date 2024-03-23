using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonFlame : MonoBehaviour
{
    ParticleSystem ps;

    float flameDmg = 0.0f;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if(other.TryGetComponent<HitTarget>(out HitTarget player))
        {
            player.Damaged(flameDmg);
        }
    }

    public void SetFlameDmg(float flameDmg)
    {
        this.flameDmg = flameDmg;
    }
}
