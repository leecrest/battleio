using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelCombat : PanelBase
{
    public Button m_BtnSkill0;
    public Button m_BtnSkill1;
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
        m_BtnSkill0.onClick.AddListener(OnBtnSkill0);
        m_BtnSkill1.onClick.AddListener(OnBtnSkill1);
        m_BtnWeapon.onClick.AddListener(OnBtnWeapon);
    }

    public override void Hide()
    {
        base.Hide();
        m_BtnSkill0.onClick.RemoveListener(OnBtnSkill0);
        m_BtnSkill1.onClick.RemoveListener(OnBtnSkill1);
        m_BtnWeapon.onClick.RemoveListener(OnBtnWeapon);
    }

    void OnBtnSkill0()
    {
        CombatMgr.It.OnUseSkill(0);
    }

    void OnBtnSkill1()
    {
        CombatMgr.It.OnUseSkill(1);
    }

    void OnBtnWeapon()
    {
        CombatMgr.It.GM_ChangeWeapon();
    }
}
