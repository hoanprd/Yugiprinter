using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUIItem : MonoBehaviour
{
    public Image cardImage;
    public TMP_Text statusText;

    public void UpdateStatus(string message)
    {
        if (statusText != null) statusText.text = message;
    }

    public void SetImage(Sprite sprite)
    {
        if (cardImage != null)
        {
            cardImage.sprite = sprite;
            // Chỉnh alpha về 1 nếu trước đó bạn để mờ
            var color = cardImage.color;
            color.a = 1f;
            cardImage.color = color;
        }
    }
}