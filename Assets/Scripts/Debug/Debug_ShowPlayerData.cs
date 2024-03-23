using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Debug_ShowPlayerData : MonoBehaviour
{
    TextMeshProUGUI text;

    [SerializeField]
    PlayerInfo player;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        text.text = player.GetPlayerInfoData();
    }
}
