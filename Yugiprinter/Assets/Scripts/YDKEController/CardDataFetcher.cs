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

    [Header("Scroll View Settings")]
    public GameObject infoCardPrefab; // Prefab có gắn CardUIItem
    public Transform infoPanel;      // Content của Scroll View

    [Header("Config")]
    public string apiUrl;
    public string saveDirectory;

    private string[] tempArray;
    private string[] cardURL;

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

            // Xóa các card cũ trong Scroll View nếu có
            foreach (Transform child in infoPanel) { Destroy(child.gameObject); }

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

            // 1. Tạo Prefab trong Scroll View
            GameObject newCard = Instantiate(infoCardPrefab, infoPanel);
            CardUIItem uiItem = newCard.GetComponent<CardUIItem>();
            uiItem.UpdateStatus("Đang tải...");

            // 2. Bắt đầu tải hình ảnh
            yield return StartCoroutine(DownloadAndDisplay(url, savePath, uiItem));
        }

        waitText.text = "Đã tải xong tất cả!";
    }

    private IEnumerator DownloadAndDisplay(string url, string savePath, CardUIItem uiItem)
    {
        // Sử dụng UnityWebRequestTexture để lấy ảnh trực tiếp
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            uiItem.UpdateStatus("Lỗi tải ảnh");
            Debug.LogError(request.error);
        }
        else
        {
            // Lấy Texture và hiển thị lên UI
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Sprite cardSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            uiItem.SetImage(cardSprite);
            uiItem.UpdateStatus("Hoàn tất");

            // Lưu vào máy (chạy ngầm để không giật lag UI)
            byte[] bytes = request.downloadHandler.data;
            File.WriteAllBytes(savePath, bytes);
        }
    }
}