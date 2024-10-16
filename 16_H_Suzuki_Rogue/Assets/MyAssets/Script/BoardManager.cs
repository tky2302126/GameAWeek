using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            var randomIndex = Random.Range(0,emptyPositions.Count);

            var randomPosition = emptyPositions[randomIndex];

            emptyPositions.RemoveAt(randomIndex);

            return randomPosition;
        }

        void LayoutObjectAtRandom(GameObject tile,int minimun ,int maxmum) 
        {
            int objectCount = Random.Range (minimun,maxmum);

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

    public void DungeonGenerate(int rowSize,int colSize) 
    {
        int[,] grid = new int[rowSize,colSize];
        InitGrid(ref grid);



    }

    private void InitGrid(ref int[,] grid) 
    {
        
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

            if(newRow>=0 && newCol >=0 &&  newRow < grid.GetLength(0) && newCol < grid.GetLength(1)) 
            {
                
            }
        }

        return result;
    }
}

