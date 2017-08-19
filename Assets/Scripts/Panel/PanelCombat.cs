using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelCombat : PanelBase
{
    public Button m_BtnShoot;
    public Button m_BtnWeapon;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Show()
    {
        base.Show();
        CombatMgr.It.CombatStart();
        m_BtnShoot.onClick.AddListener(OnBtnShoot);
        m_BtnWeapon.onClick.AddListener(OnBtnWeapon);
    }

    public override void Hide()
    {
        base.Hide();
        m_BtnShoot.onClick.RemoveListener(OnBtnShoot);
        m_BtnWeapon.onClick.RemoveListener(OnBtnWeapon);
    }

    void OnBtnShoot()
    {
        CombatMgr.It.OnHeroShoot();
    }

    void OnBtnWeapon()
    {
        CombatMgr.It.GM_ChangeWeapon();
    }
}
