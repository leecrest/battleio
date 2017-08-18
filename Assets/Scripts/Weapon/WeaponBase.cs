using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnWeaponType
{
    Arrow = 1,      // 弓箭
    Blade,          // 刀

}




public class WeaponBase : MonoBehaviour {
    public int ID;
    public FighterHero Owner;

    public string IdleAnim;
    public string AttackAnim;

    // 子弹资源
    public ShellBase ShellPrefab;
    // 子弹资源编号
    public int ShellID = -1;
    // 子弹数量
    public int ShellCount = 0;
    

    public virtual void OnInit()
    {

    }

    public virtual void OnUninit()
    {

    }

    public virtual void OnAttach(FighterHero hero)
    {
        Owner = hero;
        transform.parent = hero.transform;

    }

    public virtual void OnDetach()
    {
        Owner = null;
        transform.parent = null;
    }
}
