﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterHero : FighterBase
{
    [HideInInspector]
    public int ID;
    [HideInInspector]
    public string Name;

    [HideInInspector]
    public bool IsMain = false;
    [HideInInspector]
    public bool IsAI = false;
    [HideInInspector]
    public bool IsMoving = false;

    public Vector2 BloodSize = new Vector2(150, 20);
    public Vector2 NameSize = new Vector2(200, 50);
    [HideInInspector]
    public WeaponBase Weapon;
    [HideInInspector]
    public int CurHP = 50;
    [HideInInspector]
    public int MaxHP = 100;
    [HideInInspector]
    public float MoveSpeed = 10f;


    // 武器加成
    [HideInInspector]
    public int ShellCount = 0;
    [HideInInspector]
    public float ShellSpeed = 0f;
    [HideInInspector]
    public float ShellDistance = 0f;

    void OnGUI()
    {
        //得到NPC头顶在3D世界中的坐标
        //默认NPC坐标点在脚底下，所以这里加上npcHeight它模型的高度即可
        Vector3 worldPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.6f);
        //根据NPC头顶的3D坐标换算成它在2D屏幕中的坐标
        Vector2 position = Camera.main.WorldToScreenPoint(worldPosition);
        //得到真实NPC头顶的2D坐标
        position = new Vector2(position.x, Screen.height - position.y);
        //通过血值计算红色血条显示区域
        float blood_width = BloodSize.x * CurHP / MaxHP;
        //先绘制黑色血条
        GUI.DrawTexture(new Rect(position.x - BloodSize.x / 2, position.y - BloodSize.y, BloodSize.x, BloodSize.y),
            ResMgr.It.TexBloodBg);
        //在绘制红色血条
        GUI.DrawTexture(new Rect(position.x - BloodSize.x / 2, position.y - BloodSize.y, blood_width, BloodSize.y),
            IsMain ? ResMgr.It.TexBloodSelf : ResMgr.It.TexBloodFriend);

        //计算名称的宽高
        GUI.Label(new Rect(position.x - NameSize.x / 2, position.y - NameSize.y - BloodSize.y, NameSize.x, NameSize.y), 
            Name, ResMgr.It.MySkin.label);
    }

    public override void OnInit()
    {
        base.OnInit();
        Weapon = null;
    }

    public override void OnUninit()
    {
        base.OnUninit();
    }

    public void MoveStart()
    {
        if (IsMoving) return;
        IsMoving = true;
        //Animator anim = GetComponent<Animator>();
        //anim.Play("char_move");
    }

    public void MoveStop()
    {
        if (!IsMoving) return;
        IsMoving = false;
        //Animator anim = GetComponent<Animator>();
        //anim.Play("char_idle");
    }

    public void MoveBy(float x, float z)
    {
        float angle = Vector2.SignedAngle(Vector2.down, new Vector2(x, z));
        angle += 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector2.down);
        Vector3 pos = transform.position;
        if (Mathf.Approximately(x, 0f))
        {
            if (Mathf.Approximately(z, 0f)) return;
            pos.z -= z > 0f ? MoveSpeed : -MoveSpeed;
        }
        else if (Mathf.Approximately(z, 0f))
        {
            if (Mathf.Approximately(x, 0f)) return;
            pos.x -= x > 0f ? MoveSpeed : -MoveSpeed;
        }
        else
        {
            float delta = Mathf.Atan2(x, z);
            pos.x -= MoveSpeed * Mathf.Sin(delta);
            pos.z -= MoveSpeed * Mathf.Cos(delta);
        }
        transform.position = pos;
    }

    public void FaceTo(float x, float z)
    {
        float angle = Vector2.SignedAngle(Vector2.up, new Vector2(x, z));
        angle += 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector2.up);
    }

    #region AI机器人
    public void InitAI(bool ai)
    {
        if (IsAI == ai) return;
        IsAI = ai;
        if (ai)
        {
            InvokeRepeating("OnTimerAI", 1f, 1f);
        }
        else
        {
            CancelInvoke("OnTimerAI");
        }
    }

    void OnTimerAI()
    {
        if (IsMoving)
        {
            //if (Random.Range(1, 100) < 50)
            //{
            //    MoveStop();
            //}
            //else
            {
                MoveBy(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            }
        }
        else if (Random.Range(1, 100) < 50)
        {
            MoveStart();
            MoveBy(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        }
    }

    #endregion

    #region 武器
    public void ChangeWeapon(int id)
    {
        if (id < 0)
        {
            id = Random.Range(1, ResMgr.It.GetWeaponCount());
        }
        if (Weapon)
        {
            if (Weapon.ID == id) return;
            ResMgr.It.ReleaseWeapon(Weapon);
            Weapon = null;
        }
        Weapon = ResMgr.It.CreateWeapon(id);
        Weapon.OnAttach(this);
    }

    // 开火
    public void Shoot()
    {
        if (Weapon.IsShooting) return;
        // 播放人物的射击动画

        // 执行武器的射击逻辑
        Weapon.ShootBegin();
    }

    #endregion

    #region 数值加成

    #endregion
}
