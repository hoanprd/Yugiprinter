using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InCardController : MonoBehaviour
{
    void Start()
    {
        AppManager.Instance.PlayBGM(1, false);
        AppManager.Instance.PlayBGM(2, true);
    }

    public void BackToMenu()
    {
        AppManager.Instance.ChangeScene("MenuScene", 1f, true, 1f);
    }
}