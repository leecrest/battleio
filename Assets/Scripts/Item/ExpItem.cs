using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpItem : ItemBase {

    public override void OnInit()
    {
        base.OnInit();
        transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        transform.localRotation = Quaternion.Euler(45, 90, 45);
    }

    public override bool OnAttachHero(FighterHero hero)
    {
        CombatMgr.It.Cure(hero, 10);
        return true;
    }
}
