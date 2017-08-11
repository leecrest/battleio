using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour {
    [HideInInspector]
    public bool IsMain = false;
    private bool m_Moving = false;
    private Vector3 m_FaceTo;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        /*if (IsMain)
        {
            Vector3 dst = transform.position;
            float offset = Time.deltaTime;
            bool move = false;
            if (Input.GetKey(KeyCode.W)) { dst.z += offset; move = true; }
            if (Input.GetKey(KeyCode.S)) { dst.z -= offset; move = true; }
            if (Input.GetKey(KeyCode.A)) { dst.x -= offset; move = true; }
            if (Input.GetKey(KeyCode.D)) { dst.x += offset; move = true; }
            if (move)
            {
                if (!m_Moving) StartMove();
                transform.position = dst;

                Vector3 src = Camera.main.transform.position;
                dst.y = 3;
                dst.z -= 3;
                Camera.main.transform.position = dst;
            }
            else
            {
                if (m_Moving) StopMove();
            }
        }*/
    }

    public void Init()
    {
        m_FaceTo = Vector3.up;
    }

    public void StartMove()
    {
        m_Moving = true;
        Animator anim = GetComponent<Animator>();
        anim.Play("Run");
    }

    public void StopMove()
    {
        m_Moving = false;
        Animator anim = GetComponent<Animator>();
        anim.Play("Idle");
    }

    public void MoveBy(float x, float z)
    {
        Vector3 pos = transform.position;
        pos.x -= x * 0.05f;
        pos.z -= z * 0.05f;
        transform.position = pos;
        transform.Rotate(Vector3.up, Vector3.Angle(m_FaceTo, pos));
        m_FaceTo = pos;
    }
}


/**
 * 模型的动画控制如下：
 * 1、idle：没有发现敌人时的原地待机动作
 * 2、stand：战斗状态时的站立动作
 * 3、walk：巡逻
 * 4、run：战斗中的移动
 * 5、attack：普通攻击
 * 6、damage：受击
 * 7、skill：大招
 * 8、death：死亡
 * 9、knockback：被击退
 * 
 * 
 * 
 */