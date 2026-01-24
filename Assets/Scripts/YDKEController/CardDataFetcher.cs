using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System;
using TMPro;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class CardDataFetcher : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text resultText;
    public TMP_InputField ydkeInputField;
    public TMP_Text waitText;
    public GameObject downCardButton, printCardButton;

    [Header("Scroll View Settings")]
    public GameObject infoCardPrefab;
    public Transform infoPanel;
    public Transform errorPanel;

    [Header("Config")]
    public string apiUrl;
    public string saveDirectory;
    //public string deckListFolderSettingPath;
    //public string deckListFileSetting;
    public string deckListSettingPath;
    public string cardListSettingPath;

    private string[] tempArray;
    private string[] cardURL;
    public string printAppOpenPath;

    void Start()
    {
        // Khởi động nút tải thẻ
        saveDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\CardImageData";
        printAppOpenPath = Path.Combine(Application.streamingAssetsPath, "YugiprinterController.exe");
        deckListSettingPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\PRD Team\\YugipriterSetting";
        cardListSettingPath = Path.Combine(deckListSettingPath, "settingDeckString.txt");
    }

    /*public void DropdownValueChanged(TMP_Dropdown change)
    {
        int index = change.value;
        string selectedText = change.options[index].text;

        Debug.Log("Bạn đã chọn mục số: " + index + " có tên là: " + selectedText);

        if (selectedText == "Offline")
        {
            downCardButton.enabled = false;
        }
        else
        {
            downCardButton.enabled = true;
        }
    }*/

    public void OnDecodeAndFetchClicked()
    {
        string ydkeString = ydkeInputField.text.Trim();
        if (string.IsNullOrEmpty(ydkeString))
        {
            if (resultText != null) resultText.text = "Vui lòng nhập chuỗi YDKE.";
            return;
        }

        try
        {
            downCardButton.SetActive(false);
            TypedDeck deck = YdkeParser.ParseURL(ydkeString);
            List<int> allPasscodes = deck.main.Concat(deck.extra).Concat(deck.side)
                                        .Where(id => id != 0).ToList();

            if (allPasscodes.Count == 0)
            {
                resultText.text = "Không tìm thấy Passcode.";
                return;
            }

            tempArray = allPasscodes.Select(id => id.ToString()).ToArray();

            //string readDeckListPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + deckListFolderSettingPath + deckListFileSetting;

            Debug.Log(deckListSettingPath);

            try
            {
                if (!Directory.Exists(deckListSettingPath))
                {
                    Directory.CreateDirectory(deckListSettingPath);
                }

                if (!File.Exists(cardListSettingPath))
                {
                    File.WriteAllText(cardListSettingPath, string.Empty);
                }

                //File.WriteAllLines(readDeckListPath, tempArray);
                File.WriteAllLines(cardListSettingPath, tempArray);
                Debug.Log("Đã lưu danh sách Passcode vào: " + deckListSettingPath);
            }
            catch (Exception fileEx)
            {
                Debug.LogError("Lỗi khi ghi file setting: " + fileEx.Message);
            }

            cardURL = tempArray.Select(id => "https://images.ygoprodeck.com/images/cards/" + id + ".jpg").ToArray();

            // Xóa dữ liệu cũ ở cả 2 bảng
            foreach (Transform child in infoPanel) { Destroy(child.gameObject); }
            foreach (Transform child in errorPanel) { Destroy(child.gameObject); }

            waitText.text = "Đang bắt đầu tải...";
            StartCoroutine(DownloadImages());
        }
        catch (Exception e)
        {
            resultText.text = $"Lỗi: {e.Message}";
            downCardButton.SetActive(true);
        }
    }

    private IEnumerator DownloadImages()
    {
        if (!Directory.Exists(saveDirectory)) Directory.CreateDirectory(saveDirectory);

        for (int i = 0; i < cardURL.Length; i++)
        {
            string url = cardURL[i];
            string fileName = tempArray[i] + ".jpg";
            string savePath = Path.Combine(saveDirectory, fileName);

            // 1. Tạo Prefab mặc định ở infoPanel
            GameObject newCard = Instantiate(infoCardPrefab, infoPanel);
            CardUIItem uiItem = newCard.GetComponent<CardUIItem>();
            uiItem.UpdateStatus($"Đang tải {tempArray[i]}...");

            // 2. Bắt đầu tải (truyền cả GameObject vào để xử lý nếu lỗi)
            yield return StartCoroutine(DownloadAndDisplay(url, savePath, uiItem));
        }

        waitText.text = "Quá trình hoàn tất!";
        //downCardButton.SetActive(false);
        printCardButton.SetActive(true);
    }

    public void PrintCardControllerOpen()
    {
        string templateSource = Path.Combine(Application.streamingAssetsPath, "Doc1.docx");
        string templateDest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Doc1.docx");

        if (!File.Exists(templateDest))
            File.Copy(templateSource, templateDest, true);

        var startInfo = new ProcessStartInfo
        {
            FileName = printAppOpenPath,
            WorkingDirectory = Path.GetDirectoryName(templateDest)
        };
        Process.Start(startInfo);
        downCardButton.SetActive(true);
        printCardButton.SetActive(false);
    }

    private IEnumerator DownloadAndDisplay(string url, string savePath, CardUIItem uiItem)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            uiItem.transform.SetParent(errorPanel, false);
            uiItem.UpdateStatus("Lỗi: " + url + " - " + request.error);
            Debug.LogError($"Lỗi tải {url}: {request.error}");
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Sprite cardSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            uiItem.SetImage(cardSprite);
            uiItem.UpdateStatus("Hoàn tất");

            byte[] bytes = request.downloadHandler.data;
            File.WriteAllBytes(savePath, bytes);
        }
    }
}