using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResMgr : MonoBehaviour
{
    public static ResMgr It;
    void Awake() { It = this; }

    // ������Դ
    public GameObject[] m_ScenePrefabs;
    // ������Դ
    public GameObject[] m_HeroPrefabs;
    // ������Դ
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
        // ��������ģ��
        GameObject obj = Instantiate(m_HeroPrefabs[id]);
        // ���ؽű�
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
        // ����ģ��
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