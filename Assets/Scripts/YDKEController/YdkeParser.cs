using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

// =========================================================================
// YDKE PARSER
// =========================================================================
public static class YdkeParser
{
    /// <summary>
    /// Chuyển đổi chuỗi Base64 (URL-safe) thành danh sách Passcodes (int 32-bit).
    /// </summary>
    public static List<int> Base64ToPasscodes(string base64)
    {
        try
        {
            // Chuyển URL-safe Base64 thành Base64 tiêu chuẩn
            string standardBase64 = base64.Replace('-', '+').Replace('_', '/');

            // Bù padding nếu cần
            while (standardBase64.Length % 4 != 0)
            {
                standardBase64 += '=';
            }

            byte[] bytes = Convert.FromBase64String(standardBase64);
            var passcodes = new List<int>();

            for (int i = 0; i < bytes.Length; i += 4)
            {
                if (i + 4 <= bytes.Length)
                {
                    int passcode = BitConverter.ToInt32(bytes, i);
                    passcodes.Add(passcode);
                }
            }
            return passcodes;
        }
        catch (Exception e)
        {
            Debug.LogError($"Lỗi giải mã Base64 cho đoạn '{base64}': {e.Message}");
            return new List<int>();
        }
    }

    /// <summary>
    /// Phân tích chuỗi YDKE URL thành TypedDeck.
    /// </summary>
    public static TypedDeck ParseURL(string ydke)
    {
        const string protocol = "ydke://";
        if (!ydke.StartsWith(protocol))
        {
            throw new Exception("Unrecognized URL protocol. Must start with 'ydke://'.");
        }

        string componentString = ydke.Substring(protocol.Length);

        // Loại bỏ '!' cuối
        if (componentString.EndsWith("!"))
        {
            componentString = componentString.Substring(0, componentString.Length - 1);
        }

        string[] components = componentString.Split('!');

        if (components.Length < 3)
        {
            throw new Exception("Missing deck component (Main, Extra, or Side).");
        }

        return new TypedDeck
        {
            main = YdkeParser.Base64ToPasscodes(components[0]),
            extra = YdkeParser.Base64ToPasscodes(components[1]),
            side = YdkeParser.Base64ToPasscodes(components[2])
        };
    }
}