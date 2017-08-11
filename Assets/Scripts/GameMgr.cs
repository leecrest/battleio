using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnGameState {
    Ready = 0,
    Start,
    Over,
}

public class GameMgr : MonoBehaviour {
    public static GameMgr It;
    void Awake() { It = this; }

    [HideInInspector]
    public EnGameState State;

    public SceneMgr m_SceneMgr;
    public Text m_MsgText;

    // Use this for initialization
    void Start () {
        GameReady();
	}
	
	// Update is called once per frame
	void Update () {
		switch (State)
        {
            case EnGameState.Ready:
                if (Input.GetMouseButtonDown(0))
                {
                    GameStart();
                }
                break;
            default:
                break;
        }
	}

    public void GameReady()
    {
        State = EnGameState.Ready;
        m_MsgText.text = "READY!";
        m_SceneMgr.OnGameReady();
    }

    public void GameStart()
    {
        State = EnGameState.Start;
        m_MsgText.gameObject.SetActive(false);
        m_SceneMgr.OnGameStart();
    }

    public void GameOver()
    {
        State = EnGameState.Over;
        m_MsgText.text = "GameOver!";
        m_MsgText.gameObject.SetActive(true);
        m_SceneMgr.OnGameOver();
    }
}
