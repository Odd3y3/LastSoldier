using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingTips : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI tipsText = null;

    [SerializeField]
    List<string> randomTips = new List<string>();

    private void Start()
    {
        int idx = Random.Range(0, randomTips.Count);
        tipsText.text = randomTips[idx];
    }
}
