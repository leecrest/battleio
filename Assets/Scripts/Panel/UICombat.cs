using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICombat : UIBase
{
    public Button m_BtnShoot;
    public Button m_BtnWeapon;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnShow()
    {
        base.OnShow();
        GameMgr.It.CombatStart();
        m_BtnShoot.onClick.AddListener(OnBtnShoot);
        m_BtnWeapon.onClick.AddListener(OnBtnWeapon);
    }

    public override void OnHide()
    {
        base.OnHide();
        m_BtnShoot.onClick.RemoveListener(OnBtnShoot);
        m_BtnWeapon.onClick.RemoveListener(OnBtnWeapon);
    }

    void OnBtnShoot()
    {
        GameMgr.It.OnHeroShoot();
    }

    void OnBtnWeapon()
    {
        GameMgr.It.GM_ChangeWeapon();
    }
}
