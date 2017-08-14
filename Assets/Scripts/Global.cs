using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Global : MonoBehaviour {
    public static Global It;
    void Awake() { It = this; }



    #region 资源管理
    // 场景资源
    public GameObject[] m_ScenePrefabs;
    // 战斗单位资源
    public FighterHero[] m_FighterHeroPrefabs;
    // 道具资源
    public FighterItem[] m_FighterItemPrefabs;
    // 子弹资源
    public FighterBullet[] m_FighterBulletPrefabs;

    private Stack<FighterHero> m_FighterHeroPool;
    private Stack<FighterItem> m_FighterItemPool;
    private Stack<FighterBullet> m_FighterBulletPool;

    void InitPools()
    {
        m_FighterHeroPool = new Stack<FighterHero>();
        m_FighterItemPool = new Stack<FighterItem>();
        m_FighterBulletPool = new Stack<FighterBullet>();
    }

    public GameObject LoadScene(int id, Transform parent)
    {
        GameObject obj = Instantiate(m_ScenePrefabs[id]);
        obj.transform.rotation = Quaternion.identity;
        obj.transform.position = Vector3.zero;
        obj.transform.parent = parent;
        return obj;
    }

    public FighterHero NewFighterHero(int id, float x, float z, Transform parent)
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
        hero.transform.parent = parent;
        hero.transform.rotation = Quaternion.identity;
        hero.transform.position = new Vector3(x, 0, z);
        hero.transform.localScale = new Vector3(5, 5, 5);
        hero.gameObject.SetActive(true);
        hero.Init();
        return hero;
    }

    public void ReleaseHero(FighterHero hero)
    {
        hero.gameObject.SetActive(false);
        m_FighterHeroPool.Push(hero);
    }

    public void DistroyAllHeros()
    {
        if (m_FighterHeroPool.Count <= 0) return;
        FighterHero hero;
        while (m_FighterHeroPool.Count > 0)
        {
            hero = m_FighterHeroPool.Pop();
            if (hero == null) break;
            DestroyObject(hero.gameObject);
        }
        m_FighterHeroPool.Clear();
    }

    public FighterItem NewFighterItem(int id, float x, float z, Transform parent)
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
        item.transform.parent = parent;
        item.transform.rotation = Quaternion.identity;
        item.transform.position = new Vector3(x, 0, z);
        item.transform.localScale = new Vector3(5, 5, 5);
        item.gameObject.SetActive(true);
        return item;
    }

    public void ReleaseHero(FighterItem item)
    {
        item.gameObject.SetActive(false);
        m_FighterItemPool.Push(item);
    }

    public void DistroyAllItems()
    {
        if (m_FighterItemPool.Count <= 0) return;
        FighterItem item;
        while (m_FighterItemPool.Count > 0)
        {
            item = m_FighterItemPool.Pop();
            if (item == null) break;
            DestroyObject(item.gameObject);
        }
        m_FighterItemPool.Clear();
    }

    public FighterBullet NewFighterBullet(int id, float x, float z, Transform parent)
    {
        FighterBullet item;
        if (m_FighterBulletPool.Count > 0)
        {
            item = m_FighterBulletPool.Pop();
        }
        else
        {
            item = Instantiate(m_FighterBulletPrefabs[id]);
        }
        item.transform.parent = parent;
        item.transform.rotation = Quaternion.identity;
        item.transform.position = new Vector3(x, 0, z);
        item.transform.localScale = new Vector3(5, 5, 5);
        item.gameObject.SetActive(true);
        return item;
    }

    public void ReleaseBullet(FighterBullet item)
    {
        item.gameObject.SetActive(false);
        m_FighterBulletPool.Push(item);
    }

    public void DistroyAllBullets()
    {
        if (m_FighterBulletPool.Count <= 0) return;
        FighterBullet item;
        while (m_FighterBulletPool.Count > 0)
        {
            item = m_FighterBulletPool.Pop();
            if (item == null) break;
            DestroyObject(item.gameObject);
        }
        m_FighterBulletPool.Clear();
    }
    #endregion



    #region UI管理
    public PanelBase[] Panels;
    private int m_CurPanelID;

    void InitPanels()
    {
        PanelBase obj;
        for (int i = 0; i < Panels.Length; i++)
        {
            obj = Panels[i];
            obj.ID = i;
            obj.gameObject.SetActive(false);
        }
    }

    public void ShowPanel(int id)
    {
        PanelBase obj = Panels[id];
        obj.Show();
    }

    public void HidePanel(int id)
    {
        PanelBase obj = Panels[id];
        obj.Hide();
    }

    #endregion



    void Start()
    {
        InitPools();
        InitPanels();
        ShowPanel(0);
    }
}
