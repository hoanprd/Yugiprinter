using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InCardController : MonoBehaviour
{
    public void BackToMenu()
    {
        AppManager.Instance.ChangeScene("MenuScene",0f, false, 0f);
    }
}
