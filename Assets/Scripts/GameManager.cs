using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager inst = null;

    public SceneLoader sceneLoader = null;
    public ItemManager itemManager = null;
    public InGameManager inGameManager = null;
    public SoundManager soundManager = null;
    public UIManager uiManager = null;
    public PlayerInput player = null;
    

    private void Awake()
    {
        Time.timeScale = 1.0f;

        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    
}
