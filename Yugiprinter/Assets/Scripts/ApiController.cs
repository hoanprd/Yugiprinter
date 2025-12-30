using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using TMPro;

public class ApiController : MonoBehaviour
{
    public TMP_Text waitText;
    public string apiUrl, saveDirectory;
    private string duongdanDoc, settingDecklistPath, settingSaveDownloadPath;
    public string[] tempArray;
    string[] imageUrlTemp, cardIdTemp;
    public string[] cardURL;

    private void Start()
    {
        /*duongdanDoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        settingSaveDownloadPath = duongdanDoc + @"\YugiohPrinterSetting\SettingValue1.txt";
        settingDecklistPath = duongdanDoc + @"\YugiohPrinterSetting\SettingValue7.txt";

        string fileContents = File.ReadAllText(settingDecklistPath);
        saveDirectory = File.ReadAllText(settingSaveDownloadPath);

        var lines = File.ReadLines(fileContents).Skip(1).ToArray();
        tempArray = new string[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] != "#main" && lines[i] != "#extra" && lines[i] != "!side" && lines[i] != "" && lines != null)
            {
                tempArray[i] = lines[i];
            }
        }*/



        List<string> list = new List<string>(tempArray);
        list.RemoveAll(string.IsNullOrEmpty);
        tempArray = list.ToArray();

        ApiManager.Instance.Get(apiUrl, OnDataReceived);
    }

    private void OnDataReceived(string jsonResponse)
    {
        if (string.IsNullOrEmpty(jsonResponse))
        {
            Debug.LogError("Failed to get data from API");
            return;
        }

        try
        {
            ApiResponse response = JsonUtility.FromJson<ApiResponse>(jsonResponse);
            int indexCount = 0;
            cardIdTemp = new string[20000];
            imageUrlTemp = new string[20000];
            cardURL = new string[tempArray.Length];

            if (response != null && response.data != null)
            {
                foreach (Datum card in response.data)
                {
                    if (card.card_images != null && card.card_images.Count > 0)
                    {
                        // Lấy ID và URL ảnh đầu tiên của mỗi thẻ
                        cardIdTemp[indexCount] = card.id;
                        imageUrlTemp[indexCount] = card.card_images[0].image_url;

                        indexCount++;
                        //Debug.Log($"Card ID: {cardId}, Image URL: {imageUrl}");
                    }
                }

                List<string> list = new List<string>(cardIdTemp);
                list.RemoveAll(string.IsNullOrEmpty);
                cardIdTemp = list.ToArray();

                for (int i = 0; i < tempArray.Length; i++)
                {
                    for (int j = 0; j < cardIdTemp.Length; j++)
                    {
                        if (tempArray[i] == cardIdTemp[j])
                        {
                            cardURL[i] = imageUrlTemp[j];
                        }
                    }
                }

                waitText.text = "Đã nhận dữ liệu và đang trong quá trình tải hình ảnh!";
                StartCoroutine(DownloadImages());
            }
            else
            {
                waitText.text = "Kiểm tra dữ liệu server thất bại!";
            }
        }
        catch (System.Exception ex)
        {
            waitText.text = "Lỗi không xác định";
            //Debug.LogError("Exception caught while parsing API response: " + ex.Message);
        }
    }

    private IEnumerator DownloadImages()
    {
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        for (int i = 0; i < cardURL.Length; i++)
        {
            string url = cardURL[i];
            string fileName = Path.GetFileName(url);
            string savePath = Path.Combine(saveDirectory, fileName);

            if (File.Exists(savePath))
            {
                continue;
            }

            yield return StartCoroutine(DownloadImage(url, savePath));
        }

        waitText.text = "Đã tải xong hình ảnh!";
        StartCoroutine(EndProgram());
    }

    private IEnumerator DownloadImage(string url, string savePath)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            File.WriteAllBytes(savePath, request.downloadHandler.data);
            Debug.Log("Image saved to: " + savePath);
        }
    }

    private IEnumerator EndProgram()
    {
        yield return new WaitForSeconds(2f);
        Application.Quit();
    }
}