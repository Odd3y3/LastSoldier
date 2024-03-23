using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpBar : MonoBehaviour
{
    [SerializeField]
    GameObject[] childObjs;

    Slider slider;
    Transform target = null;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        HideHpBar();
    }

    private void FixedUpdate()
    {
        if(target != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position);
            if (screenPos.z > 0)
            {
                transform.position = screenPos;
                float scl = screenPos.z < 2f ? 3.5f : 7f / screenPos.z;
                transform.localScale = Vector3.one * scl;
                
            }
            else
                transform.position = new Vector3(-10000, -10000);
        }
    }

    public void SetTarget(Enemy target, Transform pos)
    {
        this.target = pos;
        
        target.hpBarSlider = slider;
    }

    public void ShowHpBar()
    {
        foreach(GameObject obj in childObjs)
        {
            obj.SetActive(true);
        }
    }

    public void HideHpBar()
    {
        foreach (GameObject obj in childObjs)
        {
            obj.SetActive(false);
        }
    }
}
