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
    private Battle m_Battle;

    void Start ()
    {
        m_SceneID = -1;
        m_LastSceneID = -1;
        m_SceneObj = null;
    }
	
    public void CombatReady()
    {
        m_Joystick.activated = false;
    }

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
            m_SceneObj = Global.It.LoadScene(m_SceneID, SceneRoot);
            m_Battle = m_SceneObj.GetComponent<Battle>();
        }
        m_LastSceneID = m_SceneID;
        m_Battle.BattleStart();
        m_Joystick.activated = true;
        m_Joystick.enabled = true;
    }

    public void CombatOver()
    {
        m_Joystick.activated = false;
        m_Battle.BattleOver();
    }

    public void CombatExit()
    {
        m_Battle.BattleExit();
        m_Battle = null;
    }

    

    

    #region 输入控制
    public void OnJoystickMoveStart()
    {
        if (m_Battle == null) return;
        m_Battle.ReqMainMoveStart();
    }

    public void OnJoystickMove(Vector2 vec)
    {
        if (m_Battle == null) return;
        m_Battle.ReqMainMove(vec.x, vec.y);
    }

    public void OnJoystickMoveEnd()
    {
        if (m_Battle == null) return;
        m_Battle.ReqMainMoveStop();
    }

    public void OnUseSkill(int idx)
    {
        if (m_Battle == null) return;
        m_Battle.ReqMainUseSkill(idx);
    }
    #endregion
}
