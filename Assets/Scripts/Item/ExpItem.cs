﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpItem : ItemBase {

    public override void OnInit()
    {
        base.OnInit();
        transform.localScale = new Vector3(2, 2, 2);
        transform.localRotation = Quaternion.Euler(-90, 0, 0);
    }

    public override bool OnAttachHero(FighterHero hero)
    {
        //CombatMgr.It.Cure(hero, 10);
        hero.AddExp(1);
        return true;
    }
}
