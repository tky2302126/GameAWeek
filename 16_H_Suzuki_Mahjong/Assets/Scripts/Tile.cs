using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace ShisenSho 
{
    public enum State 
    {
        None,
        IsAlive,
        Selected
    }
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

        public State State { get; private set; } = State.None;


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

        public void SetState(State state) 
        {
            State = state;
        }
    }
}


