using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHome : UIBase {

    public Text m_TextGold;
    public Button m_BtnGold;
    public Text m_TextMoney;
    public Button m_BtnMoney;

    public Button m_BtnSetting;
    public Button m_BtnRoom;
    public Button m_BtnLimit;
    public Button m_BtnOnline;
    public Button m_BtnOffline;
    public Button m_BtnStart;
    public Button m_BtnPre;
    public Button m_BtnNext;
    public Button m_BtnName;

    public InputField m_InputName;
    public Text m_TextRoom;
    public Text m_MaxScore;
    public Text m_CurScore;
    

    public override void OnShow()
    {
        GameMgr.It.CombatReady();
        base.OnShow();
        m_TextGold.text = GameMgr.It.Gold.ToString();
        m_TextMoney.text = GameMgr.It.Money.ToString();
        m_MaxScore.text = "最高得分：" + GameMgr.It.ScoreMax;
        m_CurScore.text = "当前得分：" + GameMgr.It.ScoreCur;

        m_BtnGold.onClick.AddListener(OnBtnGold);
        m_BtnMoney.onClick.AddListener(OnBtnMoney);
        m_BtnSetting.onClick.AddListener(OnBtnSetting);
        m_BtnRoom.onClick.AddListener(OnBtnRoom);
        m_BtnLimit.onClick.AddListener(OnBtnLimit);
        m_BtnOnline.onClick.AddListener(OnBtnOnline);
        m_BtnOffline.onClick.AddListener(OnBtnOffline);
        m_BtnStart.onClick.AddListener(OnBtnStart);
        m_BtnPre.onClick.AddListener(OnBtnPre);
        m_BtnNext.onClick.AddListener(OnBtnNext);
        m_BtnName.onClick.AddListener(OnBtnName);
    }

    public override void OnHide()
    {
        base.OnHide();
        m_BtnGold.onClick.RemoveListener(OnBtnGold);
        m_BtnMoney.onClick.RemoveListener(OnBtnMoney);
        m_BtnSetting.onClick.RemoveListener(OnBtnSetting);
        m_BtnRoom.onClick.RemoveListener(OnBtnRoom);
        m_BtnLimit.onClick.RemoveListener(OnBtnLimit);
        m_BtnOnline.onClick.RemoveListener(OnBtnOnline);
        m_BtnOffline.onClick.RemoveListener(OnBtnOffline);
        m_BtnStart.onClick.RemoveListener(OnBtnStart);
        m_BtnPre.onClick.RemoveListener(OnBtnPre);
        m_BtnNext.onClick.RemoveListener(OnBtnNext);
        m_BtnName.onClick.RemoveListener(OnBtnName);
    }

    void OnBtnGold()
    {
        SoundMgr.It.OnButtonClick();
        GameMgr.It.Gold++;
        m_TextGold.text = GameMgr.It.Gold.ToString();
    }

    void OnBtnMoney()
    {
        SoundMgr.It.OnButtonClick();
        GameMgr.It.Money++;
        m_TextMoney.text = GameMgr.It.Money.ToString();
    }

    void OnBtnSetting()
    {
        SoundMgr.It.OnButtonClick();
    }

    void OnBtnRoom()
    {
        SoundMgr.It.OnButtonClick();
    }

    void OnBtnLimit()
    {
        SoundMgr.It.OnButtonClick();
    }

    void OnBtnOnline()
    {
        SoundMgr.It.OnButtonClick();
    }

    void OnBtnOffline()
    {
        SoundMgr.It.OnButtonClick();
    }

    void OnBtnStart()
    {
        SoundMgr.It.OnButtonClick();
        UIMgr.It.CloseUI("UIHome");
        UIMgr.It.OpenUI("UICombat");
        GameMgr.It.CombatStart();
    }

    void OnBtnPre()
    {
        SoundMgr.It.OnButtonClick();
    }

    void OnBtnNext()
    {
        SoundMgr.It.OnButtonClick();
    }

    void OnBtnName()
    {
        SoundMgr.It.OnButtonClick();
    }
}
