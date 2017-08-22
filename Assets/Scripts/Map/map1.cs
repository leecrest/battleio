using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class map1 : MapBase
{
    public override void OnStart()
    {
        base.OnStart();
        // 创建主角，并将镜头绑定到主角头上
        FighterHero obj = CombatMgr.It.AddHero(true, 1, 0, 0, 0, "哈哈", 10, 100);
        obj.ChangeWeapon(1);
        for (int i = 0; i < 20; i++)
        {
            obj = CombatMgr.It.AddHero(false, 1, Random.Range(-20f,20f), Random.Range(-20f,20f),
                0, "吼吼", 50, 100);
            obj.InitAI(true);
            obj.ChangeWeapon(-1);
        }

        for (int i = 0; i < 30; i++)
        {
            ResMgr.It.CreateItem(1, Random.Range(-20f, 20f), Random.Range(-20f, 20f));
        }
    }
}
