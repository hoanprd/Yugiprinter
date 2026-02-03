using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InCardController : MonoBehaviour
{
    void Start()
    {
        for (int i = 0; i < AppManager.Instance.bgmAudioSource.Length; i++)
        {
            AppManager.Instance.PlayBGM(i, false);
        }

        AppManager.Instance.PlayBGM(2, true);
    }

    public void BackToMenu()
    {
        AppManager.Instance.ChangeScene("MenuScene", 1f, true, 1f);
    }
}