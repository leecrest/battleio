using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using LitJson;

/**
 * 缩放比例说明
 * 
 * 比如：地图宽40，高40
 * 则：地图中的有效点的范围是：(-20, 20)
 * 
 */

public class MapMaker : MonoBehaviour {

    [MenuItem("MyMenu/导入自定义地图...")]
    static void ImportFile()
    {
        var path = EditorUtility.OpenFilePanel("选择需要导入的json文件", Application.dataPath + "/Scenes", "json");
        if (path.Length == 0) return;
        var name = Path.GetFileNameWithoutExtension(path);

        Debug.Log("开始导入地图：" + name);
        StreamReader sr = new StreamReader(path);
        string content = sr.ReadToEnd();
        sr.Close();
        JsonReader jr = new JsonReader(content);
        JsonData jd = JsonMapper.ToObject(jr);
        if (!jd.IsObject)
        {
            Debug.LogError("这不是一个有效的json导出文件");
            return;
        }

        // 创建prefab
        int width = (int)jd["width"];
        int height = (int)jd["height"];
        JsonData block = jd["block"];
        if (block == null || !block.IsArray)
        {
            Debug.LogError("这不是一个有效的地图文件，没有 block 配置");
            return;
        }
        Debug.Log("开始创建预制件");
        GameObject root = GameObject.Find(name);
        if (root != null)
        {
            GameObject.DestroyImmediate(root, true);
        }
        root = new GameObject(name);

        // 创建地面

        // 创建墙壁
        GameObject obj;
        JsonData data;
        for (int i = 0; i < block.Count; i++)
        {
            data = block[i];
            obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.parent = root.transform;
            obj.transform.position = new Vector3((int)data["x"], 0, (int)data["z"]);
            obj.transform.localScale = new Vector3((float)(double)data["sx"], 1, (float)(double)data["sz"]);
        }
        

        /*for (int i = 0; i < layerdata.Count; i++)
        {
            JsonData layer = layerdata[i];
            GameObject layerObj = new GameObject();
            layerObj.name = (string)layer["name"];
            layerObj.SetActive(true);

        }*/

        // 创建prefab
        string filename = "Assets/Scenes/" + name + ".prefab";
        string filepath = Application.dataPath + "/Scenes/" + name + ".prefab";
        if (File.Exists(filepath))
        {
            File.Delete(filepath);
        }
        Object prefab = PrefabUtility.CreateEmptyPrefab(filename);
        PrefabUtility.ReplacePrefab(root, prefab);
        //GameObject.Destroy(root);
        Debug.Log("预制件创建完成：" + name);
        Debug.Log("地图导入完成");
    }
}
