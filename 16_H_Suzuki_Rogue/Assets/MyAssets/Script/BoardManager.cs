using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;


public enum CELL
{
    WALL,
    FLOOR,
    CHILD,
    ROOM,
    OUTERWALL
}
public class BoardManager : MonoBehaviour
{
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            this.minimum = min;
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

    private GameObject boardHolder;
    private GameObject itemHolder;

    private List<Vector3> emptyPositions = new List<Vector3>();
    private List<Vector2Int> _emptyPositions =  new List<Vector2Int>();

    private CELL[,] map;
    private GameObject _player;

        void Start() 
        {
            
        }

    //1部屋だけのマップを作るスクリプト
    //void RoomSetUp() 
    //    {
    //        boardHolder = new GameObject("Room").transform;

    //        for(int x = -1;x<columns+1; x++) 
    //        {
    //            for(int y = -1;y<rows+1; y++) 
    //            {
    //                GameObject toInstantuate = _floor;

    //                if(x == -1|| y == -1 || x== columns || y == rows) 
    //                {
    //                    toInstantuate = _outerWall;
    //                }

    //                var instance = 
    //                    Instantiate(toInstantuate, new Vector3(x, y, 0), Quaternion.identity) 
    //                    as GameObject;

    //                instance.transform.SetParent(boardHolder);
    //            }
    //        }
    //    }

    //void GetEmptyPositions() 
    //{
    //    emptyPositions.Clear();

    //    for(int x = 1; x < columns - 1; x++) 
    //    {
    //        for(int y = 1; y < rows - 1; y++) 
    //        {
    //            emptyPositions.Add(new Vector3 (x, y, 0));
    //        }
    //    }
    //}

    public void ResetMapData()
    {
        if(itemHolder != null) { Destroy(itemHolder.gameObject); }
        if(boardHolder != null) { Destroy(boardHolder.gameObject); }
    }

    void GetEmptyPositions(ref CELL[,] map) 
    {
        _emptyPositions.Clear();

        for(int x= 0; x< map.GetLength(0); x++) 
        {
            for(int y=0;y< map.GetLength(1); y++) 
            {
                if (map[x,y] != CELL.WALL) { _emptyPositions.Add(new Vector2Int(x,y)); }
            }        
        }
    }

    Vector3 GetRandomPosition() 
    {
        var randomIndex = UnityEngine.Random.Range(0,_emptyPositions.Count-1);

        Vector3 randomPosition = new Vector3(_emptyPositions[randomIndex].x, _emptyPositions[randomIndex].y, 0);

        _emptyPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject tile,int minimun ,int maxmum) 
    {
        if(itemHolder == null) 
        {
            itemHolder = new GameObject("ITEM");
        }

        int objectCount = UnityEngine.Random.Range (minimun,maxmum);

        for(int i = 0; i < objectCount; i++) 
        {
            var randomPosition = GetRandomPosition();

            var instance =
            Instantiate(tile, randomPosition, Quaternion.identity);
            instance.transform.SetParent(itemHolder.transform);
        }
    }

    void LayoutObjectAtRandom(GameObject tile) 
    {
        if (itemHolder == null)
        {
            itemHolder = new GameObject("ITEM");
        }

        var randomPosition = GetRandomPosition();
        var instance =
        Instantiate(tile, randomPosition, Quaternion.identity);
        instance.transform.SetParent(itemHolder.transform);

    }
    #region アセットのスクリプト  
    //public void SetupScene(int level) 
    //{
    //    RoomSetUp();

    //    GetEmptyPositions();

    //    LayoutObjectAtRandom(_wall, wallCount.minimum, wallCount.maximum);

    //    LayoutObjectAtRandom(_food,foodCount.minimum, foodCount.maximum);

    //    int enemyCount = (int)Mathf.Log(level, 2f);

    //    LayoutObjectAtRandom(_enemy1, enemyCount, enemyCount);

    //    // !出口はemptypositionのどこかに置く
    //    Instantiate(_exit,new Vector3(columns-1,rows-1), Quaternion.identity);
    //}
    #endregion

    public void SetUpScene(int width ,int Height,int level) 
    {
        //! マップの大きさによってセルオートマトンのループ数を調整する
        // int iteration;

        DungeonGenerate(Height,width,4);

        GetEmptyPositions(ref map);

        SetMapTip(ref map);

        //! 壁や食料もマップの大きさによって数値を調整

        LayoutObjectAtRandom(_wall, wallCount.minimum, wallCount.maximum);

        LayoutObjectAtRandom(_food, foodCount.minimum, foodCount.maximum);

        // ! 敵の実装(優先度:低)
        //int enemyCount = (int)Mathf.Log(level, 2f);

        //LayoutObjectAtRandom(_enemy1, enemyCount, enemyCount);

        LayoutObjectAtRandom(_exit);

        _player = GameObject.Find("Player");

        LayoutPlayerAtRandom(_player);


    }

    private void LayoutPlayerAtRandom(GameObject player)
    {
        var randomIndex = UnityEngine.Random.Range(0, _emptyPositions.Count - 1);

        var randomPosition = _emptyPositions[randomIndex];

        _emptyPositions.RemoveAt(randomIndex);

        _player.transform.position = new Vector3(randomPosition.x, randomPosition.y, 0);
    }

    private void SetMapTip(ref CELL[,] map)
    {
        boardHolder = new GameObject("MAP");
        for(int x = 0; x < map.GetLength(0); x++) 
        {
            for(int y = 0; y < map.GetLength(1); y++) 
            {
                GameObject toInstantiate = _floor;
                if (map[x,y] == CELL.WALL) 
                {
                    toInstantiate = _outerWall;
                }

                var instance =
                    Instantiate(toInstantiate,new Vector3(x,y,0),Quaternion.identity);

                instance.transform.SetParent(boardHolder.transform);
            }
        }
    }

    // セルオートマトンを使ったダンジョン生成
    // 部屋のセルをランダムにまく
    public void DungeonGenerate(int rowSize,int colSize,int iteration) 
    {
        //! 二次元配列の順番が[x,y]
        map = new CELL[colSize+2,rowSize+2];
        
        InitGrid();

        RunCellAutomaton(iteration);

        List<Vector2Int> rooms = GetRooms();

        ConnectRooms(rooms);
#if UNITY_EDITOR
        DumpDungeon(map);
#endif
    }  

    // 島同士をRouteでつなぐ(最短距離のもの同士)
    private void ConnectRooms(List<Vector2Int> roomCenters)
    {
        for(int i=0; i<roomCenters.Count -1 ; i++) 
        {
            Vector2Int start = roomCenters[i];
            Vector2Int end = roomCenters[i+1];

            CreateCorridor(start, end);    
        }
    }
    
    private void CreateCorridor(Vector2Int start, Vector2Int end) 
    {
        for (int x = Mathf.Min(start.x, end.x); x <= Mathf.Max(start.x, end.x); x++) 
        {
            if (map[x,start.y] != CELL.ROOM) 
            {
                map[x,start.y] = CELL.FLOOR;
            }
        }

        for (int y = Mathf.Min(start.y,end.y);y <= Mathf.Max(start.y,end.y);y++) 
        {
            if (map[end.x,y] != CELL.ROOM) 
            {
                map[end.x,y] = CELL.FLOOR;
            }
        }
    }

    private List<Vector2Int> GetRooms()
    {
        List<Vector2Int> Rooms = new List<Vector2Int>();

        for(int x=0; x < map.GetLength(0); x++) 
        {
            for(int y=0; y < map.GetLength(1); y++) 
            {
                if (map[x,y] == CELL.ROOM ) 
                {
                    Vector2Int room = new Vector2Int(x,y);
                    Rooms.Add(room);
                }
            }
        }
        return Rooms;
    }

    private void RunCellAutomaton(int iteration)
    {
        for(int i = 0; i < iteration; i++) 
        {
            UpdateGrid();   
        }
        //Debug.Log("before Finalize");
        //DumpDungeon(map);

        // 最後に四方がwallのfloor,RoomはWallにする
        FinalaizeMap();
    }

    // マップにランダムにセルをまく
    private void InitGrid() 
    {
        var width  = map.GetLength(0);
        var height = map.GetLength(1);

        for(int y=0; y < height; y++) 
        {
            for(int x=0; x < width; x++) 
            {
                if(x>= 1 && y>= 1 && x<width-1 && y< height - 1) 
                {
                    //適当に種をまく
                    map[x, y] = UnityEngine.Random.value <= 0.03f ? CELL.ROOM : CELL.WALL;
                }
                else { map[x, y] = CELL.OUTERWALL; }
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

            case CELL.CHILD:
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
            // OUTERWALL
            if (map[newCol,newRow] == CELL.OUTERWALL) continue;
            // 25%でChildにする
            // Roomにすると部屋を無駄に数える
            map[newCol,newRow] = UnityEngine.Random.value <= 0.25 ? CELL.CHILD : CELL.FLOOR;
            
        }
    }

    private void FinalaizeMap()
    {
        bool[,] visited = new bool[map.GetLength(0),map.GetLength(1)];

        for(int x = 0; x < map.GetLength(0); x++) 
        {
            for(int y=0; y<map.GetLength(1); y++) 
            {
                if (visited[x, y] == true) continue;

                // childはfloorにする
                if (map[x, y] == CELL.CHILD) { map[x, y] = CELL.FLOOR; }
                // OuterWallはWallに
                if (map[x,y] == CELL.OUTERWALL) { map[x,y] = CELL.WALL; }

                if (map[x,y]==CELL.FLOOR || map[x, y] == CELL.ROOM) 
                {
                    
                    // 1マスの場合はこれで閉じるが、
                    // ふさがった通路マスは閉じない
                    if (IsSurroundedByWalls(x, y)) 
                    {
                        map[x, y] = CELL.WALL;
                    }                    
                }

                visited[x, y] = true;
            }
        }

        CheckRoom(ref map);
    }

    // BFSでRoomを探索し、
    // roomがなければ埋める
    private void CheckRoom(ref CELL[,] map) 
    {
        var width = map.GetLength(0);
        var height = map.GetLength(1);

        (int, int)[] directions = { (-1, 0), (1, 0), (0, -1), (0, 1) };

        bool[,] visited = new bool[width, height];

        for(int x = 0; x < width; x++) 
        {
            for(int y = 0; y < height; y++) 
            {
                if (!visited[x,y] && map[x,y] != CELL.WALL) 
                {
                    List<Vector2Int> regionCells = new List<Vector2Int>();
                    Queue<Vector2Int> queue = new Queue<Vector2Int>();
                    bool containsRoom = false;

                    queue.Enqueue(new Vector2Int(x, y));
                    visited[x,y] = true;

                    while(queue.Count > 0) 
                    {
                        Vector2Int crrPos = queue.Dequeue();
                        regionCells.Add(crrPos);

                        if (map[crrPos.x,crrPos.y] == CELL.ROOM) 
                        {
                            containsRoom = true;
                        }

                        foreach (var dir in directions)
                        {
                            int nextX = crrPos.x+dir.Item1;
                            int nextY = crrPos.y+dir.Item2;

                            if(!CheckInBounds(nextX, nextY)) { continue; }
                            if(visited[nextX,nextY]) { continue; }
                            if(map[nextX,nextY] == CELL.WALL || map[nextX,nextY] == CELL.OUTERWALL) {  continue; }
                            queue.Enqueue(new Vector2Int(nextX, nextY));
                            visited[nextX,nextY] = true;
                        }
                    }

                    if(!containsRoom) { FillRoom(regionCells); }

                }
            }
        }

    }

    private void FillRoom(List<Vector2Int> cells)
    {
        foreach (var Vector2 in cells)
        {
            map[Vector2.x, Vector2.y] = CELL.WALL;
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
                if (map[newX,newY] == CELL.WALL || map[newX, newY] == CELL.OUTERWALL) { wallCount++; }
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
    public void DumpDungeon(CELL[,] map) 
    {
        StringBuilder sb = new StringBuilder();

        for(int x = 0; x < map.GetLength(0); x++) 
        {
            for(int y = 0; y < map.GetLength(1); y++) 
            {
                if (map[x,y] == CELL.WALL) { sb.Append("#"); }
                if (map[x,y] == CELL.ROOM) { sb.Append("R"); }
                if (map[x,y] == CELL.FLOOR) { sb.Append(" "); }
            }

            sb.AppendLine();
        }

        Debug.Log(sb.ToString());
    }
}

