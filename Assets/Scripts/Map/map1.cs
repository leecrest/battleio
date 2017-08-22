using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class map1 : MapBase
{
    public override void OnStart()
    {
        base.OnStart();
        // 创建主角，并将镜头绑定到主角头上
        FighterHero obj = CombatMgr.It.AddHero(true, 0, 1, 0, 0);
        obj.ChangeWeapon(1);
        for (int i = 0; i < 1; i++)
        {
            obj = CombatMgr.It.AddHero(false, 0, 1, Random.Range(-10f,10f), Random.Range(-10f,10f));
            obj.InitAI(true);
            obj.ChangeWeapon(-1);
        }
    }
}
