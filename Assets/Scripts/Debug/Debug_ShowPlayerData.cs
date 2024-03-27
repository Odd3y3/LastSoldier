using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Debug_ShowPlayerData : MonoBehaviour
{
    TextMeshProUGUI text;

    [SerializeField]
    PlayerInfo player;


    PlayerInputActions playerInputActions;

    bool isShowPlayerData = false;


    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.enabled = false;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Debug.Enable();
        playerInputActions.Debug.PlayerData.performed += ShowDebugPlayerData;
    }
    private void Update()
    {
        text.text = player.GetPlayerInfoData();
    }

    private void ShowDebugPlayerData(InputAction.CallbackContext context)
    {
        if(isShowPlayerData)
        {
            text.enabled = false;
            isShowPlayerData = false;
        }
        else
        {
            text.enabled = true;
            isShowPlayerData = true;
        }
    }

    private void OnDestroy()
    {
        playerInputActions.Debug.Disable();
    }
}
