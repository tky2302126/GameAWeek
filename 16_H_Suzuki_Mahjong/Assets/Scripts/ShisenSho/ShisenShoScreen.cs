using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShisenShoScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayBGM(BGM.ShisenSho);    
    }

    private void OnDestroy()
    {
        SoundManager.Instance.StopBGM();
    }

    public void OnBackToTitle() 
    {
        SceneManager.LoadScene("Title");
    }


}
