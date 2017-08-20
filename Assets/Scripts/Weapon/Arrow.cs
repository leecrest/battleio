using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : WeaponBase
{
    void Start()
    {
        Name = "arrow";
        HasShell = true;
        ShellBack = false;
        ShellCount = 1;
        ShellSpeed = 3f;
        ShellDistance = 10f;
    }

    public override void OnAttach(FighterHero hero)
    {
        base.OnAttach(hero);
        transform.localPosition = new Vector3(0.25f, 0f, 0f);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = Vector3.one;
        LoadShell();
    }

    public override void LoadShell()
    {
        base.LoadShell();
        ShellBase obj;
        // 重新计算箭头的位置
        float angle = 90f / (ShellCount+1);
        float start = 45f + angle;
        for (int i = 0; i < ShellCount; i++)
        {
            obj = m_Shells[i];
            obj.transform.localPosition = new Vector3(-0.1f, 0, 0);
            obj.transform.localRotation = Quaternion.Euler(0, start, 0);
            obj.transform.localScale = Vector3.one;
            start += angle;
        }
    }
}
