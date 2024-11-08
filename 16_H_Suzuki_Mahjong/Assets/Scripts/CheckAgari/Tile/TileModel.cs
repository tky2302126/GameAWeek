
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

    // Žš”v
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

    private int id;
    public int Id => id;

    public void SetModel(int _id) 
    {
        id = _id;
    }
}