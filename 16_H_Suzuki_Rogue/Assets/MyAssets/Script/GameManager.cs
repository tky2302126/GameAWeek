using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Scene = UnityEngine.SceneManagement.Scene;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2.0f;
    public float turnDelay = 0.1f;


    public static GameManager instance;
    public BoardManager _board;
    public int playerFoodPoints = 100;
    [HideInInspector] public bool playersTurn = true;

    public TextMeshProUGUI _levelText;
    public GameObject _levelImage;
    public GameObject _gameOver;
    public int level = 1;

    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doSetup;

    [SerializeField]
    private InputActionAsset _input;

    private Canvas _canvas;

    #region ダンジョンテスト

    //private Button _debugButton;
    //[SerializeField]
    //private int testRow = 20;
    //[SerializeField]
    //private int testCol = 20;
    //[SerializeField]
    //private int testIteration = 4;
    
    #endregion
    private int crrWidth;

    private int crrHeight;

    readonly int firstlevel = 1;
    readonly int firstPlayerFP = 100;

    //シングルトン設計
    private void Awake()
    {
        if(instance == null) 
        {
            instance = this;
        }
        else if(instance != this) 
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();

        crrWidth  = 30;
        crrHeight = 20;
        
    }

    private void Start()
    {
        if(SceneManager.GetActiveScene().name != "Main") 
        {
            _canvas = GetComponentInChildren<Canvas>();
            _canvas.enabled = false;
        }
    }

    #region ダンジョンデバッグ
    //private void Start()
    //{
    //    _debugButton = GameObject.Find("Button(Debug)").GetComponent<Button>();
    //    _debugButton.onClick.AddListener(DebugDungeon);
    //}

    //void DebugDungeon() 
    //{
    //    _board?.DungeonGenerate(testRow, testCol, testIteration);
    //}

    #endregion

    //シーンロード後に１度だけ呼び出す
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallBackInit() 
    {
        //呼ばれてない?
        //SceneManager.sceneLoaded += OnSceneLoaded;

        OnSceneLoaded();
    }

    static private void OnSceneLoaded(Scene scene ,LoadSceneMode mode) 
    {
        if(scene.name == "Main") 
        {
            instance.InitGame();
        }
    }
    
    static private void OnSceneLoaded() 
    {
        if (SceneManager.GetActiveScene().name == "Main")
        {
            
            instance.InitGame();
        }
    }

    public void InitGame() 
    {
        doSetup = true;

        _canvas.enabled = true;

        SoundManager.Instance.PlayBGM(BGM.Main);

        var text = "Day" + level;
        _levelImage.SetActive(true);

        _levelText.SetText(text);

        StartCoroutine(HideImage(levelStartDelay));

        enemies.Clear();

        _board.SetUpScene(crrWidth, crrHeight, level);
    }

    public IEnumerator HideImage(float duration)
    {
        yield return new WaitForSeconds(duration);
        _levelImage.SetActive(false);

        doSetup = false;

    }
    // プレイヤーの入力を判定してから敵の移動を行う
    
    public void Update()
    {
        if (doSetup || playersTurn || enemiesMoving) { return; }

        StartCoroutine(MoveEnemies());
    }

    public IEnumerator myUpdate() 
    {
        if(doSetup) { yield break; }
        playersTurn = true;

        enemiesMoving = true;

        // プレイヤーの移動処理

        // エネミーの移動処理
        // !エネミーの実装をしていないためコメントアウト中
        //StartCoroutine(MoveEnemies());

        //updateの終わり
        playersTurn = false;
        enemiesMoving= false;
    }



    public void AddEnemyToList(Enemy _enemy) 
    {
        enemies.Add( _enemy );
    }

    public void GameOver() 
    {
        var text = "After" + level + " days, you starved.";

        _levelText.SetText(text);

        _levelImage.SetActive(true);

        enabled = false;

        // 字幕のフェードイン
        _gameOver.transform.DOLocalMove(new Vector3(0, -180, 0), 2.0f)
            .OnComplete(MenuOptionEnabled);
    }

    void MenuOptionEnabled()
    {
        _input.FindAction("Retry").performed += RetryGame;
        _input.FindAction("EndGame").performed += EndGame;
    }

    private void EndGame(InputAction.CallbackContext context)
    {
# if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void RetryGame(InputAction.CallbackContext context)
    {
        level = firstlevel;
        playerFoodPoints = firstPlayerFP;
        ReloadScene();
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;

        yield return new WaitForSeconds(turnDelay);

        if(enemies.Count == 0) 
        {
            yield return new WaitForSeconds(turnDelay);
        }

        foreach (Enemy _enemy in enemies) 
        {
            _enemy.MoveEnemy();

            yield return new WaitForSeconds(_enemy.moveTime);
        }

        playersTurn = true;

        enemiesMoving =false;
    }

    public Vector2Int GetMapSize() 
    {
        Vector2Int MapSize = new Vector2Int(crrWidth,crrHeight);
        return MapSize;
    }

    public void LoadScene(string sceneName) 
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public void ReloadScene()
    {
        var sceneName = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // ロードが完了するまで待機
        while(!asyncLoad.isDone) 
        {
            yield return null;
        }

        OnSceneLoaded();
    }
}
