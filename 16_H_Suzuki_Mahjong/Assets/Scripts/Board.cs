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

        public int Rows;
        public int Cols;

        int sortingOrder = 1;

        private void Start()
        {
            GenerateTiles();
            //ShuffleTile();
        }

        void RegistTile(GameObject _tileObj) 
        {
            _tileObj.transform.parent = TileHolder;
            _tileObj.transform.localScale = Vector3.one;
            tileObjList.Add(_tileObj);
        }

        void GenerateTiles() 
        {
            // êîîv
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

            // éöîv

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

