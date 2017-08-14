using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMgr : MonoBehaviour {
    public static CombatMgr It;
    void Awake() { It = this; }

    public Transform m_SceneRoot;
    public Transform m_ObjRoot;
    public ETCJoystick m_Joystick;

    private FighterHero m_MainHero;

    private int m_SceneID;
    private int m_LastSceneID;
    private GameObject m_SceneObj;

    void Start () {
        m_SceneID = -1;
        m_LastSceneID = -1;
        m_SceneObj = null;
    }
	
    public void CombatReady()
    {
        CameraReset();
        m_Joystick.activated = false;
    }

    public void CombatStart()
    {
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
            m_SceneObj = Global.It.LoadScene(m_SceneID, m_SceneRoot);
        }
        m_LastSceneID = m_SceneID;

        // 创建主角，并将镜头绑定到主角头上
        CreateHero(true, 0, 0);
        for (int i = 0; i < 20; i++)
        {
            CreateHero(false, Random.Range(-10f,10f), Random.Range(-10f,10f));
        }
        m_Joystick.activated = true;
        m_Joystick.enabled = true;
    }

    public void CombatOver()
    {
        m_Joystick.activated = false;
        CameraReset();
    }

    void CreateHero(bool isMain, float x, float z)
    {
        FighterHero hero = Global.It.NewFighterHero(0, x, z, m_ObjRoot);
        hero.IsMain = isMain;
        if (isMain)
        {
            m_MainHero = hero;
            //m_Joystick.axisX.directTransform = hero.transform;
            UpdateCamera();
        }
    }

    #region 主角对象池
    

    #endregion

    #region 镜头控制
    public void CameraReset()
    {
        Camera.main.transform.position = new Vector3(0, 10, 25);
        Camera.main.transform.rotation = Quaternion.identity;
        Camera.main.transform.Rotate(Vector3.right, 45);
    }

    void UpdateCamera()
    {
        if (m_MainHero == null) return;
        Vector3 dst = m_MainHero.transform.position;
        Vector3 src = Camera.main.transform.position;
        dst.y = 3;
        dst.z -= 3;
        Camera.main.transform.position = dst;
    }

    #endregion

    #region 输入控制
    
    public void OnJoystickMoveStart()
    {
        // 主角进入移动状态
        if (m_MainHero) m_MainHero.StartMove();
    }

    public void OnJoystickMove(Vector2 vec)
    {
        if (m_MainHero)
        {
            m_MainHero.MoveBy(vec.x, vec.y);
            UpdateCamera();
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
        if (m_MainHero) m_MainHero.StopMove();
    }

    #endregion
}
