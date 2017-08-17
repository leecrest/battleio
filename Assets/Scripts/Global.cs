using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Global : MonoBehaviour {
    public static Global It;
    void Awake() { It = this; }


    #region 常量
    public GUISkin MySkin;
    public Texture2D TexBloodSelf;
    public Texture2D TexBloodFriend;    // 友方血条前景
    public Texture2D TexBloodEnemy;     // 敌方血条前景
    public Texture2D TexBloodBg;        // 血条背景
    #endregion




    void Start()
    {
        ResMgr.It.OnInit();
        UIMgr.It.OnInit();
        SoundMgr.It.OnInit();
        CombatMgr.It.OnInit();

        UIMgr.It.ShowPanel(0);
    }

    void Destroy()
    {
        CombatMgr.It.OnUninit();
        SoundMgr.It.OnUninit();
        ResMgr.It.OnUninit();
    }
}
