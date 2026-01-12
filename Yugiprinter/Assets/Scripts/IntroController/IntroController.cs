using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitEndIntro());
    }

    public void SkipIntro()
    {
        SceneManager.LoadScene("StartScene");
    }

    IEnumerator WaitEndIntro()
    {
        yield return new WaitForSeconds(90f);
        SceneManager.LoadScene("StartScene");
    }
}
