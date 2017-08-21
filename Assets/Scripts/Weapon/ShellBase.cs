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
    private float m_Speed;
    private float m_Distance;
    private Vector3 m_BeginPos;
    private bool m_Check;
    private GameObject m_Target;

    // 子类属性
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
    public virtual void Shoot(float speed, float distance, bool back)
    {
        State = EnShellState.Flying;
        // 子弹脱离武器
        transform.parent = CombatMgr.It.ObjRoot;
        m_Dir = transform.position - Owner.Owner.transform.position;
        m_Dir = m_Dir.normalized;
        m_Speed = speed;
        m_Distance = distance;
        m_BeginPos = transform.position;
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
            Vector3 pos = transform.localPosition;
            pos.y = m_DropPosY;
            transform.localPosition = pos;
            //Quaternion q = transform.rotation;
            //transform.rotation = Quaternion.Euler(m_DropAngleX, q.y, q.z);
            transform.Rotate(Vector3.right, m_DropAngleX);
        }
        else
        {
            switch (m_Target.tag)
            {
                case "wall":

                    break;
                case "ground":

                    break;
                case "hero":

                    break;
                default:
                    break;
            }
            
        }
        //DestroyObject(gameObject, CONST.SHELL_DESTROY);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!m_Check || State != EnShellState.Flying) return;
        if (collider.gameObject == Owner || collider.gameObject == Owner.Owner.gameObject) return;
        if (collider.tag == "shell") return;
        FlyEnd(collider.gameObject);
        Debug.Log(name + "->" + collider.name);
    }
}
