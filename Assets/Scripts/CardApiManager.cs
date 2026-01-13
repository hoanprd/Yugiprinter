using UnityEngine;
using UnityEngine.Networking; // Bắt buộc phải có thư viện này
using System.Collections;

public class CardApiManager : MonoBehaviour
{
    private string url = "https://db.ygoprodeck.com/api/v7/cardinfo.php";

    void Start()
    {
        // Bắt đầu gọi API
        StartCoroutine(GetCardData());
    }

    IEnumerator GetCardData()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Gửi request và đợi phản hồi
            yield return webRequest.SendWebRequest();

            // KIỂM TRA TRẠNG THÁI THÀNH CÔNG HAY THẤT BẠI
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Lỗi kết nối hoặc xử lý: " + webRequest.error);
                    break;

                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("Lỗi HTTP: " + webRequest.responseCode + " - " + webRequest.error);
                    break;

                case UnityWebRequest.Result.Success:
                    Debug.Log("GỌI API THÀNH CÔNG!");
                    // In ra một phần dữ liệu để kiểm tra (vì dữ liệu API này rất lớn)
                    string jsonResponse = webRequest.downloadHandler.text;
                    Debug.Log("Dữ liệu nhận được (100 ký tự đầu): " + jsonResponse.Substring(0, 100));
                    break;
            }
        }
    }
}