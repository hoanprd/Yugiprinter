using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AppManager.Instance.PlayBGM(0, true);
    }

    public void StartButtonGo()
    {
        AppManager.Instance.CheckInternet((connected) =>
        {
            if (connected)
            {
                AppManager.Instance.ChangeScene("MenuScene", 0.5f, true, 1f);
            }
        });
    }
}
