using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMgr : MonoBehaviour
{
    public static UIMgr It;
    void Awake() { It = this; }

    public PanelBase[] Panels;
    private int m_CurPanelID;



    public void OnInit()
    {
        PanelBase obj;
        for (int i = 0; i < Panels.Length; i++)
        {
            obj = Panels[i];
            obj.ID = i;
            obj.gameObject.SetActive(false);
        }
    }

    public void OnUninit()
    {

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
}