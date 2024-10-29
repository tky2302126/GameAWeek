using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace ShisenSho 
{
    public enum Suit
    {
        Manzu,
        Pinzu,
        Souzu,
        Jihai
    }

    public enum Rank
    {
        // ���v
        One = 1,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,

        // ���v
        East = 11,
        South,
        West,
        North,
        White,
        Green,
        Red
    }

    public class Tile : MonoBehaviour
    {

        public Suit Suit { get; private set; }
        public Rank Rank { get; private set; }

        [SerializeField]
        private Image image;

        [SerializeField]
        private TileImage images;

        public Tile(Suit _suit, Rank _rank)
        {
            Suit = _suit;
            Rank = _rank;
        }

        public void Init(Suit _suit, Rank _rank) 
        {
            Suit = _suit;
            Rank = _rank;
        }

        

        public void SetImage(int  index) 
        {
            image.sprite = images.sprites[index];
        }


    }
}


