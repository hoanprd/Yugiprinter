using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PRD;

public class SplashController : MonoBehaviour
{
    public float splashTime;

    // Start is called before the first frame update
    void Start()
    {
        if (AppManager.Instance.skipIntro == false)
            AppManager.Instance.ChangeScene("IntroScene", splashTime, false, 0f);
        else
            AppManager.Instance.ChangeScene("StartScene", splashTime, false, 0f);
    }
}
