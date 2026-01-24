using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void InCardOpen()
    {
        Debug.Log("Chuyển sang InCardScene");
        SceneManager.LoadScene("InCardScene");
    }
}
