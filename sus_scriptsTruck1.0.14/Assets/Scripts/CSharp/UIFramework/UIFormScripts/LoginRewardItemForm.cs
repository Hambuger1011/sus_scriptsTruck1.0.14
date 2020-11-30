
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;


public class LoginRewardItemForm : MonoBehaviour {

    public GameObject NoUser, NowUser, HadUser;

    private int gameNumber = 0;
	
    void Update()
    {
        ShowState();
    }
    public void GameInite(int gameNumber)
    {
        this.gameNumber = gameNumber;
        ShowState();
    }

    //这个是用来显示该物体的状态的
    private void ShowState()
    {
        int loginDay = PlayerPrefs.GetInt("wardNumber");

#if USE_SERVER_DATA
        if (UserDataManager.Instance.selfBookInfo != null && UserDataManager.Instance.selfBookInfo.data != null)
        {
            loginDay = UserDataManager.Instance.selfBookInfo.data.loginday;
        }
#endif
        if (loginDay < gameNumber)
        {
            NoUser.SetActive(true);
            NowUser.SetActive(false);
            HadUser.SetActive(false);
        }
        else if (loginDay == gameNumber)
        {
            NoUser.SetActive(false);
            NowUser.SetActive(true);
            HadUser.SetActive(false);
        }
        else if (loginDay > gameNumber)
        {
            NoUser.SetActive(false);
            NowUser.SetActive(false);
            HadUser.SetActive(true);
        }
    }
}
