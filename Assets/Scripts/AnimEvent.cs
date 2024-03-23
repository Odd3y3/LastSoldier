using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimEvent : MonoBehaviour
{
    public UnityEvent attackEvent = null;
    public UnityEvent playAttackSoundEvent = null;
    public UnityEvent playAttackSoundEvent2 = null;

    public void OnAttack()
    {
        attackEvent?.Invoke();
    }

    public void OnPlayAttackSound()
    {
        playAttackSoundEvent?.Invoke();
    }

    public void OnPlayAttackSound2()
    {
        playAttackSoundEvent2?.Invoke();
    }
}
