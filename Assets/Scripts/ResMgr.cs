using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResMgr : MonoBehaviour
{
    public static ResMgr It;
    void Awake() { It = this; }

    // ������Դ
    public GameObject[] m_ScenePrefabs;
    // ս����λ��Դ
    public FighterHero[] m_FighterHeroPrefabs;
    // ������Դ
    public FighterItem[] m_FighterItemPrefabs;
    // �ӵ���Դ
    public FighterBullet[] m_FighterBulletPrefabs;

    private Stack<FighterHero> m_FighterHeroPool;
    private Stack<FighterItem> m_FighterItemPool;

    public void OnInit()
    {
        m_FighterHeroPool = new Stack<FighterHero>();
        m_FighterItemPool = new Stack<FighterItem>();
    }

    public void OnUninit()
    {
        DestroyAllHeros();
        DestroyAllItems();
    }

    public GameObject LoadScene(int id, Transform parent)
    {
        GameObject obj = Instantiate(m_ScenePrefabs[id]);
        obj.transform.rotation = Quaternion.identity;
        obj.transform.position = Vector3.zero;
        obj.transform.parent = parent;
        return obj;
    }

    public FighterHero NewFighterHero(int fid, int id, float x, float z)
    {
        FighterHero hero;
        if (m_FighterHeroPool.Count > 0)
        {
            hero = m_FighterHeroPool.Pop();
        }
        else
        {
            hero = Instantiate(m_FighterHeroPrefabs[id]);
        }
        hero.transform.parent = CombatMgr.It.ObjRoot;
        hero.transform.rotation = Quaternion.identity;
        hero.transform.position = new Vector3(x, 2, z);
        hero.transform.localScale = new Vector3(2, 2, 2);
        hero.gameObject.SetActive(true);
        hero.Fid = fid;
        hero.name = "hero_" + fid;
        hero.OnCreate();
        return hero;
    }

    public void ReleaseHero(FighterHero hero)
    {
        hero.OnDestroy();
        hero.gameObject.SetActive(false);
        m_FighterHeroPool.Push(hero);
    }

    public void DestroyAllHeros()
    {
        if (m_FighterHeroPool.Count <= 0) return;
        FighterHero hero;
        while (m_FighterHeroPool.Count > 0)
        {
            hero = m_FighterHeroPool.Pop();
            if (hero == null) break;
            hero.OnDestroy();
            DestroyObject(hero.gameObject);
        }
        m_FighterHeroPool.Clear();
    }

    public FighterItem NewFighterItem(int fid, int id, float x, float z)
    {
        FighterItem item;
        if (m_FighterItemPool.Count > 0)
        {
            item = m_FighterItemPool.Pop();
        }
        else
        {
            item = Instantiate(m_FighterItemPrefabs[id]);
        }
        item.transform.parent = CombatMgr.It.ObjRoot;
        item.transform.rotation = Quaternion.identity;
        item.transform.position = new Vector3(x, 0, z);
        item.transform.localScale = new Vector3(5, 5, 5);
        item.gameObject.SetActive(true);
        item.Fid = fid;
        item.name = "item_" + fid;
        item.OnCreate();
        return item;
    }

    public void ReleaseItem(FighterItem item)
    {
        item.OnDestroy();
        item.gameObject.SetActive(false);
        m_FighterItemPool.Push(item);
    }

    public void DestroyAllItems()
    {
        if (m_FighterItemPool.Count <= 0) return;
        FighterItem item;
        while (m_FighterItemPool.Count > 0)
        {
            item = m_FighterItemPool.Pop();
            if (item == null) break;
            item.OnDestroy();
            DestroyObject(item.gameObject);
        }
        m_FighterItemPool.Clear();
    }

    public FighterBullet CreateBullet(int fid, int id, float x, float z)
    {
        FighterBullet item = Instantiate(m_FighterBulletPrefabs[id]);
        item.transform.parent = CombatMgr.It.ObjRoot;
        item.transform.rotation = Quaternion.identity;
        item.transform.position = new Vector3(x, 0, z);
        item.transform.localScale = new Vector3(5, 5, 5);
        item.gameObject.SetActive(true);
        item.Fid = fid;
        item.OnCreate();
        return item;
    }

    public void ReleaseBullet(FighterBullet item)
    {
        item.OnDestroy();
        item.gameObject.SetActive(false);
        DestroyObject(item.gameObject);
    }
}