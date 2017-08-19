using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnShellState
{
    InWeapon = 1,   // 挂接在武器上
    Flying,         // 弹道飞行中
    InTarget,       // 挂接在目标上
}


public class ShellBase : MonoBehaviour {
    [HideInInspector]
    public WeaponBase Owner;
    [HideInInspector]
    public EnShellState State;

    private Vector3 m_Dir;



    void Update()
    {
        if (State != EnShellState.Flying) return;
        Vector3 pos = transform.localPosition;
        pos.x -= m_Dir.x * Time.deltaTime;
        pos.z -= m_Dir.z * Time.deltaTime;
        transform.localPosition = pos;
    }

    public virtual void OnInit()
    {
    }

    public virtual void OnUninit()
    {

    }

    public virtual void OnAttach(WeaponBase weapon)
    {
        Owner = weapon;
        transform.parent = weapon.transform;
        State = EnShellState.InWeapon;
    }

    public virtual void OnDetach()
    {
        transform.parent = null;
        State = EnShellState.Flying;
    }

    // 子弹发射
    public virtual void Shoot(bool back)
    {
        State = EnShellState.Flying;
        // 子弹脱离武器
        transform.parent = CombatMgr.It.ObjRoot;
        m_Dir = transform.localPosition.normalized;
    }
}
