﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 战斗状态
public enum EnGameState
{
    Ready = 0,
    Start,
    Over,
}

public class GameMgr : MgrBase
{
    public static GameMgr It;
    void Awake() { It = this; }

    [HideInInspector]
    public EnGameState State;
    [HideInInspector]
    public int Gold = 0;
    [HideInInspector]
    public int Money = 0;
    [HideInInspector]
    public int ScoreMax = 0;
    [HideInInspector]
    public int ScoreCur = 0;

    public Transform SceneRoot;
    public Transform ObjRoot;
    public ETCJoystick m_Joystick;

    private bool m_Moving;
    private int m_SceneID;
    private int m_LastSceneID;
    private GameObject m_SceneObj;
    private MapBase m_SceneMap;

    private int m_AllocFid = 0;
    private FighterHero m_MainHero;
    protected Dictionary<int, FighterHero> m_Heros;

    public override void Init ()
    {
        m_Moving = false;
        m_SceneID = -1;
        m_LastSceneID = -1;
        m_SceneObj = null;
        m_Heros = new Dictionary<int, FighterHero>();
        LoadUser();
        UIMgr.It.OpenUI("UIHome");
    }

    public override void UnInit()
    {
        SaveUser();
    }

    void LoadUser()
    {
        Gold = PlayerPrefs.GetInt("Gold", 0);
        Money = PlayerPrefs.GetInt("Money", 0);
        ScoreMax = PlayerPrefs.GetInt("ScoreMax", 0);
        ScoreCur = PlayerPrefs.GetInt("ScoreCur", 0);

    }

    void SaveUser()
    {
        PlayerPrefs.SetInt("Gold", Gold);
        PlayerPrefs.SetInt("Money", Money);
        PlayerPrefs.SetInt("ScoreMax", ScoreMax);
        PlayerPrefs.SetInt("ScoreCur", ScoreCur);
    }

    void Update()
    {
#if UNITY_STANDALONE_WIN
        Vector2 delta = new Vector2(0, 0);
        bool move = false;
        if (Input.GetKey(KeyCode.W)) { delta.y = -1f; move = true; }
        if (Input.GetKey(KeyCode.S)) { delta.y = 1f; move = true; }
        if (Input.GetKey(KeyCode.A)) { delta.x = 1f; move = true; }
        if (Input.GetKey(KeyCode.D)) { delta.x = -1f; move = true; }

        if (m_Moving)
        {
            if (move)
            {
                OnJoystickMove(delta);
            }
            else
            {
                m_Moving = false;
                if (m_MainHero) OnJoystickMoveEnd();
            }
        }
        else if (move)
        {
            m_Moving = true;
            if (m_MainHero)
            {
                OnJoystickMoveStart();
            }
        }

        if (m_MainHero && Input.GetKey(KeyCode.Space))
        {
            OnHeroShoot();
        }
#endif
    }

    #region 战斗逻辑
    // 战斗准备
    public void CombatReady()
    {
        CameraReset();
        State = EnGameState.Ready;
        m_Joystick.activated = false;
        m_Joystick.onMoveStart.AddListener(OnJoystickMoveStart);
        m_Joystick.onMove.AddListener(OnJoystickMove);
        m_Joystick.onMoveEnd.AddListener(OnJoystickMoveEnd);
    }

    // 战斗开始
    public void CombatStart()
    {
        if (State == EnGameState.Start) return;
        State = EnGameState.Start;
        m_SceneID = 1;
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
        State = EnGameState.Over;
        m_Joystick.activated = false;
        CameraReset();
        if (m_SceneMap) m_SceneMap.OnOver();
    }

    // 战斗退出
    public void CombatExit()
    {
        State = EnGameState.Ready;
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

    public FighterHero AddHero(bool isMain, int id, float x, float z, int fid, string name, int hp, int maxhp)
    {
        if (fid <= 0) fid = NewFid();
        FighterHero hero = ResMgr.It.CreateHero(id, x, z, fid, name, hp, maxhp);
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

    public void Damage(FighterHero atk, FighterHero def, int damage)
    {
        if (damage <= 0 || def.CurHP <= 0) return;
        // 伤害公式
        
        def.OnDamage(damage);
        if (def.CurHP <= 0)
        {
            OnHeroKill(atk, def);
        }
    }

    public void Cure(FighterHero hero, int cure)
    {
        if (cure <= 0 || hero.CurHP <= 0 || hero.CurHP >= hero.MaxHP) return;
        hero.OnCure(cure);
    }

    // 玩家atk击杀了def
    public void OnHeroKill(FighterHero atk, FighterHero def)
    {

        if (def.IsMain)
        {
            // 主角死亡，显示死亡界面

        }
        else
        {
            ResMgr.It.ReleaseHero(def);
        }
    }

    // 玩家升级
    public void OnHeroLevelup(FighterHero hero)
    {

    }

    #endregion


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
            float speed = Config.CAMERA_SPEED;
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

    public void OnHeroShoot()
    {
        if (m_MainHero == null) return;
        m_MainHero.Shoot();
    }

    public void GM_ChangeWeapon()
    {
        if (m_MainHero == null) return;
        int id = 1;
        if (m_MainHero.Weapon)
        {
            id = m_MainHero.Weapon.ID + 1;
            if (id > Config.WeaponCfg.Length)
            {
                id = 1;
            }
        }
        m_MainHero.ChangeWeapon(id);
    }

    #endregion

    #region 镜头控制
    protected void CameraReset()
    {
        Camera.main.transform.position = new Vector3(0, Config.CAMERA_HEIGHT, 0);
        //Camera.main.transform.rotation = Quaternion.identity;
        //Camera.main.transform.Rotate(Vector3.right, 80);
    }

    protected void UpdateCamera()
    {
        if (m_MainHero == null) return;
        Vector3 dst = m_MainHero.transform.position;
        Vector3 src = Camera.main.transform.position;
        dst.y = Config.CAMERA_HEIGHT;
        dst.z -= 2;
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

    #endregion
}
