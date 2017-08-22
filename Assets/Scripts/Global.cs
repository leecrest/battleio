using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Global : MonoBehaviour {
    public static Global It;
    void Awake() { It = this; }




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
        UIMgr.It.OnUninit();
        ResMgr.It.OnUninit();
    }
}
