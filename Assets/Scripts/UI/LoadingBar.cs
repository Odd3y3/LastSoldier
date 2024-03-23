using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LoadingBar : MonoBehaviour
{
    public UnityEvent OnComplete = null;

    public void OnCompleteLoad()
    {
        OnComplete?.Invoke();
    }
}
