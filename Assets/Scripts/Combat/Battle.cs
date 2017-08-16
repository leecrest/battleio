using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour
{
    protected int m_AllocFid = 0;
    protected FighterHero m_MainHero;
    protected Dictionary<int, FighterHero> m_Heros = new Dictionary<int, FighterHero>();
    protected Dictionary<int, FighterItem> m_Items = new Dictionary<int, FighterItem>();
    protected Dictionary<int, FighterBullet> m_Bullets = new Dictionary<int, FighterBullet>();


    // 战斗开始
    public virtual void BattleStart()
    {

    }

    // 战斗结束
    public virtual void BattleOver()
    {
        CameraReset();
    }

    // 战斗销毁
    public virtual void BattleExit()
    {
        foreach (FighterHero obj in m_Heros.Values)
        {
            Global.It.ReleaseHero(obj);
        }
        m_Heros.Clear();
        foreach (FighterItem obj in m_Items.Values)
        {
            Global.It.ReleaseItem(obj);
        }
        m_Items.Clear();
        foreach (FighterBullet obj in m_Bullets.Values)
        {
            Global.It.ReleaseBullet(obj);
        }
        m_Bullets.Clear();
    }

    protected int NewFid()
    {
        m_AllocFid++;
        return m_AllocFid;
    }

    protected FighterHero AddHero(bool isMain, int fid, int id, float x, float z)
    {
        FighterHero hero = Global.It.NewFighterHero(fid, id, x, z);
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
        Global.It.ReleaseHero(hero);
    }

    protected FighterItem AddItem(int fid, int id, float x, float z)
    {
        FighterItem item = Global.It.NewFighterItem(fid, id, x, z);

        m_Items.Add(fid, item);
        return item;
    }

    protected void DelItem(int fid)
    {
        FighterItem item;
        if (!m_Items.TryGetValue(fid, out item)) return;
        m_Items.Remove(fid);
        Global.It.ReleaseItem(item);
    }

    protected FighterBullet AddBullet(int id, float x, float z)
    {
        int fid = NewFid();
        FighterBullet bullet = Global.It.CreateBullet(fid, id, x, z);
        m_Bullets.Add(fid, bullet);
        return bullet;
    }

    protected void DelBullet(int fid)
    {
        FighterBullet bullet;
        if (!m_Bullets.TryGetValue(fid, out bullet)) return;
        m_Bullets.Remove(fid);
        Global.It.ReleaseBullet(bullet);
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

    #region 主角控制
    public void ReqMainMoveStart()
    {
        if (m_MainHero == null) return;
        OnMoveStart(m_MainHero);
    }

    public void ReqMainMoveStop()
    {
        if (m_MainHero == null) return;
        OnMoveStop(m_MainHero);
    }

    public void ReqMainMove(float x, float z)
    {
        if (m_MainHero)
        {
            OnMove(m_MainHero, x, z);
        }
        else
        {
            Transform tf = Camera.main.transform;
            Vector3 pos = tf.position;
            pos.x -= x * 0.05f;
            pos.z -= z * 0.05f;
            tf.position = pos;
        }
    }

    public void ReqMainUseSkill(int idx)
    {
        if (m_MainHero == null) return;
        OnUseSkill(m_MainHero, idx);
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
