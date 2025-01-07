using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    private int score = 0;
    public TextMeshPro scoreText;  // UnityのInspectorから設定

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void AddScore(int linesCleared)
    {
        int scoreToAdd = 0;
        switch (linesCleared)
        {
            case 1:
                scoreToAdd = 100;
                break;
            case 2:
                scoreToAdd = 300;
                break;
            case 3:
                scoreToAdd = 500;
                break;
            case 4:
                scoreToAdd = 800;
                break;
            default:
                break;
        }

        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }

    public void ResetScore() 
    {
        score = 0;
        scoreText.text = "Score: 0000"; 
    }
}