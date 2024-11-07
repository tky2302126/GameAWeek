using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheckAgari 
{
    public class BoardPresenter : MonoBehaviour
    {
        private BoardModel model;
        public BoardModel Model => model;

        [SerializeField]
        private BoardView view;

        [SerializeField]
        GameObject tilePrefab;

        private void Start()
        {
            model = new BoardModel();
            Init();
        }

        private void Init() 
        {
            
            for (Suit suit = Suit.Manzu; suit <= Suit.Souzu; suit++) 
            {
                for(Rank rank = Rank.One; rank <= Rank.Nine; rank++) 
                {
                    var tileObj = Instantiate(tilePrefab, new Vector3(0,0,0), Quaternion.identity);
                    var tile = tileObj.GetComponent<TilePresenter>();
                    tile.Init(suit, rank);
                    tileObj.transform.SetParent(view.TilesHolder.transform, false);
                }
            }

            for(Rank rank = Rank.East;  rank <= Rank.Red; rank++) 
            {
                Suit suit = Suit.Jihai;
                var tileObj = Instantiate(tilePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                var tile = tileObj.GetComponent<TilePresenter>();
                tile.Init(suit, rank);
                tileObj.transform.SetParent(view.TilesHolder.transform, false);
            }
        }


    }
}

