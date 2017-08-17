using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelHome : PanelBase {
    public Text m_Notify;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            UIMgr.It.HidePanel(0);
            UIMgr.It.ShowPanel(1);
        }
    }

    public override void Show()
    {
        CombatMgr.It.CombatReady();
        base.Show();
        m_Notify.text = "PanelHome 点击开始";
    }
}
