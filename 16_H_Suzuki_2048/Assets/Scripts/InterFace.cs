using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterFace : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public interface IScreen 
{
    void LoadScene();

}

public interface IMainScreen 
{
    //プロパティ
    int[,] Board {  get;}

    int Score { get;}

    //メソッド
    void InitGame(int[,] board,int score);

    void GoToResult();

    IEnumerator CountDown(int second);
}

public interface ITitleScreen
{
    void StartGame();
    int LoadOrCreateBestScore();
}

public interface IResultScreen 
{
    void BackToTitle();

    void SaveBestScore(int score);
}
