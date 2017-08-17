using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMgr : MonoBehaviour {
    public static CombatMgr It;
    void Awake() { It = this; }

    public Transform SceneRoot;
    public Transform ObjRoot;
    public ETCJoystick m_Joystick;

    private int m_SceneID;
    private int m_LastSceneID;
    private GameObject m_SceneObj;
    private int m_AllocFid = 0;
    private FighterHero m_MainHero;
    private Dictionary<int, FighterHero> m_Heros;
    private Dictionary<int, FighterItem> m_Items;
    private Dictionary<int, FighterBullet> m_Bullets;

    public void OnInit ()
    {
        m_SceneID = -1;
        m_LastSceneID = -1;
        m_SceneObj = null;
        m_Heros = new Dictionary<int, FighterHero>();
        m_Items = new Dictionary<int, FighterItem>();
        m_Bullets = new Dictionary<int, FighterBullet>();
    }

    public void OnUninit()
    {

    }
	

    // 战斗准备
    public void CombatReady()
    {
        m_Joystick.activated = false;
    }

    // 战斗开始
    public void CombatStart()
    {
        m_SceneID = 0;
        // 检查是否要销毁原场景
        if (m_SceneObj != null && m_LastSceneID != m_SceneID)
        {
            DestroyObject(m_SceneObj);
            m_SceneObj = null;
        }
        // 检查是否需要加载场景
        if (m_SceneObj == null)
        {
            m_SceneObj = ResMgr.It.LoadScene(m_SceneID, SceneRoot);
        }
        m_LastSceneID = m_SceneID;
        m_Joystick.activated = true;
        m_Joystick.enabled = true;
    }

    // 战斗结束
    public void CombatOver()
    {
        m_Joystick.activated = false;
        CameraReset();
    }

    // 战斗退出
    public void CombatExit()
    {
        foreach (FighterHero obj in m_Heros.Values)
        {
            ResMgr.It.ReleaseHero(obj);
        }
        m_Heros.Clear();
        foreach (FighterItem obj in m_Items.Values)
        {
            ResMgr.It.ReleaseItem(obj);
        }
        m_Items.Clear();
        foreach (FighterBullet obj in m_Bullets.Values)
        {
            ResMgr.It.ReleaseBullet(obj);
        }
        m_Bullets.Clear();
    }

    

    #region 输入控制
    public void OnJoystickMoveStart()
    {
        if (m_MainHero == null) return;
        OnMoveStart(m_MainHero);
    }

    public void OnJoystickMove(Vector2 vec)
    {
        if (m_MainHero)
        {
            OnMove(m_MainHero, vec.x, vec.y);
        }
        else
        {
            Transform tf = Camera.main.transform;
            Vector3 pos = tf.position;
            pos.x -= vec.x * 0.05f;
            pos.z -= vec.y * 0.05f;
            tf.position = pos;
        }
    }

    public void OnJoystickMoveEnd()
    {
        if (m_MainHero == null) return;
        OnMoveStop(m_MainHero);
    }

    public void OnUseSkill(int idx)
    {
        if (m_MainHero == null) return;
        OnUseSkill(m_MainHero, idx);
    }
    #endregion

    private int NewFid()
    {
        m_AllocFid++;
        return m_AllocFid;
    }

    protected FighterHero AddHero(bool isMain, int fid, int id, float x, float z)
    {
        FighterHero hero = ResMgr.It.NewFighterHero(fid, id, x, z);
        hero.IsMain = isMain;
        m_Heros.Add(fid, hero);
        if (isMain)
        {
            m_MainHero = hero;
            UpdateCamera();
        }
        return hero;
    }

    protected void DelHero(int fid)
    {
        FighterHero hero;
        if (!m_Heros.TryGetValue(fid, out hero)) return;
        m_Heros.Remove(fid);
        ResMgr.It.ReleaseHero(hero);
    }

    protected FighterItem AddItem(int fid, int id, float x, float z)
    {
        FighterItem item = ResMgr.It.NewFighterItem(fid, id, x, z);

        m_Items.Add(fid, item);
        return item;
    }

    protected void DelItem(int fid)
    {
        FighterItem item;
        if (!m_Items.TryGetValue(fid, out item)) return;
        m_Items.Remove(fid);
        ResMgr.It.ReleaseItem(item);
    }

    protected FighterBullet AddBullet(int id, float x, float z)
    {
        int fid = NewFid();
        FighterBullet bullet = ResMgr.It.CreateBullet(fid, id, x, z);
        m_Bullets.Add(fid, bullet);
        return bullet;
    }

    protected void DelBullet(int fid)
    {
        FighterBullet bullet;
        if (!m_Bullets.TryGetValue(fid, out bullet)) return;
        m_Bullets.Remove(fid);
        ResMgr.It.ReleaseBullet(bullet);
    }


    #region 镜头控制
    protected void CameraReset()
    {
        Camera.main.transform.position = new Vector3(0, 10, 25);
        //Camera.main.transform.rotation = Quaternion.identity;
        //Camera.main.transform.Rotate(Vector3.right, 80);
    }

    protected void UpdateCamera()
    {
        if (m_MainHero == null) return;
        Vector3 dst = m_MainHero.transform.position;
        Vector3 src = Camera.main.transform.position;
        dst.y = 10;
        dst.z -= 1;
        Camera.main.transform.position = dst;
    }
    #endregion

    #region 单位控制
    public void OnMoveStart(FighterHero hero)
    {
        hero.MoveStart();
    }

    public void OnMove(FighterHero hero, float x, float z)
    {
        hero.MoveBy(x, z);
        UpdateCamera();
    }

    public void OnMoveStop(FighterHero hero)
    {
        hero.MoveStop();
    }

    public void OnUseSkill(FighterHero hero, int idx)
    {

    }

    #endregion
}
