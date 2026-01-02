using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System;
using TMPro;
using System.IO;

public class CardDataFetcher : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text resultText;
    public TMP_InputField ydkeInputField;
    public TMP_Text waitText;
    public Button downCardButton, printCardButton;

    [Header("Scroll View Settings")]
    public GameObject infoCardPrefab; // Prefab có gắn CardUIItem
    public Transform infoPanel;      // Content của Scroll View thành công
    public Transform errorPanel;     // Content của Scroll View thất bại (MỚI)

    [Header("Config")]
    public string apiUrl;
    public string saveDirectory;

    private string[] tempArray;
    private string[] cardURL;

    public void DropdownValueChanged(TMP_Dropdown change)
    {
        int index = change.value; // Lấy vị trí (0, 1, 2...)
        string selectedText = change.options[index].text; // Lấy chữ hiển thị

        Debug.Log("Bạn đã chọn mục số: " + index + " có tên là: " + selectedText);

        // Thực hiện lệnh của bạn tại đây
        if (selectedText == "Offline")
        {
            downCardButton.enabled = false;
        }
        else
        {
            downCardButton.enabled = true;
        }    
    }

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
            TypedDeck deck = YdkeParser.ParseURL(ydkeString);
            List<int> allPasscodes = deck.main.Concat(deck.extra).Concat(deck.side)
                                        .Where(id => id != 0).Distinct().ToList();

            if (allPasscodes.Count == 0)
            {
                resultText.text = "Không tìm thấy Passcode.";
                return;
            }

            tempArray = allPasscodes.Select(id => id.ToString()).ToArray();
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
    }

    private IEnumerator DownloadAndDisplay(string url, string savePath, CardUIItem uiItem)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            // THẤT BẠI: Đưa object sang errorPanel
            uiItem.transform.SetParent(errorPanel, false);
            uiItem.UpdateStatus("Lỗi: " + url + " - " + request.error); // Hoặc hiện ID lỗi
            Debug.LogError($"Lỗi tải {url}: {request.error}");
        }
        else
        {
            // THÀNH CÔNG: Hiển thị ảnh
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Sprite cardSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            uiItem.SetImage(cardSprite);
            uiItem.UpdateStatus("Hoàn tất");

            // Lưu file
            byte[] bytes = request.downloadHandler.data;
            File.WriteAllBytes(savePath, bytes);
        }
    }
}