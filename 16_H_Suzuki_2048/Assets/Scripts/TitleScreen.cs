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
    }

    private void OnDestroy()
    {
        UnInit();
    }

    private void Init() 
    {
        _startButton.onClick.AddListener(StartGame);
    }

    private void UnInit() 
    {
        _startButton.onClick.RemoveListener(StartGame);    
    }


    public void StartGame() 
    {
        //�V�[���̑J��
        SceneManager.LoadScene("Main");
    }

    public int LoadOrCreateBestScore() 
    {
        int highScore = 0;
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

        return highScore;
    }    
}
