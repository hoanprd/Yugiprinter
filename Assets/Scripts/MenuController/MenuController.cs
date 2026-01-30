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
        for (int i = 0; i < AppManager.Instance.bgmAudioSource.Length; i++)
        {
            AppManager.Instance.PlayBGM(i, false);
        }

        AppManager.Instance.PlayBGM(1, true);
    }

    public void InCardOpen()
    {
        AppManager.Instance.ChangeScene("InCardScene", 0.5f, true, 1f);
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
