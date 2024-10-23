using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    private void Start()
    {
        int ScreenWidth = Screen.width;
        int ScreenHeight = Screen.height;
        Screen.SetResolution(ScreenWidth,ScreenHeight,false);
    }
    public void OnSceneChange() 
    {
        GameManager.instance.LoadScene("Main");
    }
}
