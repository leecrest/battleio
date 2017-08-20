using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnWeaponType
{
    Arrow = 1,      // 弓箭
    Blade,          // 刀

}




public class WeaponBase : MonoBehaviour {
    public int ID = 0;
    public string Name = "";
    public FighterHero Owner = null;
    public bool HasShell = false;

    // 子弹资源
    public ShellBase ShellPrefab;
    // 子弹数量
    public int ShellCount = 0;
    // 子弹回弹
    public bool ShellBack = false;
    // 攻击前摇
    public float TimeAttack = 0f;
    // 子弹飞行的基础速度
    public float ShellSpeed = 1f;
    // 子弹的飞行距离，0表示无限制
    public float ShellDistance = 10f;


    // 是否处于发射状态
    [HideInInspector]
    public bool IsShooting = false;
    protected List<ShellBase> m_Shells;

    public virtual void OnInit()
    {
        if (HasShell) m_Shells = new List<ShellBase>();
    }

    public virtual void OnUninit()
    {

    }

    // 绑定到玩家身上
    public virtual void OnAttach(FighterHero hero)
    {
        Owner = hero;
        transform.parent = hero.transform;
        IsShooting = false;
    }

    // 解除和玩家的绑定
    public virtual void OnDetach()
    {
        Owner = null;
        transform.parent = null;
    }

    // 装弹
    public virtual void LoadShell()
    {
        // 播放人物的装弹动画

        // 加载子弹
        int i = m_Shells.Count;
        ShellBase obj;
        for (; i < ShellCount; i++)
        {
            obj = Instantiate(ShellPrefab);
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.OnInit();
            obj.OnAttach(this);
            m_Shells.Add(obj);
        }
    }


    // 开始射击
    public void ShootBegin()
    {
        if (IsShooting) return;
        IsShooting = true;
        // 播放武器的攻击动画

        // 执行子弹行为
        if (HasShell && ShellCount > 0)
        {
            ShootShell();
        }
        if (TimeAttack > 0)
        {
            Invoke("ShootEnd", TimeAttack);
        }
        else
        {
            ShootEnd();
        }
    }

    // 射击结束
    public void ShootEnd()
    {
        IsShooting = false;
        if (HasShell && m_Shells.Count < ShellCount && !ShellBack) LoadShell();
    }

    // 子弹发射
    public virtual void ShootShell()
    {
        ShellBase shell;
        float speed;
        float distance;
        for (int i = 0; i < m_Shells.Count; i++)
        {
            shell = m_Shells[i];
            speed = ShellSpeed + Owner.WeaponShellSpeed;
            distance = ShellDistance + Owner.WeaponShellDistance;
            shell.Shoot(speed, distance, ShellBack);
        }
        m_Shells.Clear();
    }
}
