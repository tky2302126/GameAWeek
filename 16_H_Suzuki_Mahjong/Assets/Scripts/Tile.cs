using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // êîîv
        One = 1,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,

        // éöîv
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
        private SpriteRenderer spriteRenderer;

        public Tile(Suit _suit, Rank _rank)
        {
            Suit = _suit;
            Rank = _rank;
        }

        [SerializeField]
        private TileImage images;

        public void SetImage(int  index) 
        {
            if(spriteRenderer != null) 
            {
                spriteRenderer.sprite = images.sprites[index];
            }
            else 
            {
                spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = images.sprites[index];
            }
        }


    }
}


