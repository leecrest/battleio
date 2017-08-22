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

public class MapConfig
{
    public int id;
    public string name;
    public int width;
    public int height;
    public GameObject prefab;
}

public class HeroConfig
{
    public int id;
    public string name;
    public GameObject prefab;
}

public class WeaponConfig
{
    public int id;
    public string name;
    public GameObject prefab;
    public int shell;
    public float atktime;       //攻击前摇
}

public class ShellConfig
{
    public int id;
    public GameObject prefab;
    public GameObject hole;
    public float speed;         // 基础飞行速度
    public float distance;      // 基础飞行距离
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
    private List<string> m_LoadList;
    private EnLoadState m_LoadState;

    public EnLoadState LoadState { get { return m_LoadState; } }
    public delegate void LoadCallback(string name, Object obj);
    #endregion

    #region 配置文件
    private Dictionary<string, object> m_Const;
    private MapConfig[] m_Map;
    private HeroConfig[] m_Hero;
    private WeaponConfig[] m_Weapon;
    private ShellConfig[] m_Shell;
    #endregion


    public void OnInit()
    {
        OnInitRes();
        OnInitConfig();
    }

    public void OnUninit()
    {
        OnUninitConfig();
        OnUninitRes();
    }



    #region 资源文件的加载管理
    void OnInitRes()
    {
        m_Objects = new Dictionary<string, Object>();
        m_LoadList = new List<string>();
        m_LoadState = EnLoadState.None;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform tf = transform.GetChild(i);
            tf.gameObject.SetActive(false);
            m_Objects.Add(tf.name, tf.gameObject);
        }
    }

    void OnUninitRes()
    {
        if (m_LoadList.Count > 0)
        {
            string name;
            for (int i = 0; i < m_LoadList.Count; i++)
            {
                name = m_LoadList[i];
                if (m_Objects.ContainsKey(name))
                {
                    Resources.UnloadAsset(m_Objects[name]);
                }
            }
        }
        m_LoadList.Clear();
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
            m_LoadList.Add(name);
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
            m_LoadList.Add(name);
        }
        if (cbFunc != null)
        {
            cbFunc(name, req.asset);
        }
        return true;
    }

    public void UnLoadResource(string name)
    {
        if (!m_Objects.ContainsKey(name) || !m_LoadList.Contains(name)) return;
        Resources.UnloadAsset(m_Objects[name]);
        m_Objects.Remove(name);
    }
    #endregion



    #region 配置文件的加载管理
    void OnInitConfig()
    {
        Object asset = Resources.Load("Config");
        if (asset == null) return;
        TextAsset textAsset = (TextAsset)asset;
        JsonData jsonD = JsonMapper.ToObject(textAsset.text);
        if (!jsonD.IsObject) return;

        // 常量
        JsonData data = jsonD["const"];
        m_Const = new Dictionary<string, object>();
        JsonData item;
        for (int i = 0; i < data.Count; i++)
        {
            item = data[i];
            JsonData tmp = item["value"];
            object value = null;
            if (tmp.IsBoolean) value = (bool)tmp;
            else if (tmp.IsDouble) value = (float)(double)tmp;
            else if (tmp.IsInt) value = (int)tmp;
            else if (tmp.IsLong) value = (long)tmp;
            if (value != null) m_Const.Add((string)item["name"], value);
        }

        // 地图
        data = jsonD["map"];
        m_Map = new MapConfig[data.Count];
        for (int i = 0; i < data.Count; i++)
        {
            item = data[i];
            MapConfig map = new MapConfig
            {
                id = (int)item["id"],
                name = (string)item["name"],
                width = (int)item["width"],
                height = (int)item["height"],
                prefab = ResMgr.It.GetResource((string)item["prefab"], true) as GameObject
            };
            m_Map[map.id - 1] = map;
        }

        // 角色
        data = jsonD["hero"];
        m_Hero = new HeroConfig[data.Count];
        for (int i = 0; i < data.Count; i++)
        {
            item = data[i];
            HeroConfig hero = new HeroConfig
            {
                id = (int)item["id"],
                name = (string)item["name"],
                prefab = ResMgr.It.GetResource((string)item["prefab"], true) as GameObject
            };
            m_Hero[hero.id - 1] = hero;
        }

        // 武器
        data = jsonD["weapon"];
        m_Weapon = new WeaponConfig[data.Count];
        for (int i = 0; i < data.Count; i++)
        {
            item = data[i];
            WeaponConfig weapon = new WeaponConfig
            {
                id = (int)item["id"],
                name = (string)item["name"],
                shell = (int)item["shell"],
                atktime = (float)(double)item["atktime"],
                prefab = ResMgr.It.GetResource((string)item["prefab"], true) as GameObject
            };
            m_Weapon[weapon.id - 1] = weapon;
        }

        // 子弹
        data = jsonD["shell"];
        m_Shell = new ShellConfig[data.Count];
        for (int i = 0; i < data.Count; i++)
        {
            item = data[i];
            ShellConfig shell = new ShellConfig
            {
                id = (int)item["id"],
                speed = (float)(double)item["speed"],
                distance = (float)(double)item["distance"],
                prefab = ResMgr.It.GetResource((string)item["prefab"], true) as GameObject,
                hole = ResMgr.It.GetResource((string)item["hole"], true) as GameObject
            };
            m_Shell[shell.id - 1] = shell;
        }
    }

    void OnUninitConfig()
    {
        m_Const = null;
    }

    public object GetConst(string key)
    {
        if (!m_Const.ContainsKey(key)) return null;
        return m_Const[key];
    }

    public MapConfig GetMapConfig(int id)
    {
        return m_Map[id - 1];
    }

    public HeroConfig GetHeroConfig(int id)
    {
        return m_Hero[id - 1];
    }

    public WeaponConfig GetWeaponConfig(int id)
    {
        return m_Weapon[id - 1];
    }

    public int GetWeaponCount()
    {
        return m_Weapon.Length;
    }

    public ShellConfig GetShellConfig(int id)
    {
        return m_Shell[id - 1];
    }


    public GameObject LoadScene(int id, Transform parent)
    {
        MapConfig cfg = m_Map[id - 1];
        GameObject obj = Instantiate(cfg.prefab);
        obj.transform.rotation = Quaternion.identity;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = new Vector3(1, 1.5f, 1);
        obj.transform.parent = parent;
        obj.SetActive(true);
        return obj;
    }

    public void ReleaseScene(GameObject scene)
    {
        DestroyObject(scene);
    }

    public FighterHero CreateHero(int fid, int id, float x, float z)
    {
        HeroConfig cfg = m_Hero[id - 1];
        GameObject obj = Instantiate(cfg.prefab);
        FighterHero hero = obj.GetComponent<FighterHero>();
        hero.transform.parent = CombatMgr.It.ObjRoot;
        hero.transform.rotation = Quaternion.identity;
        hero.transform.position = new Vector3(x, 2, z);
        hero.transform.localScale = new Vector3(2, 2, 2);
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

    public WeaponBase CreateWeapon(int id)
    {
        WeaponConfig cfg = m_Weapon[id - 1];
        GameObject obj = Instantiate(cfg.prefab);
        WeaponBase weapon = obj.GetComponent<WeaponBase>();
        weapon.gameObject.SetActive(true);
        weapon.id = id;
        weapon.name = "weapon_" + id;
        weapon.OnInit();
        return weapon;
    }

    public void ReleaseWeapon(WeaponBase obj)
    {
        obj.OnDetach();
        obj.gameObject.SetActive(false);

        obj.OnUninit();
        DestroyObject(obj.gameObject);
    }

    public ShellBase CreateShell(int weaponid, Transform parent)
    {
        WeaponConfig weapon = m_Weapon[weaponid - 1];
        ShellConfig cfg = m_Shell[weapon.shell - 1];
        GameObject obj = Instantiate(cfg.prefab);
        ShellBase shell = obj.GetComponent<ShellBase>();
        shell.gameObject.SetActive(true);
        shell.transform.parent = parent;
        shell.transform.localPosition = Vector3.zero;
        shell.transform.localRotation = Quaternion.identity;
        shell.id = cfg.id;
        shell.weaponid = weaponid;
        shell.name = "shell_" + cfg.id;
        shell.OnInit();
        return shell;
    }

    public void ReleaseShell(ShellBase shell)
    {
        shell.OnDetach();
        shell.gameObject.SetActive(false);
        shell.OnUninit();
        DestroyObject(shell.gameObject);
    }

    #endregion
}