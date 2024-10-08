using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class ResultScreen : MonoBehaviour,IResultScreen
{
    [SerializeField]
    private Button _titleButton;

    [SerializeField]
    private TextMeshProUGUI _scoreText;

    [SerializeField]
    private TextMeshProUGUI _highScoreText;

    [SerializeField]
    private SessionData _sessionData;

    [SerializeField]
    GameObject UpdateScore;

    private bool ScoreUpdated = false;

    private int highScore;


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void OnDestroy() 
    {
        UnInit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Init() 
    {
        _titleButton.onClick.AddListener(BackToTitle);
        SaveBestScore(_sessionData.score);
        _scoreText.SetText(_sessionData.score.ToString());
        _highScoreText.SetText(highScore.ToString());
        if (ScoreUpdated) 
        {
            UpdateScore.SetActive(true);
        }
        SoundManager.Instance.StopBGM();
    }

    void UnInit() 
    {
        _titleButton.onClick.RemoveListener(BackToTitle);
    }

    public void BackToTitle() 
    {
        SceneManager.LoadScene("Title");
    }

    //ÉXÉRÉAÇÃìoò^
    public void SaveBestScore(int crrScore) 
    {
        highScore =0;

        StringBuilder filePath;
        filePath = new StringBuilder();
        string path = Application.streamingAssetsPath;
        string name = "best_score.txt";
        //  äKëwÇãÊêÿÇÈï∂éöÇÕOSàÀë∂Ç»ÇÃÇ≈îƒópìIÇ»Ç‡ÇÃÇégÇ§
        filePath.Append(path).Append(Path.DirectorySeparatorChar).Append(name);
        if (File.Exists(filePath.ToString()))
        {
            var text = File.ReadAllText(filePath.ToString());
            int.TryParse(text, out highScore);
            
        }
        else
        {
            File.Create(filePath.ToString()).Close();
        }

        if (highScore < crrScore)
        {
            highScore = crrScore;
            File.WriteAllText(filePath.ToString(), crrScore.ToString());
            ScoreUpdated = true;
        }
    }
}
