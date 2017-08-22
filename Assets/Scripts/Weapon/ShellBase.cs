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
    public int ID;
    [HideInInspector]
    public int WeaponID;
    [HideInInspector]
    public WeaponBase Owner;
    [HideInInspector]
    public EnShellState State;

    private float m_Speed;
    private float m_Distance;
    private Vector3 m_Dir;
    private Vector3 m_BeginPos;
    private bool m_Check;
    private GameObject m_Target;
    private GameObject m_Hole;
    private int m_Damage;

    // 子类属性
    protected ShellConfig m_Config;
    protected float m_DropPosY;       // 掉落坐标点的Y偏移
    protected float m_DropAngleX;     // 掉落角度的x



    void FixedUpdate()
    {
        if (State != EnShellState.Flying) return;
        Vector3 pos = transform.localPosition;
        float delta = m_Speed * Time.deltaTime;
        pos.x += m_Dir.x * delta;
        pos.z += m_Dir.z * delta;
        transform.localPosition = pos;
        if (m_Distance > 0 && Vector3.Distance(m_BeginPos, transform.position) >= m_Distance)
        {
            FlyEnd(null);
        }
    }

    public virtual void OnInit()
    {
        m_Check = false;
        m_Config = ResMgr.It.GetShellConfig(ID);
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
    public virtual void Shoot(float speed, float distance, int damage, bool back)
    {
        State = EnShellState.Flying;
        // 子弹脱离武器
        transform.parent = CombatMgr.It.ObjRoot;
        m_Dir = transform.position - Owner.Owner.transform.position;
        m_Dir = m_Dir.normalized;
        m_Speed = m_Config.speed + speed;
        m_Distance = m_Config.distance + distance;
        m_BeginPos = transform.position;
        m_Damage = damage;
        // 开始飞行，启动碰撞检测
        m_Check = true;
    }

    // 飞行结束
    void FlyEnd(GameObject target)
    {
        State = EnShellState.InTarget;
        m_Target = target;
        if (m_Target == null)
        {
            // 飞行超时，落在地面上
            Drop2Ground();
        }
        else
        {
            switch (m_Target.tag)
            {
                case "wall":
                    Drop2Wall();
                    break;
                case "ground":
                    Drop2Ground();
                    break;
                case "hero":
                    Drop2Hero();
                    break;
                default:
                    break;
            }
            
        }
        Invoke("OnTimerDestroy", (float)ResMgr.It.GetConst("SHELL_DESTROY"));
    }

    // 子弹落在墙壁上
    void Drop2Wall()
    {
        //m_Hole = Instantiate(m_Config.hole);
        //m_Hole.transform.parent = transform;
        //OnWallHole(m_Hole.transform);
    }

    // 子弹落在地面上
    void Drop2Ground()
    {
        Vector3 pos = transform.localPosition;
        pos.y = m_DropPosY;
        transform.localPosition = pos;
        transform.Rotate(Vector3.right, m_DropAngleX);
        m_Hole = Instantiate(m_Config.hole);
        m_Hole.transform.parent = transform;
        OnGroundHole(m_Hole.transform);
    }

    // 子弹落在玩家身上
    void Drop2Hero()
    {
        transform.parent = m_Target.transform;
        CombatMgr.It.Damage(Owner.Owner, m_Target.GetComponent<FighterHero>(), m_Damage);
    }

    // 子类继承，修改地面弹孔的数据
    protected virtual void OnGroundHole(Transform hole)
    {

    }

    protected virtual void OnWallHole(Transform hole)
    {

    }

    void OnTriggerEnter(Collider collider)
    {
        if (!m_Check || State != EnShellState.Flying) return;
        if (collider.gameObject == Owner || collider.gameObject == Owner.Owner.gameObject) return;
        if (collider.tag == "shell") return;
        FlyEnd(collider.gameObject);
    }

    void OnTriggerExit(Collider collider)
    {
        if (!m_Check || State != EnShellState.Flying) return;
        if (collider.gameObject == Owner || collider.gameObject == Owner.Owner.gameObject) return;
        if (collider.tag == "shell") return;
        FlyEnd(collider.gameObject);
    }

    // 子弹被销毁
    private void OnTimerDestroy()
    {
        if (m_Hole != null) DestroyObject(m_Hole);
        DestroyObject(gameObject);
    }
}
