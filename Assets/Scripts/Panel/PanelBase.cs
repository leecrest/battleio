using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelBase : MonoBehaviour {
    [HideInInspector]
    public int ID;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    virtual public void Show()
    {
        gameObject.SetActive(true);
    }

    virtual public void Hide()
    {
        gameObject.SetActive(false);
    }
}
