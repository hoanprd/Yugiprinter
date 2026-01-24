using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject appSetting;
    public GameObject cardPrintSetting, otherSetting;
    public TMP_InputField imageDownPath;

    public string SelectedFolderPath { get; private set; } = string.Empty;

    public void InCardOpen()
    {
        Debug.Log("Chuyển sang InCardScene");
        SceneManager.LoadScene("InCardScene");
    }

    public void OpenFolderBrowser()
    {
        try
        {
            string initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            
            string[] paths = StandaloneFileBrowser.OpenFolderPanel("Chọn thư mục", initialDirectory, false);

            if (paths != null && paths.Length > 0)
            {
                SelectedFolderPath = paths[0];
                imageDownPath.text = SelectedFolderPath;
                Debug.Log("Selected folder: " + SelectedFolderPath);
            }
            else
            {
                Debug.Log("Folder selection cancelled or no folder chosen.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error opening folder browser: " + ex.Message);
        }
    }

    public void ChooseSettingOption(string option)
    {
        switch (option)
        {
            case "CardPrintSetting":
                cardPrintSetting.SetActive(true);
                otherSetting.SetActive(false);
                break;
            case "OtherSetting":
                cardPrintSetting.SetActive(false);
                otherSetting.SetActive(true);
                break;
            default:
                Debug.LogWarning("Unknown setting option: " + option);
                break;
        }
    }

    public void AppSetting(string option)
    {
        switch (option)
        {
            case "OpenSetting":
                appSetting.SetActive(true);
                break;
            case "CloseSetting":
                appSetting.SetActive(false);
                break;
            default:
                Debug.LogWarning("Unknown setting option: " + option);
                break;
        }
    }
}
