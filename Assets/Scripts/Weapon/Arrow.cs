using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : WeaponBase
{
    private List<ArrowShell> m_Shells;

    public override void OnInit()
    {
        base.OnInit();
        m_Shells = new List<ArrowShell>();
    }

    public override void OnAttach(FighterHero hero)
    {
        base.OnAttach(hero);
        transform.localPosition = new Vector3(0.4f, 0f, 0f);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = new Vector3(1f, 1f, 1f);
        ShellCount = Random.Range(1, 10);
        FillShells();
    }

    public void FillShells()
    {
        int i = m_Shells.Count;
        if (i >= ShellCount) return;
        ArrowShell obj;
        for (; i < ShellCount; i++)
        {
            obj = Instantiate(ShellPrefab) as ArrowShell;
            obj.transform.parent = transform;
            obj.transform.localPosition = new Vector3(0.3f, 0f, 0f);
            m_Shells.Add(obj);
        }
        // 重新计算箭头的位置
        float angle = 60f / ShellCount;
        float start = -30f + angle/2;
        for (i = 0; i < ShellCount; i++)
        {
            obj = m_Shells[i];
            obj.transform.localPosition = new Vector3(0.3f, 0, start*-0.01f);
            obj.transform.localRotation = Quaternion.Euler(0, start, -90);
            start += angle;
        }
    }
}
