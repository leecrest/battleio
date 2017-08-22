using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daliandao : WeaponBase
{
    public override void OnAttach(FighterHero hero)
    {
        base.OnAttach(hero);
        transform.localPosition = new Vector3(0.2f, -0.1f, 0f);
        transform.localRotation = Quaternion.Euler(90, 90, 0);
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
