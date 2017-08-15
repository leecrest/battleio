using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 测试关卡
public class BattleTest : Battle
{
    public override void BattleStart()
    {
        base.BattleStart();
        // 创建主角，并将镜头绑定到主角头上
        FighterHero obj = AddHero(true, NewFid(), 0, 0, 0);
        for (int i = 0; i < 1; i++)
        {
            obj = AddHero(false, NewFid(), 0, Random.Range(-10f,10f), Random.Range(-10f,10f));
            obj.InitAI(true);
        }
    }
}
