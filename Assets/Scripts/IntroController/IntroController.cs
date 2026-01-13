using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PRD;

public class IntroController : MonoBehaviour
{
    public float timer;
    IntroFunctional introFunctional = new IntroFunctional();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(introFunctional.RunTimeStart("StartScene", timer));
    }

    public void SkipIntro()
    {
        StopAllCoroutines();
        introFunctional.SkipIntro("StartScene");
    }
}
