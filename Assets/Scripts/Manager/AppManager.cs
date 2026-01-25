using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class AppManager : MonoBehaviour
{
    public static AppManager Instance;

    public GameObject appSetting;
    public GameObject cardPrintSetting, otherSetting;
    public TMP_InputField imageDownPath;
    public Toggle printCloseToggle;

    // Kept the existing backing property name to avoid breaking other code.
    // It's recommended to rename this to `SelectedFolderPath` (PascalCase) later.
    public string selectedFolderPath { get; private set; } = string.Empty;
    public bool printCloseToCard { get; private set; } = false;

    // Events so other scripts can react to changes instead of polling.
    public event Action<string> OnSelectedFolderPathChanged;
    public event Action<bool> OnPrintCloseToCardChanged;

    private readonly string settingsFolderPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PRD Team", "YugipriterSetting");
    private readonly string settingsFileName = "appSetting.txt";
    private string settingsFilePath => Path.Combine(settingsFolderPath, settingsFileName);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
            // Subscribe to toggle changes if assigned
            if (printCloseToggle != null)
            {
                printCloseToggle.onValueChanged.AddListener(SetPrintCloseToCard);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid potential leaks
        if (printCloseToggle != null)
        {
            printCloseToggle.onValueChanged.RemoveListener(SetPrintCloseToCard);
        }
    }

    private void LoadSettings()
    {
        try
        {
            // Ensure folder exists
            if (!Directory.Exists(settingsFolderPath))
            {
                Directory.CreateDirectory(settingsFolderPath);
            }

            // If no settings file, create default settings and save
            if (!File.Exists(settingsFilePath))
            {
                // Default values
                selectedFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CardImageData");
                printCloseToCard = false;

                // Ensure default folder exists
                try
                {
                    if (!Directory.Exists(selectedFolderPath))
                    {
                        Directory.CreateDirectory(selectedFolderPath);
                    }
                }
                catch (Exception dirEx)
                {
                    Debug.LogWarning("Could not create default CardImageData folder: " + dirEx.Message);
                }

                SaveSettings();
                Debug.Log("Settings file not found. Created default settings at: " + settingsFilePath);
            }
            else
            {
                // Read and parse key:value lines
                string[] lines = File.ReadAllLines(settingsFilePath);
                foreach (var raw in lines)
                {
                    if (string.IsNullOrWhiteSpace(raw)) continue;
                    var line = raw.Trim();
                    var separatorIndex = line.IndexOf(':');
                    if (separatorIndex < 0) continue;

                    var key = line.Substring(0, separatorIndex).Trim();
                    var value = line.Substring(separatorIndex + 1).Trim();

                    if (key.Equals("SelectedFolderPath", StringComparison.OrdinalIgnoreCase))
                    {
                        selectedFolderPath = value;
                    }
                    else if (key.Equals("PrintCloseToCard", StringComparison.OrdinalIgnoreCase))
                    {
                        if (bool.TryParse(value, out var b)) printCloseToCard = b;
                    }
                }

                // If no path read from file, fallback to default
                if (string.IsNullOrEmpty(selectedFolderPath))
                {
                    selectedFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "CardImageData");
                }

                Debug.Log("Loaded settings. Path=" + selectedFolderPath + " PrintCloseToCard=" + printCloseToCard);
            }

            // Update UI if assigned (don't trigger toggle change callback)
            if (imageDownPath != null)
            {
                imageDownPath.SetTextWithoutNotify(selectedFolderPath);
            }

            if (printCloseToggle != null)
            {
                // Use SetIsOnWithoutNotify so we don't trigger SetPrintCloseToCard while loading
                printCloseToggle.SetIsOnWithoutNotify(printCloseToCard);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error loading settings: " + ex.Message);
        }
    }

    private void SaveSettings()
    {
        try
        {
            if (!Directory.Exists(settingsFolderPath))
            {
                Directory.CreateDirectory(settingsFolderPath);
            }

            var lines = new List<string>
            {
                $"SelectedFolderPath: {selectedFolderPath}",
                $"PrintCloseToCard: {printCloseToCard}"
            };

            File.WriteAllLines(settingsFilePath, lines);
            Debug.Log("Saved settings to " + settingsFilePath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error saving settings: " + ex.Message);
        }
    }

    public void OpenFolderBrowser()
    {
        try
        {
            string initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string[] paths = StandaloneFileBrowser.OpenFolderPanel("Chọn thư mục", initialDirectory, false);

            if (paths != null && paths.Length > 0)
            {
                // Use helper to centralize side-effects (UI update + save + event)
                UpdateSelectedFolderPath(paths[0], true);
                Debug.Log("Selected folder: " + selectedFolderPath);
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

    // Centralized method to set folder programmatically (validates, updates UI, persists, notifies).
    public void UpdateSelectedFolderPath(string path, bool save = true)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            Debug.LogWarning("UpdateSelectedFolderPath: provided path is null/empty.");
            return;
        }

        // normalize path as needed (optional)
        selectedFolderPath = path;

        // Update UI without invoking listeners
        if (imageDownPath != null)
        {
            imageDownPath.SetTextWithoutNotify(selectedFolderPath);
        }

        // Ensure the folder exists
        try
        {
            if (!Directory.Exists(selectedFolderPath))
            {
                Directory.CreateDirectory(selectedFolderPath);
            }
        }
        catch (Exception dirEx)
        {
            Debug.LogWarning("Could not ensure selected folder exists: " + dirEx.Message);
        }

        if (save)
        {
            SaveSettings();
        }

        OnSelectedFolderPathChanged?.Invoke(selectedFolderPath);
    }

    // Call this from UI (e.g., toggle) to change the print-close-to-card option and persist it.
    // This method is used as the toggle callback (onValueChanged).
    public void SetPrintCloseToCard(bool enabled)
    {
        if (printCloseToCard == enabled) return;

        printCloseToCard = enabled;
        SaveSettings();
        OnPrintCloseToCardChanged?.Invoke(printCloseToCard);
        Debug.Log("PrintCloseToCard set to: " + printCloseToCard);
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