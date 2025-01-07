using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Tetromino : MonoBehaviour
{
    // 落下に関する時間の変数
    float fall = 0;
    public float fallSpeed = 1; // 落下速度
    private bool hasDropped = false; 
    private bool move = true;

    [SerializeField]
    private InputActionAsset inputActions;

    private void OnEnable()
    {
        
        inputActions.Enable();
    }

    private void OnDisable()
    {
       // inputActions.FindAction("Move").performed -= OnMove;
       // inputActions.FindAction("Rotate").performed -= OnRotate;
       // inputActions.FindAction("Hold").performed -= OnHold;
       //inputActions.Disable();
    }
    private void InputActionDisable() 
    {
         inputActions.FindAction("Move").performed -= OnMove;
         inputActions.FindAction("Rotate").performed -= OnRotate;
         inputActions.FindAction("Hold").performed -= OnHold;
         //inputActions.Disable();
    }

    private void Awake()
    {
        inputActions.FindAction("Move").started += OnMove;
        inputActions.FindAction("Rotate").performed += OnRotate;
        inputActions.FindAction("Hold").performed += OnHold;
    }
    void Update()
    {
        //CheckUserInput();
        CheckFall();
    }

    private void CheckFall()
    {
        if (Time.time - fall >= fallSpeed)
        {
            transform.position += new Vector3(0, -1, 0);

            if (!IsValidGridPos())
            {
                transform.position += new Vector3(0, 1, 0);
                Grid.Instance.UpdateGrid(transform);

                if (Grid.IsGameOver())
                {
                    Debug.Log("Game Over!");
                    Grid.Instance.SetGameOver(true);
                }
                else
                {
                    Grid.Instance.DeleteFullRows();
                    FindObjectOfType<Spawner>().SpawnNext();
                    move = false;
                }

                enabled = false;
            }
            else
            {
                Grid.Instance.UpdateGrid(transform);
            }

            fall = Time.time;
            hasDropped = false;
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if(!move) { return; }
        Vector2 direction = context.ReadValue<Vector2>();

        if (direction.x < 0) // Left
        {
            Move(Vector3.left);
        }
        else if (direction.x > 0) // Right
        {
            Move(Vector3.right);
        }

        else if (direction.y < 0)
        {
            SoftDrop();
        }
        else
        {
            HardDrop();
        }
    }

    private void Move(Vector3 direction)
    {
        transform.position += direction;

        if (!IsValidGridPos())
        {
            transform.position -= direction;
        }
        else
        {
            Grid.Instance.UpdateGrid(transform);
        }
    }

    private void SoftDrop() 
    {
        transform.position += new Vector3(0, -1, 0);

        if (!IsValidGridPos())
        {
            transform.position += new Vector3(0, 1, 0);
            Grid.Instance.UpdateGrid(transform);

            if (Grid.IsGameOver())
            {
                Debug.Log("Game Over!");
                Grid.Instance.SetGameOver(true);
            }
            else
            {
                Grid.Instance.DeleteFullRows();
                FindObjectOfType<Spawner>().SpawnNext();
            }
            move = false;
            enabled = false;
        }
        else
        {
            Grid.Instance.UpdateGrid(transform);
        }

        fall = Time.time;
        hasDropped = false;
    }

    private void HardDrop() 
    {
        if(hasDropped) { return; }
        hasDropped = true;
        while (true)
        {
            transform.position += new Vector3(0, -1, 0);

            // 無効な位置に到達したらループを抜ける
            if (!IsValidGridPos())
            {
                transform.position += new Vector3(0, 1, 0); // 1マス戻す
                break;
            }
        }
        Grid.Instance.UpdateGrid(transform);
        enabled = false;
        move = false;
        if (Grid.IsGameOver())
        {
            Debug.Log("Game Over!");
            Grid.Instance.SetGameOver(true);
        }
        else
        {
            Grid.Instance.DeleteFullRows();
            FindObjectOfType<Spawner>().SpawnNext();
        }
        
        
    }

    private void OnRotate(InputAction.CallbackContext context)
    {
        if(!move) { return; }
        Rotate();
    }

    private void Rotate()
    {
        transform.Rotate(0, 0, -90);

        if (!IsValidGridPos())
        {
            transform.Rotate(0, 0, 90);
        }
        else
        {
            Grid.Instance.UpdateGrid(transform);
        }
    }

    private void OnHold(InputAction.CallbackContext context) 
    {
        
    }

    // ユーザー入力をチェックしてブロックを移動または回転させる
    void CheckUserInput()
    {
        // 左矢印キーが押された場合
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // ブロックを左に移動
            transform.position += new Vector3(-1, 0, 0);
            // 位置が有効かチェック
            if (!IsValidGridPos())
                transform.position += new Vector3(1, 0, 0); // 位置が無効なら元に戻す
            else
                Grid.Instance.UpdateGrid(transform); // 位置が有効ならグリッドを更新
        }
        // 右矢印キーが押された場合
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // ブロックを右に移動
            transform.position += new Vector3(1, 0, 0);
            // 位置が有効かチェック
            if (!IsValidGridPos())
                transform.position += new Vector3(-1, 0, 0); // 位置が無効なら元に戻す
            else
                Grid.Instance.UpdateGrid(transform); // 位置が有効ならグリッドを更新
        }
        // 上矢印キーが押された場合
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // ブロックを回転
            transform.Rotate(0, 0, -90);
            // 位置が有効かチェック
            if (!IsValidGridPos())
                transform.Rotate(0, 0, 90); // 位置が無効なら元に戻す
            else
                Grid.Instance.UpdateGrid(transform); // 位置が有効ならグリッドを更新
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time - fall >= fallSpeed)
        {
            // ブロックを一段下に移動
            transform.position += new Vector3(0, -1, 0);
            // 位置が有効かチェック
            if (!IsValidGridPos())
            {
                // 位置が無効なら元に戻す
                transform.position += new Vector3(0, 1, 0);
                // グリッドを更新
                Grid.Instance.UpdateGrid(transform);
                if (Grid.IsGameOver()) // ゲームオーバー条件のチェック
                {
                    Debug.Log("Game Over!");
                    // ゲームオーバー処理、例えばシーンをリロードするなど
                    Grid.Instance.SetGameOver(true);
                }
                else 
                {
                    // 完全に埋まった行を削除
                    Grid.Instance.DeleteFullRows();
                    // 新しいテトリミノを生成
                    FindObjectOfType<Spawner>().SpawnNext();
                }
                
                enabled = false;
            }
            else
            {
                Grid.Instance.UpdateGrid(transform);
            }
            // 落下時間をリセット
            fall = Time.time;
        }
    }
    // グリッド内での位置が有効かどうかを判定する
    bool IsValidGridPos()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = Grid.Instance.RoundVector2(child.position);

            if (!Grid.Instance.InsideBorder(v))
                return false;

            if (Grid.grid[(int)v.x, (int)v.y] != null &&
                Grid.grid[(int)v.x, (int)v.y].parent != transform)
                return false;
        }
        return true;
    }

    void UpdateGrid()
    {
        for (int y = 0; y < Grid.height; ++y)
            for (int x = 0; x < Grid.width; ++x)
                if (Grid.grid[x, y] != null)
                    if (Grid.grid[x, y].parent == transform)
                        Grid.grid[x, y] = null;

        foreach (Transform child in transform)
        {
            Vector2 v = Grid.Instance.RoundVector2(child.position);
            Grid.grid[(int)v.x, (int)v.y] = child;
        }
    }
}