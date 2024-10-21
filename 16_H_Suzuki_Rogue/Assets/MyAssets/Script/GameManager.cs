using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public int level = 1;

    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doSetup;

    private Button _debugButton;
    [SerializeField]
    private int testRow = 20;
    [SerializeField]
    private int testCol = 20;
    [SerializeField]
    private int testIteration = 4;

    //�V���O���g���݌v
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
        //�C�j�V�����C�Y��1�x�ł����̂ň�U�R�����g�A�E�g
        //InitGame();
    }

    #region �_���W�����f�o�b�O
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

    //�V�[�����[�h��ɂP�x�����Ăяo��
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallBackInit() 
    {
        SceneManager.sceneLoaded += OnSceneLoaded;    
    }

    static private void OnSceneLoaded(Scene arg0 ,LoadSceneMode arg1) 
    {
        instance.InitGame();
    }

    public void InitGame() 
    {
        doSetup = true;
        _levelImage = GameObject.Find("LevelImage");

        _levelText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();

        var text = "Day" + level;

        _levelText.SetText(text);

        _levelImage.SetActive(true);

        Invoke("HideImage", levelStartDelay);

        enemies.Clear();

        _board.SetupScene(level);
    }

    public void HideImage()
    {
        _levelImage?.SetActive(false);

        doSetup = false;

    }
    // �v���C���[�̓��͂𔻒肵�Ă���G�̈ړ����s��
    
       public void Update()
    {
        if(doSetup|| playersTurn || enemiesMoving) { return; }

        StartCoroutine(MoveEnemies());
    }

    public IEnumerator myUpdate() 
    {
        if(doSetup) { yield break; }
        playersTurn = true;

        enemiesMoving = true;

        // �v���C���[�̈ړ�����

        // �G�l�~�[�̈ړ�����
        StartCoroutine(MoveEnemies());

        //update�̏I���
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
}
