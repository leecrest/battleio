using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

//资源管理
//可加载AssetBundle，也可以加载本地的Prefabs等
//说明，如果有资源打包成AssetBundle形式，文件名称需要相对固定

//加载状态
public enum EnLoadState
{
    None = 0,
    Load,
    Success,
    Fail,
}

//资源定义
public struct STResConfig
{
    public string sName;    //资源名称，也是文件名称
    public bool bLoadStart; //是否启动时加载
    public bool bStayMem;   //是否常驻内存
}



public class ResMgr : MonoBehaviour
{
    public static ResMgr It;
    void Awake() { It = this; }

    #region 非配置型常量
    public GUISkin MySkin;
    public Texture2D TexBloodSelf;
    public Texture2D TexBloodFriend;    // 友方血条前景
    public Texture2D TexBloodEnemy;     // 敌方血条前景
    public Texture2D TexBloodBg;        // 血条背景
    #endregion

    #region 资源文件
    private Dictionary<string, Object> m_Objects;
    private EnLoadState m_LoadState;

    public EnLoadState LoadState { get { return m_LoadState; } }
    public delegate void LoadCallback(string name, Object obj);

    // 资源池
    private Dictionary<string, Stack<GameObject>> m_Pool;
    #endregion



    #region 资源文件的加载管理
    public void OnInit()
    {
        m_Objects = new Dictionary<string, Object>();
        m_LoadState = EnLoadState.None;
        m_Pool = new Dictionary<string, Stack<GameObject>>();
    }

    public void OnUninit()
    {
        foreach (var stack in m_Pool.Values)
        {
            foreach (var obj in stack)
            {
                DestroyObject(obj);
            }
            stack.Clear();
        }
        m_Pool.Clear();

        foreach (var obj in m_Objects.Values)
        {
            Resources.UnloadAsset(obj);
        }
        m_Objects.Clear();
    }

    // 预加载基础资源
    public void PreLoad(LoadCallback callback)
    {
        Object asset = LoadResource("preload", false);
        if (asset == null) return;
        TextAsset textAsset = (TextAsset)asset;
        LitJson.JsonReader jsonR = new LitJson.JsonReader(textAsset.text);
        LitJson.JsonData jsonD = LitJson.JsonMapper.ToObject(jsonR);
        if (!jsonD.IsArray || jsonD.Count == 0) return;
        // 逐个加载资源，全部加载完毕后执行回调函数
        for (int i = 0; i < jsonD.Count; i++)
        {
            LoadResource((string)jsonD[i]["name"], true);
        }
        if (callback != null)
        {
            callback(null, null);
        }
    }

    public Object GetResource(string name, bool load = true, System.Type type = null)
    {
        if (m_Objects.ContainsKey(name))
        {
            return m_Objects[name];
        }
        if (!load) return null;
        return LoadResource(name, load, type);
    }

    public Object LoadResource(string name, bool save = true, System.Type type = null)
    {
        Object asset;
        if (type == null)
        {
            asset = Resources.Load(name);
        }
        else
        {
            asset = Resources.Load(name, type);
        }
        if (save && asset != null)
        {
            m_Objects[name] = asset;
        }
        return asset;
    }

    public bool LoadResourceAsync(string name, LoadCallback cbFunc, bool save = true)
    {
        Object obj = GetResource(name, false);
        if (obj != null)
        {
            if (cbFunc != null)
            {
                cbFunc(name, obj);
            }
            return true;
        }

        ResourceRequest req = Resources.LoadAsync(name);
        if (save && req.asset != null)
        {
            m_Objects[name] = req.asset;
        }
        if (cbFunc != null)
        {
            cbFunc(name, req.asset);
        }
        return true;
    }

    public void UnLoadResource(string name)
    {
        if (!m_Objects.ContainsKey(name)) return;
        Resources.UnloadAsset(m_Objects[name]);
        m_Objects.Remove(name);
    }
    #endregion

    #region 资源池
    public GameObject CreatePrefab(string name)
    {
        if (m_Pool.ContainsKey(name))
        {
            var stack = m_Pool[name];
            if (stack.Count > 0)
            {
                return stack.Pop();
            }
        }
        GameObject obj = GetResource(name, true) as GameObject;
        return Instantiate(obj);
    }

    public void ReleasePrefab(string name, GameObject obj)
    {
        obj.SetActive(false);
        if (m_Pool.ContainsKey(name))
        {
            var stack = m_Pool[name];
            stack.Push(obj);
        }
        else
        {
            Stack<GameObject> stack = new Stack<GameObject>();
            stack.Push(obj);
            m_Pool.Add(name, stack);
        }
    }

    #endregion


    #region 配置加载
    public GameObject LoadScene(int id, Transform parent)
    {
        MapConfig cfg = Config.MapCfg[id - 1];
        GameObject obj = CreatePrefab(cfg.prefab);
        obj.transform.rotation = Quaternion.identity;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = new Vector3(1, 1.5f, 1);
        obj.transform.parent = parent;
        obj.SetActive(true);
        return obj;
    }

    public void ReleaseScene(int id, GameObject scene)
    {
        MapConfig cfg = Config.MapCfg[id - 1];
        ReleasePrefab(cfg.prefab, scene);
    }

    public FighterHero CreateHero(int id, float x, float z, int fid, string name, int hp, int maxhp)
    {
        HeroConfig cfg = Config.HeroCfg[id - 1];
        GameObject obj = CreatePrefab(cfg.prefab);
        FighterHero hero = obj.GetComponent<FighterHero>();
        hero.transform.parent = CombatMgr.It.ObjRoot;
        hero.transform.rotation = Quaternion.identity;
        hero.transform.position = new Vector3(x, 2, z);
        hero.transform.localScale = new Vector3(2, 2, 2);
        hero.gameObject.SetActive(true);
        hero.Fid = fid;
        hero.ID = id;
        hero.Name = name;
        hero.name = "hero_" + fid;
        hero.OnInit(hp, maxhp);
        return hero;
    }

    public void ReleaseHero(FighterHero hero)
    {
        HeroConfig cfg = Config.HeroCfg[hero.ID - 1];
        hero.OnUninit();
        ReleasePrefab(cfg.prefab, hero.gameObject);
    }

    public WeaponBase CreateWeapon(int id)
    {
        WeaponConfig cfg = Config.WeaponCfg[id - 1];
        GameObject obj = CreatePrefab(cfg.prefab);
        WeaponBase weapon = obj.GetComponent<WeaponBase>();
        weapon.gameObject.SetActive(true);
        weapon.ID = id;
        weapon.name = "weapon_" + id;
        weapon.OnInit();
        return weapon;
    }

    public void ReleaseWeapon(WeaponBase obj)
    {
        WeaponConfig cfg = Config.WeaponCfg[obj.ID - 1];
        obj.OnDetach();
        obj.OnUninit();
        ReleasePrefab(cfg.prefab, obj.gameObject);
    }

    public ShellBase CreateShell(int weaponid, Transform parent)
    {
        WeaponConfig weapon = Config.WeaponCfg[weaponid - 1];
        ShellConfig cfg = Config.ShellCfg[weapon.shell - 1];
        GameObject obj = CreatePrefab(cfg.prefab);
        ShellBase shell = obj.GetComponent<ShellBase>();
        shell.gameObject.SetActive(true);
        shell.transform.parent = parent;
        shell.transform.localPosition = Vector3.zero;
        shell.transform.localRotation = Quaternion.identity;
        shell.ID = cfg.id;
        shell.WeaponID = weaponid;
        shell.name = "shell_" + cfg.id;
        shell.OnInit();
        return shell;
    }

    public void ReleaseShell(ShellBase shell)
    {
        ShellConfig cfg = Config.ShellCfg[shell.ID - 1];
        if (shell.Hole != null) ReleasePrefab(cfg.hole, shell.Hole);
        shell.OnDetach();
        shell.OnUninit();
        ReleasePrefab(cfg.prefab, shell.gameObject);
    }

    public GameObject CreateShellHole(int id)
    {
        ShellConfig cfg = Config.ShellCfg[id - 1];
        GameObject obj = CreatePrefab(cfg.prefab);
        return obj;
    }



    public ItemBase CreateItem(int id, float x, float z, Transform parent = null)
    {
        ItemConfig cfg = Config.ItemCfg[id - 1];
        GameObject obj = CreatePrefab(cfg.prefab);
        ItemBase item = obj.GetComponent<ItemBase>();
        item.gameObject.SetActive(true);
        if (parent == null) parent = CombatMgr.It.ObjRoot;
        item.transform.parent = parent;
        item.transform.localPosition = new Vector3(x, cfg.y, z);
        item.ID = cfg.id;
        item.name = "item_" + cfg.id;
        item.OnInit();
        return item;
    }

    public void ReleaseItem(ItemBase item)
    {
        ItemConfig cfg = Config.ItemCfg[item.ID - 1];
        item.OnUninit();
        ReleasePrefab(cfg.prefab, item.gameObject);
    }

    #endregion
}