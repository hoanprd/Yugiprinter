using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    void Start()
    {
        AppManager.Instance.PlayBGM(0, false);
        AppManager.Instance.PlayBGM(2, false);
        AppManager.Instance.PlayBGM(1, true);
    }

    public void InCardOpen()
    {
        SceneManager.LoadScene("InCardScene");
    }

    public void OpenAppSetting(string option)
    {
        if (AppManager.Instance == null)
        {
            Debug.LogWarning("OpenAppSetting: AppManager.Instance is null.");
            return;
        }

        switch (option)
        {
            case "OpenSetting":
                AppManager.Instance.AppSetting("OpenSetting");
                break;
            case "CloseSetting":
                AppManager.Instance.AppSetting("CloseSetting");
                break;
            default:
                Debug.LogWarning($"OpenAppSetting: Tùy chọn không xác định '{option}'");
                return;
        }
    }
}
