using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    private void Start()
    {
        int ScreenHeight = Screen.height;
        int ScreenWidth = (int)(ScreenHeight *(16/9));
        Screen.SetResolution(ScreenWidth, ScreenHeight,false);
    }
    public void OnSceneChange() 
    {
        GameManager.instance.LoadScene("Main");
    }
}
