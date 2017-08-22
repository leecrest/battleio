using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour {
    [HideInInspector]
    public int ID;

	public virtual void OnInit () {
		
	}
	
    public virtual void OnUninit()
    {

    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag != "hero") return;
        FighterHero hero = collider.GetComponent<FighterHero>();
        if (OnAttachHero(hero))
        {
            ResMgr.It.ReleaseItem(this);
        }
    }

    // 返回true/false，表示是否需要销毁道具
    public virtual bool OnAttachHero(FighterHero hero)
    {
        return true;
    }
}
