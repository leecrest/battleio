using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMgr : MonoBehaviour
{
    public static CombatMgr It;
    void Awake() { It = this; }

    [HideInInspector]
    public EnCombatState State;

    public Transform SceneRoot;
    public Transform ObjRoot;
    public ETCJoystick m_Joystick;

    private int m_SceneID;
    private int m_LastSceneID;
    private GameObject m_SceneObj;
    private MapBase m_SceneMap;

    private int m_AllocFid = 0;
    private FighterHero m_MainHero;
    protected Dictionary<int, FighterHero> m_Heros;

    public void OnInit ()
    {
        m_SceneID = -1;
        m_LastSceneID = -1;
        m_SceneObj = null;
        m_Heros = new Dictionary<int, FighterHero>();
    }

    public void OnUninit()
    {

    }
	

    // 战斗准备
    public void CombatReady()
    {
        CameraReset();
        State = EnCombatState.Ready;
        m_Joystick.activated = false;
        m_Joystick.onMoveStart.AddListener(OnJoystickMoveStart);
        m_Joystick.onMove.AddListener(OnJoystickMove);
        m_Joystick.onMoveEnd.AddListener(OnJoystickMoveEnd);
    }

    // 战斗开始
    public void CombatStart()
    {
        if (State == EnCombatState.Start) return;
        State = EnCombatState.Start;
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
            m_SceneMap = m_SceneObj.GetComponent<MapBase>();
        }
        if (m_SceneMap) m_SceneMap.OnStart();
        m_LastSceneID = m_SceneID;
        m_Joystick.activated = true;
        m_Joystick.enabled = true;
    }

    // 战斗结束
    public void CombatOver()
    {
        State = EnCombatState.Over;
        m_Joystick.activated = false;
        CameraReset();
        if (m_SceneMap) m_SceneMap.OnOver();
    }

    // 战斗退出
    public void CombatExit()
    {
        State = EnCombatState.Ready;
        foreach (FighterHero obj in m_Heros.Values)
        {
            ResMgr.It.ReleaseHero(obj);
        }
        m_Heros.Clear();
        if (m_SceneMap) m_SceneMap.OnExit();
    }

    protected int NewFid()
    {
        m_AllocFid++;
        return m_AllocFid;
    }

    public FighterHero AddHero(bool isMain, int fid, int id, float x, float z)
    {
        if (fid <= 0) fid = NewFid();
        FighterHero hero = ResMgr.It.NewHero(fid, id, x, z);
        hero.IsMain = isMain;
        m_Heros.Add(fid, hero);
        if (isMain)
        {
            m_MainHero = hero;
            UpdateCamera();
        }
        return hero;
    }

    public void DelHero(int fid)
    {
        FighterHero hero;
        if (!m_Heros.TryGetValue(fid, out hero)) return;
        m_Heros.Remove(fid);
        ResMgr.It.ReleaseHero(hero);
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
            float speed = CONST.CAMERA_SPEED;
            if (Mathf.Approximately(vec.x, 0f))
            {
                if (Mathf.Approximately(vec.y, 0f)) return;
                pos.z -= vec.y > 0f ? speed : -speed;
            }
            else if (Mathf.Approximately(vec.y, 0f))
            {
                if (Mathf.Approximately(vec.x, 0f)) return;
                pos.x -= vec.x > 0f ? speed : -speed;
            }
            else
            {
                float delta = Mathf.Atan2(vec.x, vec.y);
                pos.x -= speed * Mathf.Sin(delta);
                pos.z -= speed * Mathf.Cos(delta);
            }
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

    public void GM_ChangeWeapon()
    {
        if (m_MainHero == null) return;
        int id = 0;
        if (m_MainHero.Weapon)
        {
            id = m_MainHero.Weapon.ID + 1;
            if (id > ResMgr.It.m_WeaponPrefabs.Length - 1)
            {
                id = 0;
            }
        }
        m_MainHero.ChangeWeapon(id);
    }

    #endregion

    #region 镜头控制
    protected void CameraReset()
    {
        Camera.main.transform.position = new Vector3(0, CONST.CAMERA_Z, 0);
        //Camera.main.transform.rotation = Quaternion.identity;
        //Camera.main.transform.Rotate(Vector3.right, 80);
    }

    protected void UpdateCamera()
    {
        if (m_MainHero == null) return;
        Vector3 dst = m_MainHero.transform.position;
        Vector3 src = Camera.main.transform.position;
        dst.y = CONST.CAMERA_Z;
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
