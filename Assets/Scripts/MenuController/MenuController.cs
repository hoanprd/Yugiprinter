using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void InCardOpen()
    {
        Debug.Log("Chuyển sang InCardScene");
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
