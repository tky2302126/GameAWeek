using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShisenSho
{
    public class Board : MonoBehaviour
    {
        public GameObject tilePrefab;
        public Transform board;
        public RectTransform TileHolder;

        private List<ShisenSho.Tile> tiles = new List<Tile>();
        private List<GameObject> tileObjList = new List<GameObject>();

        private Tile[,] tileArray;
        public  Tile[,] TileArray => tileArray;

        readonly int Rows = 10;
        readonly int Cols = 19;


        private void Start()
        {
            tileArray = new Tile[Cols, Rows];
            GenerateTiles();
            ShuffleTile();
            SetBoard();
        }

        private void SetBoard()
        {
            for(int i = 0; i < tileObjList.Count; i++) 
            {
                tileObjList[i].transform.SetParent(TileHolder,false);
                int x = i % 17+1;
                int y = i / 17+1;
                var tile = tileObjList[i].GetComponent<ShisenSho.Tile>();
                tile.SetState(State.IsAlive);
                tileArray[x, y] = tile;
            }

            for (int i = 0; i < Cols - 1; i++)
            {
                var tile = new Tile(Suit.Manzu, Rank.One);
                tileArray[i, 0] = tile;
                tileArray[i, Rows - 1] = tile;
            }

            for (int i = 1; i < Rows - 1; i++)
            {
                var tile = new Tile(Suit.Manzu, Rank.One);
                tileArray[0, i] = tile;
                tileArray[Cols - 1, i] = tile;
            }

        }

        private void ShuffleTile()
        {
            for(int i = 0; i < tileObjList.Count; i++) 
            {
                int randomIndex = UnityEngine.Random.Range(i, tileObjList.Count);
                var temp = tileObjList[i];
                tileObjList[i] = tileObjList[randomIndex];
                tileObjList[randomIndex] = temp;
            }
        }

        void RegistTile(GameObject _tileObj) 
        {
            tileObjList.Add(_tileObj);
        }

        void GenerateTiles() 
        {
            // ���v
            for(Suit suit = (int)Suit.Manzu;suit <= Suit.Souzu; suit++) 
            {
                for (int i = 0; i < 4; i++)
                {
                    for (Rank rank = Rank.One; rank <= Rank.Nine; rank++)
                    {
                        var tileObject = Instantiate(tilePrefab,board.position,Quaternion.identity);
                        var tile = tileObject.GetComponent<Tile>();
                        if(tile != null) 
                        {
                            tile.Init(suit, rank);
                            var index = ((int)suit * 9) + ((int)rank-1); // Rank.One = 1
                            tile.SetImage(index);
                            
                        }
                        RegistTile(tileObject);
                    }
                }
            }

            // ���v

            for(Rank rank = Rank.East; rank <= Rank.Red; rank++) 
            {
                for(int i = 0;i < 4; i++) 
                {
                    Suit suit = Suit.Jihai;
                    var tileObject = Instantiate(tilePrefab, board.position, Quaternion.identity);
                    var tile = tileObject.GetComponent<Tile>();
                    if (tile != null)
                    {
                        tile .Init(suit, rank);
                        var index = ((int) suit*9) + ((int) rank-11); // Rank.East = 11
                        tile.SetImage(index);
                    }
                    RegistTile(tileObject);
                }                
            }
        }
    }
}

