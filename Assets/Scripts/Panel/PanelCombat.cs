using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelCombat : PanelBase
{

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
    }
}
