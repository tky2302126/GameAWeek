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
        SaveBestScore(SessionData.instance.score);
        _scoreText.SetText(SessionData.instance.score.ToString());
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
    public void SaveBestScore(int score) 
    {
        var highScore =0;

        StringBuilder filePath;
        filePath = new StringBuilder();
        string path = Application.persistentDataPath;
        string name = "best_score.txt";
        filePath.Append(filePath.ToString()).Append("/").Append(name.ToString());
        if (File.Exists(filePath.ToString()))
        {
            var text = File.ReadAllText(filePath.ToString());
           highScore = int.Parse(text);

            
        }
        else
        {
            File.Create(filePath.ToString()).Close();
        }

        if (highScore < score)
        {
            File.WriteAllText(filePath.ToString(), score.ToString());
        }
    }
}
