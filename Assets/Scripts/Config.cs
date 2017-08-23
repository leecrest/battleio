using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapConfig
{
    public int id;
    public string name;
    public string prefab;
}

public class HeroConfig
{
    public int id;
    public string name;
    public string prefab;
}

public class WeaponConfig
{
    public int id;
    public string name;
    public string prefab;
    public int shell;
    public float atktime;       //攻击前摇
}

public class ShellConfig
{
    public int id;
    public string prefab;
    public string hole;
    public float speed;         // 基础速度
    public float range;         // 基础射程
}

public class ItemConfig
{
    public int id;
    public float y;
    public string prefab;
}

public class Config
{
    // 开发说明
    public static readonly string VERSION = "" +
        "目前支持功能如下：\n\r" +
        "1、地图加载，支持多张地图\n\r" +
        "2、支持多种角色模型加载\n\r" +
        "3、支持弹道武器：弓箭\n\r" +
        "4、支持道具：经验点、回复点\n\r";

    #region 常量定义
    public static readonly float CAMERA_HEIGHT = 8f;    // 镜头距离地面高度
    public static readonly float CAMERA_SPEED = 0.15f;  // 观察时，镜头的移动速度
    public static readonly float SHELL_DESTROY = 1f;    // 子弹的销毁时间，单位：秒
    #endregion


    #region 地图配置
    public static readonly MapConfig[] MapCfg = {
        new MapConfig { id=1, name="测试", prefab="Map/map01" }
    };
    #endregion


    #region 玩家配置
    public static readonly HeroConfig[] HeroCfg = {
        new HeroConfig { id=1, name="测试模型", prefab="Hero/hero01" }
    };
    #endregion


    #region 武器配置
    public static readonly WeaponConfig[] WeaponCfg = {
        new WeaponConfig { id=1, name="弓箭", prefab="Weapon/arrow", shell=1, atktime=0.5f },
    };

    #endregion

    #region 子弹配置
    public static readonly ShellConfig[] ShellCfg = {
        new ShellConfig { id=1, prefab="Weapon/arrow_shell", speed=10f, range=5f, hole="Weapon/arrowshell_hole" },
    };
    #endregion


    #region 道具配置
    public static readonly ItemConfig[] ItemCfg = {
        new ItemConfig { id=1, prefab="Item/exp", y=1f },
        new ItemConfig { id=2, prefab="Item/heart", y=1f },
    };
    #endregion

    #region 每个等级的经验上限
    public static readonly int[] LevelExp = {
        10, 20, 30, 40, 50, 60, 70, 80, 90, 100,
        110, 120, 130, 140, 150, 160, 170, 180, 190, 200,
        210, 220, 230, 240, 250,
    };
    public static readonly int LevelMax = LevelExp.Length;
    #endregion

}
