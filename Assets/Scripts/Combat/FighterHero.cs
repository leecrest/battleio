using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterHero : FighterBase
{
    [HideInInspector]
    public bool IsMain = false;
    [HideInInspector]
    public bool IsAI = false;

    public Vector2 BloodSize = new Vector2(150, 20);
    public Vector2 NameSize = new Vector2(200, 50);

    private bool m_Moving = false;      // 是否移动中
    private bool m_Fighting = false;    // 是否战斗中

    public int CurHP = 50;
    public int MaxHP = 100;
    public float Speed = 0.05f;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
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
            Global.It.TexBloodBg);
        //在绘制红色血条
        GUI.DrawTexture(new Rect(position.x - BloodSize.x / 2, position.y - BloodSize.y, blood_width, BloodSize.y),
            IsMain ? Global.It.TexBloodSelf : Global.It.TexBloodFriend);

        //计算名称的宽高
        GUI.Label(new Rect(position.x - NameSize.x / 2, position.y - NameSize.y - BloodSize.y, NameSize.x, NameSize.y), 
            name, Global.It.MySkin.label);
    }

    public void MoveStart()
    {
        if (m_Moving) return;
        m_Moving = true;
        Animator anim = GetComponent<Animator>();
        anim.Play("Run");
    }

    public void MoveStop()
    {
        if (!m_Moving) return;
        m_Moving = false;
        Animator anim = GetComponent<Animator>();
        anim.Play("Idle");
    }

    public void MoveBy(float x, float z)
    {
        float angle = Vector2.SignedAngle(Vector2.down, new Vector2(x, z));
        transform.rotation = Quaternion.AngleAxis(angle, Vector2.down);
        Vector3 pos = transform.position;
        if (Mathf.Approximately(x, 0f))
        {
            if (Mathf.Approximately(z, 0f)) return;
            pos.z -= z > 0f ? Speed : -Speed;
        }
        else if (Mathf.Approximately(z, 0f))
        {
            if (Mathf.Approximately(x, 0f)) return;
            pos.x -= x > 0f ? Speed : -Speed;
        }
        else
        {
            float delta = Mathf.Atan2(x, z);
            pos.x -= Speed * Mathf.Sin(delta);
            pos.z -= Speed * Mathf.Cos(delta);
        }
        transform.position = pos;
    }

    public void FaceTo(float x, float z)
    {
        float angle = Vector2.SignedAngle(Vector2.down, new Vector2(x, z));
        transform.rotation = Quaternion.AngleAxis(angle, Vector2.down);
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
        if (m_Moving)
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
}
