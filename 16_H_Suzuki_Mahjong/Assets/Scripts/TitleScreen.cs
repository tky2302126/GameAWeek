using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayBGM(BGM.Title);    
    }

    private void OnDestroy()
    {
        SoundManager.Instance.StopBGM();
    }

    public void OnClickShisenSho() 
    {
        SceneManager.LoadScene("Shisen-Sho");
    }

    public void OnClickCheckAgari() 
    {
        SceneManager.LoadScene("CheckAgari");
    }

    public void OnClickQuit() 
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }


}
