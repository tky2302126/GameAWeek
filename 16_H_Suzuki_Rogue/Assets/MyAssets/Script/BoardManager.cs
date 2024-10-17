using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CELL
{
    WALL,
    ROUTE,
    FLOOR,
    ROOM
}
public class BoardManager : MonoBehaviour
{
    public class Count
    {
        public int minmum;
        public int maximum;

        public Count(int min, int max)
        {
            this.minmum = min;
            this.maximum = max;
        }
    }

    public int columns = 8;
    public int rows = 8;

    public Count wallCount = new Count(3, 9);
    public Count foodCount = new Count(1, 5);


    public GameObject _exit;
    public GameObject _floor;
    public GameObject _wall;
    public GameObject _outerWall;
    public GameObject _enemy1;
    public GameObject _food;

    private Transform boardHolder;

    private List<Vector3> emptyPositions = new List<Vector3>();

    private CELL[,] map;

        void Start() 
        {
            
        //! 大きく変える予定
            //BoardSetUp();

            InitList();

            LayoutObjectAtRandom(_wall,wallCount.minmum,wallCount.maximum);

            LayoutObjectAtRandom(_food,foodCount.minmum,foodCount.maximum);

            Instantiate(_exit, new Vector3(columns - 1, rows - 1, 0), Quaternion.identity);
        }

        //1部屋だけのマップを作るスクリプト
        void RoomSetUp() 
        {
            boardHolder = new GameObject("Room").transform;

            for(int x = -1;x<columns+1; x++) 
            {
                for(int y = -1;y<rows+1; y++) 
                {
                    GameObject toInstantuate = _floor;

                    if(x == -1|| y == -1 || x== columns || y == rows) 
                    {
                        toInstantuate = _outerWall;
                    }

                    var instance = 
                        Instantiate(toInstantuate, new Vector3(x, y, 0), Quaternion.identity) 
                        as GameObject;
                    
                    instance.transform.SetParent(boardHolder);
                }
            }
        }

        void InitList() 
        {
            emptyPositions.Clear();

            for(int x = 1; x < columns - 1; x++) 
            {
                for(int y = 1; y < rows - 1; y++) 
                {
                    emptyPositions.Add(new Vector3 (x, y, 0));
                }
            }
        }

        Vector3 GetRandomPosition() 
        {
            var randomIndex = UnityEngine.Random.Range(0,emptyPositions.Count);

            var randomPosition = emptyPositions[randomIndex];

            emptyPositions.RemoveAt(randomIndex);

            return randomPosition;
        }

        void LayoutObjectAtRandom(GameObject tile,int minimun ,int maxmum) 
        {
            int objectCount = UnityEngine.Random.Range (minimun,maxmum);

            for(int i = 0; i < objectCount; i++) 
            {
                var randomPosition = GetRandomPosition();

                Instantiate(tile, randomPosition, Quaternion.identity);
            }
        }

    public void SetupScene(int level) 
    {
        RoomSetUp();

        InitList();

        LayoutObjectAtRandom(_wall, wallCount.minmum, wallCount.maximum);

        LayoutObjectAtRandom(_food,foodCount.minmum, foodCount.maximum);

        int enemyCount = (int)Mathf.Log(level, 2f);

        LayoutObjectAtRandom(_enemy1, enemyCount, enemyCount);

        // !出口はemptypositionのどこかに置く
        Instantiate(_exit,new Vector3(columns-1,rows-1), Quaternion.identity);
    }

    // セルオートマトンを使ったダンジョン生成
    // 部屋のセルをランダムにまく

    public void DungeonGenerate(int rowSize,int colSize,int iteration) 
    {
        //! 二次元配列の順番が[x,y]
        map = new CELL[colSize,rowSize];
        
        InitGrid();

        RunCellAutomaton(iteration);

        //いらなそう
        var roomNum = CountRooms();

        List<Vector2Int> roomCenters = GetRoomCenters();

        ConnectRooms(roomCenters);


    }

    // 島同士をRouteでつなぐ(最短距離のもの同士)
    private void ConnectRooms(List<Vector2Int> roomCenters)
    {
        throw new System.NotImplementedException();
    }
    
    private List<Vector2Int> GetRoomCenters()
    {
        List<Vector2Int> RoomCenters = new List<Vector2Int>();
        //探索済みか保存するコンテナ
        bool[,] visited = new bool[map.GetLength(0), map.GetLength(1)];

        for(int x=0; x < map.GetLength(0); x++) 
        {
            for(int y=0; y < map.GetLength(1); y++) 
            {
                if (map[x,y] == CELL.ROOM && !visited[x, y]) 
                {
                    Vector2Int center = FindRoomCenter(x, y, ref visited);
                    RoomCenters.Add(center);
                }
            }
        }

        return RoomCenters;
    }
    // BFSを使った探索
    private Vector2Int FindRoomCenter(int x, int y, ref bool[,] visited)
    {
        List<Vector2Int> RoomCells = new List<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(x, y));

        //ValueTuple std::pairのように使えるやつ
        (int, int)[] direction = { (-1, 0), (1, 0),(0, -1),(0, 1) };
        //Tuple<int, int>[] Tdirection = { new Tuple<int, int>(-1, 0), new Tuple<int, int>(1, 0), new Tuple<int, int>(0, -1), new Tuple<int, int>(0, 1) };

        while (queue.Count > 0) 
        {
            //dequeue で最初の要素を取り出す
            Vector2 current = queue.Dequeue();
            var crrX = current.x;
            var crrY = current.y;

        }

        return new Vector2Int();
    }

    private void RunCellAutomaton(int iteration)
    {
        for(int i = 0; i < iteration; i++) 
        {
            UpdateGrid();   
        }
        // 最後に四方がwallのfloor,RoomはWallにする
        FinalaizeMap();
    }

    // Roomを数える(島を数えるアルゴリズム)
    private int CountRooms()
    {
        throw new System.NotImplementedException();
    }

    // マップにランダムにセルをまく
    private void InitGrid() 
    {
        var height = map.GetLength(0);
        var width  = map.GetLength(1);
        for(int y=0; y < height; y++) 
        {
            for(int x=0; x < width; x++) 
            {
                //適当に種をまく
                map[x,y] = UnityEngine.Random.value <= 0.05f ? CELL.ROOM : CELL.WALL;
            }
        }
    }

    int CountWallNeighbors(ref int[,] grid,int row,int col) 
    {
        int result = 0;

        int[] rowAxis = { -1, -1, -1, 0, 0, 1, 1, 1 };

        int[] colAxis = { -1, 0, 1, -1, 1, -1, 0, 1 };

        for(int i = 0; i < 8; i++) 
        {
            int newRow = row + rowAxis[i];
            int newCol = col + colAxis[i];

            if(CheckInBounds(newCol,newRow)) 
            {
                
            }
        }

        return result;
    }

    // 撒かれたセルがルールに基づいて
    // マップを変える
    // 一定回数繰り返す
    void UpdateGrid() 
    {
        CELL[,] newMap = new CELL[map.GetLength(0),map.GetLength(1)];

        for(int x = 0; x < map.GetLength(0); x++) 
        {
            for(int y = 0; y < map.GetLength(1); y++) 
            {
                newMap[x, y] = ApplyRule(x, y);
            }
        }

        map = newMap;
    }

    bool CheckInBounds(int x,int y) 
    {
        bool result = false;
        if(x >= 0 && y >= 0 && x<map.GetLength(0) && y < map.GetLength(1)) 
        {
            result = true;
        } 
        return result;
    }
        
    private CELL ApplyRule(int x, int y)
    {
        CELL crrCell = map[x, y];
        
        switch(crrCell) 
        {
            // Room ->周囲のマスをRoomかFloorに変える
            // 変えた先のマスがすでにFloorだった場合、Roomにしない
            case CELL.ROOM:

                ChangeSurrondCells(x, y);
                break;

            // Floor->ときどきWallになる
            case CELL.FLOOR:
                if(UnityEngine.Random.value <= 0.1f) 
                {
                    return CELL.WALL;
                }
                break;

            // Wall->ときどきFloorになる
            case CELL.WALL:
                if(UnityEngine.Random.value <= 0.05f) 
                {
                    return CELL.FLOOR;
                }
                break;
        }

        return crrCell;
    }

    private void ChangeSurrondCells(int x, int y)   
    {
        int[] rowAxis = { -1, -1, -1, 0, 0, 1, 1, 1 };

        int[] colAxis = { -1, 0, 1, -1, 1, -1, 0, 1 };

        for(int i = 0;i<8;i++) 
        {
            var newRow = y + rowAxis[i];
            var newCol = x + colAxis[i];
            if(!CheckInBounds(newCol, newRow)) { continue; }
            // Floor,Roomのセルは更新しない
            if (map[newCol, newRow] == CELL.FLOOR|| map[newCol,newRow] == CELL.ROOM) continue;
            // 25%でRoomにする
            map[newCol,newRow] = UnityEngine.Random.value <= 0.25 ? CELL.ROOM : CELL.FLOOR;
            
        }
    }

    private void FinalaizeMap()
    {
        for(int x = 0; x < map.GetLength(0); x++) 
        {
            for(int y=0; y<map.GetLength(1); y++) 
            {
                if (map[x,y]==CELL.FLOOR || map[x, y] == CELL.ROOM) 
                {
                    if (IsSurroundedByWalls(x, y)) 
                    {
                        map[x, y] = CELL.WALL;
                    }
                }
            }
        }
    }

    private bool IsSurroundedByWalls(int x, int y)
    {
        bool result = false;

        int[] rowAxis = {-1,1,0,0 };
        int[] colAxis = {0,0,-1,1 };

        int wallCount = 0;

        for(int i = 0; i < 4; i++) 
        {
            int newX = x + colAxis[i];
            int newY = y + rowAxis[i];

            if(CheckInBounds(newX, newY)) 
            {
                if (map[newX,newY] == CELL.WALL) { wallCount++; }
            }
            else 
            {
                wallCount++;
            }
        }

        if(wallCount == 4) { result = true; }

        return result;
    }

    //生成されたマップをコンソールログに表示する
    public void DumpDungeon() 
    {
        
    }
}

