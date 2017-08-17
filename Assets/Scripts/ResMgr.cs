using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResMgr : MonoBehaviour
{
    public static ResMgr It;
    void Awake() { It = this; }

    // 场景资源
    public GameObject[] m_ScenePrefabs;
    // 人物资源
    public GameObject[] m_HeroPrefabs;
    // 武器资源
    public WeaponBase[] m_WeaponPrefabs;

    public void OnInit()
    {
    }

    public void OnUninit()
    {
    }

    public GameObject LoadScene(int id, Transform parent)
    {
        GameObject obj = Instantiate(m_ScenePrefabs[id]);
        obj.transform.rotation = Quaternion.identity;
        obj.transform.position = Vector3.zero;
        obj.transform.parent = parent;
        obj.SetActive(true);
        return obj;
    }

    public FighterHero NewHero(int fid, int id, float x, float z)
    {
        // 加载人物模型
        GameObject obj = Instantiate(m_HeroPrefabs[id]);
        // 加载脚本
        FighterHero hero = obj.AddComponent<FighterHero>();
        hero.transform.parent = CombatMgr.It.ObjRoot;
        hero.transform.rotation = Quaternion.identity;
        hero.transform.position = new Vector3(x, 2, z);
        hero.transform.localScale = new Vector3(1, 1, 1);
        hero.gameObject.SetActive(true);
        hero.Fid = fid;
        hero.name = "hero_" + fid;
        hero.OnInit();
        return hero;
    }

    public void ReleaseHero(FighterHero hero)
    {
        hero.OnUninit();
        hero.gameObject.SetActive(false);
        DestroyObject(hero.gameObject);
    }

    public WeaponBase NewWeapon(int id)
    {
        // 加载模型
        WeaponBase obj = Instantiate(m_WeaponPrefabs[id]);
        obj.gameObject.SetActive(true);
        obj.ID = id;
        obj.OnInit();
        return obj;
    }

    public void ReleaseWeapon(WeaponBase obj)
    {
        obj.OnUninit();
        obj.gameObject.SetActive(false);
        DestroyObject(obj.gameObject);
    }
}