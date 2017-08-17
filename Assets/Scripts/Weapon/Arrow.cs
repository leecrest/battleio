using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : WeaponBase
{
    public override void OnAttach(FighterHero hero)
    {
        base.OnAttach(hero);
        transform.localPosition = new Vector3(-0.5f, -0.5f, 0f);
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
