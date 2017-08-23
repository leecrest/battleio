using System.Collections;
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
    public WeaponBase Weapon = null;
    [HideInInspector]
    public float MoveSpeed = 10f;

    // 养成属性
    public int Damage { get { return Mathf.FloorToInt(Atk * (1 + DamageRatio)); } }

    private System.DateTime m_Birth;        // 出生时间点，用于计算存活时长
    public int Atk;                         // 攻击
    public int Def;                         // 防御
    public int CurHP;                       // 当前生命值
    public int MaxHP;                       // 最大生命值
    public int Exp = 0;                     // 经验值
    public int Level = 1;                   // 等级
    public int ShellCount = 0;              // 武器加成：子弹数量加成
    public float ShellSpeed = 0f;           // 武器加成：子弹速度加成
    public float ShellRange = 0f;           // 武器加成：子弹射程加成
    public float CureRatio = 0;             // 治疗加成系数
    public float DamageRatio = 0;           // 伤害加成系数


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

    public void OnInit(int hp, int maxhp)
    {
        base.OnInit();
        Weapon = null;
        CurHP = hp;
        MaxHP = maxhp;
        Exp = 0;
        Level = 1;
        ShellCount = 0;
        ShellRange = 0;
        ShellSpeed = 0;
        CureRatio = 0;
        DamageRatio = 0;
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
            id = Random.Range(1, Config.WeaponCfg.Length);
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

    // 加血
    public void OnCure(int hp)
    {
        if (hp <= 0 || CurHP <= 0) return;
        CurHP += Mathf.FloorToInt(hp * (CureRatio + 1));
        if (CurHP > MaxHP) CurHP = MaxHP;
    }

    // 扣血
    public void OnDamage(int hp)
    {
        if (hp > 0) return;
        CurHP -= hp;
        if (CurHP < 0) CurHP = 0;
    }

    // 加经验
    public void AddExp(int exp)
    {
        if (exp <= 0 || Level >= Config.LevelMax) return;
        int max = Config.LevelExp[Level];
        Exp += exp;
        if (Exp >= max)
        {
            Level++;
            Exp = 0;
            CombatMgr.It.OnHeroLevelup(this);
        }
    }


    #endregion
}
