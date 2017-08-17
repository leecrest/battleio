﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 战斗状态
public enum EnCombatState
{
    Ready = 0,
    Start,
    Over,
}




public class CONST
{
    // 角色朝向偏移角度
    public static readonly float CHAR_FACE_OFFSET = 90f;
    // 镜头距离地面高度
    public static readonly float CAMERA_Z = 5f;
    // 观察时，镜头的移动速度
    public static readonly float CAMERA_SPEED = 0.15f;
}