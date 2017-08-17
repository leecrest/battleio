using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : MonoBehaviour
{
    public static SoundMgr It;
    void Awake() { It = this; }

    public void OnInit()
    {
    }

    public void OnUninit()
    {

    }
}