using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowShell : ShellBase
{

    void Start()
    {
        m_DropAngleX = 30;
        m_DropPosY = 1.4f;
    }

    protected override void OnGroundHole(Transform hole)
    {
        hole.localPosition = new Vector3(0, 0.024f, 0.417f);
        hole.localScale = Vector3.one;
        hole.localRotation = Quaternion.Euler(60,0,0);
    }

    protected override void OnWallHole(Transform hole)
    {
        hole.localPosition = new Vector3(0, 0, 0.33f);
        hole.localScale = Vector3.one;
        hole.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
