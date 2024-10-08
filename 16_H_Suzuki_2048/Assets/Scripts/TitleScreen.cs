using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class TitleScreen : MonoBehaviour,ITitleScreen
{
    [SerializeField]
    private Button _startButton;
    [SerializeField]
    private TextMeshProUGUI _highScoreText;

    private bool initialized;

    void Start()
    {
        Init();
        int screenHeight = Screen.height;
        int screenWidth = (int)(screenHeight*(9f/16f));
        Screen.SetResolution(screenWidth, screenHeight,false);
        SoundManager.Instance.PlayBGM(BGM.Main);
    }

    private void OnDestroy()
    {
        UnInit();
    }

    private void Init() 
    {
        _startButton.onClick.AddListener(StartGame);
        var HighScore = LoadOrCreateBestScore();
        _highScoreText.SetText(HighScore.ToString());
    }

    private void UnInit() 
    {
        _startButton.onClick.RemoveListener(StartGame);    
    }


    public void StartGame() 
    {
        //ÉVÅ[ÉìÇÃëJà⁄
        SceneManager.LoadScene("Main");
    }

    public int LoadOrCreateBestScore() 
    {
        int highScore = 0;
        StringBuilder filePath;
        filePath = new StringBuilder();
        string path = Application.streamingAssetsPath;
        string name = "best_score.txt";
        filePath.Append(path).Append(Path.DirectorySeparatorChar).Append(name);
        if (File.Exists(filePath.ToString()))
        {
            var text = File.ReadAllText(filePath.ToString());
            int.TryParse(text, out highScore );
        }
        else 
        {
            File.Create(filePath.ToString()).Close();
        }

        return highScore;
    }    
}
