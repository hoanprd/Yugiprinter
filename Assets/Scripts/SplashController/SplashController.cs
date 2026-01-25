using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PRD;

public class SplashController : MonoBehaviour
{
    IntroFunctional introFunctional;

    // Start is called before the first frame update
    void Start()
    {
        introFunctional = new IntroFunctional();
        StartCoroutine(introFunctional.RunTimeStart("IntroScene", 5f));
    }
}
