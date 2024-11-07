
using System;
using System.Collections.Generic;
using UnityEngine;
public enum Suit
{
    Manzu,
    Pinzu,
    Souzu,
    Jihai
}

public enum Rank
{
    // ””v
    One = 1,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,

    // š”v
    East = 11,
    South,
    West,
    North,
    White,
    Green,
    Red
}

public class TileModel 
{
    // you may write here fields

    private Suit suit;
    public Suit Suit => suit;

    private Rank rank;
    public Rank Rank => rank;

    public void SetModel(Suit _suit, Rank _rank) 
    {
        suit = _suit;
        rank = _rank;
    }
}