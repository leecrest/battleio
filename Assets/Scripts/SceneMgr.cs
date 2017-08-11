using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMgr : MonoBehaviour {
    public GameObject m_HeroPrefab;
    public Transform m_ObjRoot;

    private Stack<Hero> m_HeroPool;
    private Hero m_MainHero;

	// Use this for initialization
	void Start () {
        m_HeroPool = new Stack<Hero>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnGameReady()
    {
        CameraReset();
    }

    public void OnGameStart()
    {
        // 创建主角，并将镜头绑定到主角头上
        CreateHero(true, 0, 0);
        for (int i = 0; i < 20; i++)
        {
            CreateHero(false, Random.Range(-10f,10f), Random.Range(-10f,10f));
        }
    }

    public void OnGameOver()
    {
        CameraReset();
    }

    void CreateHero(bool isMain, float x, float z)
    {
        Hero hero = NewHero(x, z);
        hero.IsMain = isMain;
        if (isMain)
        {
            m_MainHero = hero;
            UpdateCamera();
        }
    }

    #region 主角对象池
    Hero NewHero(float x, float z)
    {
        Hero hero;
        if (m_HeroPool.Count > 0)
        {
            hero = m_HeroPool.Pop();
        }
        else
        {
            GameObject obj = Instantiate<GameObject>(m_HeroPrefab);
            hero = obj.GetComponent<Hero>();
            if (hero == null)
            {
                hero = obj.AddComponent<Hero>();
            }
        }
        hero.transform.parent = m_ObjRoot;
        hero.transform.rotation = Quaternion.identity;
        hero.transform.position = new Vector3(x, 0, z);
        hero.transform.localScale = new Vector3(5, 5, 5);
        hero.gameObject.SetActive(true);
        return hero;
    }

    void ReleaseHero(Hero hero)
    {
        hero.gameObject.SetActive(false);
        m_HeroPool.Push(hero);
        if (hero == m_MainHero)
        {
            m_MainHero = null;
        }
    }

    void DistroyAllHero()
    {
        if (m_HeroPool.Count <= 0) return;
        Hero hero;
        while (m_HeroPool.Count > 0)
        {
            hero = m_HeroPool.Pop();
            if (hero == null) break;
            DestroyObject(hero.gameObject);
        }
    }

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
}
