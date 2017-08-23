using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponBase : MonoBehaviour {
    public int ID = 0;
    public FighterHero Owner = null;

    // 是否处于发射状态
    [HideInInspector]
    public bool IsShooting = false;
    [HideInInspector]
    public bool HasShell = false;

    protected WeaponConfig m_Config;
    protected List<ShellBase> m_Shells;

    public virtual void OnInit()
    {
        m_Config = Config.WeaponCfg[ID - 1];
        HasShell = m_Config.shell > 0;
        if (HasShell) m_Shells = new List<ShellBase>();
    }

    public virtual void OnUninit()
    {
        if (HasShell) m_Shells.Clear();
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

    public int GetShellMax()
    {
        if (Owner) return Owner.ShellCount + 1;
        else return 1;
    }

    // 装弹
    public virtual void LoadShell()
    {
        // 播放人物的装弹动画

        // 加载子弹
        ShellBase obj;
        int max = Owner.ShellCount + 1;
        for (int i = m_Shells.Count; i < max; i++)
        {
            obj = ResMgr.It.CreateShell(ID, transform);
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
        if (HasShell)
        {
            ShootShell();
        }
        if (m_Config.atktime > 0)
        {
            Invoke("ShootEnd", m_Config.atktime);
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
        if (HasShell && m_Shells.Count < Owner.ShellCount+1) LoadShell();
    }

    // 子弹发射
    public virtual void ShootShell()
    {
        ShellBase shell;
        for (int i = 0; i < m_Shells.Count; i++)
        {
            shell = m_Shells[i];
            shell.Shoot(Owner.ShellSpeed, Owner.ShellRange, Owner.Damage, false);
        }
        m_Shells.Clear();
    }
}
