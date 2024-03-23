using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DamagePopupUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI tmp;
    Vector3 hitPos;

    const string DMG_POPUP_TYPENAME = "dmgPopup";

    private void OnEnable()
    {
        //Destroy(gameObject, 1.0f);
        ObjectPool.inst.ReleaseObject(DMG_POPUP_TYPENAME, gameObject, 1.0f);
    }

    private void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(hitPos);
        if(screenPos.z >= 0)
            transform.position = screenPos;
        else
            transform.position = new Vector3(-100, -100);
    }


    public void SetDamage(float dmg)
    {
        if(tmp != null)
        {
            tmp.text = dmg.ToString("F0");
        }
    }

    public void SetHitPos(Vector3 hitPos)
    {
        this.hitPos = hitPos;
    }

    static public void CreateDmgPopup(float dmg, Vector3 hitPos)
    {
        //GameObject dmgPopupObj = Instantiate(Resources.Load<GameObject>("UI/DmgPopup"), GameManager.inst.uiManager.dmgPopupsObj);
        GameObject dmgPopupObj = ObjectPool.inst.GetObject(DMG_POPUP_TYPENAME,
            Resources.Load<GameObject>("UI/DmgPopup"), GameManager.inst.uiManager.dmgPopupsObj);

        DamagePopupUI dmgPopup = dmgPopupObj.GetComponent<DamagePopupUI>();
        if(dmgPopup != null)
        {
            dmgPopup.SetHitPos(hitPos);
            dmgPopup.SetDamage(dmg);
        }
    }
}
