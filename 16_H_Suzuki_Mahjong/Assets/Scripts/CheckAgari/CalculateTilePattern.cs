using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 麻雀の手牌パターンをスクリプタブルオブジェクトで保存する
/// 
/// いったん清一色麻雀で考える
/// 
/// すべての手牌パターンを保存する
/// 
/// 手牌パターンのルール
/// 1種につき4枚まで
/// 合計で14枚
/// 
/// すべてのあがり手牌パターンを保存する
/// int ビット演算で牌情報を記録できそう
/// 河は17，18枚のため可能
/// 
/// </summary>
public class CalculateTilePattern : MonoBehaviour
{
    string folderPath = "Assets/Scripts/CheckAgari";

    public string fileName_ALL;

    public string fileName_Agari;

    public List<List<sbyte>> patternTest;

    private List<byte[]> patternTestByte = new List< byte[]>();

    private void Start()
    {
        if(patternTest == null) { patternTest = new List<List<sbyte>>(); }
    }
    public void CalculateALLPatterns() 
    {
        
    }

    /// <summary>
    /// 手牌パターンをすべて書き出すスクリプト(清一色版)
    /// 
    /// 手牌パターン: 118800
    ///               143975635
    ///               多すぎる
    ///               データ量が多すぎてメモリリークを起こしてそう
    ///               405,350(普通の麻雀の場合)
    /// </summary>
    public void GenerateALLHandPatternsTEST() 
    {
        int MaxNum = 8;
        int Length = 14;
        int MaxCount = 4;
        patternTest.Clear();

        GeneratePattern(MaxNum, Length, MaxCount);

        Debug.Log($"パターン数{patternTest.Count}");


    }
    /// <summary>
    /// 再帰関数ですべての手牌パターンを記録する
    /// インスタンスが巨大になるため、メモ化再帰関数を使う
    /// 
    /// </summary>
    /// <param name="maxNum"></param>
    /// <param name="length"></param>
    /// <param name="maxCount"></param>
    /// <param name="currentHand"></param>
    /// <param name="results"></param>
    private List<List<sbyte>> GeneratePattern(int maxNum, int length, int maxCount, List<sbyte> currentHand = null, List<List<sbyte>> results = null , int startIndex =0) 
    {
        currentHand ??= new List<sbyte>();
        results ??= new List<List<sbyte>>();

        // lengthに達した時
        if(currentHand.Count == length) 
        {
            results.Add(new List<sbyte>(currentHand));
            return results;
        }
        //メモリ管理用に配列のサイズを制御
        if(results.Count >= 1000) 
        {
            patternTest.AddRange(results);
            results.Clear();
        }

        //再帰部分
        // !前より小さい数を入れないようにする
        for(int i =startIndex; i <= maxNum; i++) 
        {
            // 同じ牌が4枚以上入らないようにする
            if (!CheckSameTileLimit(currentHand, i, maxCount)) { continue; }
            currentHand.Add((sbyte)i);
            GeneratePattern(maxNum,length,maxCount,currentHand,results,i);
            currentHand.RemoveAt(currentHand.Count - 1); //バックトラック

        }
        if(results != null) 
        {
            patternTest.AddRange(results);
        }

        return results;
    }

    private List<byte[]> GeneratePatternByte(int maxNum, int length, int maxCount,
                                         byte[] currentHand = null, List<byte[]> results = null,
                                         int startIndex = 0) 
    {
        currentHand ??= new byte[5];
        results ??= new List<byte[]>();



        return results;
    }



    /// <summary>
    /// 指定枚数以上同じ牌が入らないようにする
    /// true : なし false : あり 
    /// </summary>
    /// <param name="currentHand"></param>
    /// <param name="target"></param>
    /// <param name="maxCount"></param>
    /// <returns></returns>
    private bool CheckSameTileLimit(List<sbyte> currentHand, int target, int maxCount)
    {
        bool result = true;
        if(currentHand.Count <= 3 ) { return true; }

        int count = 0; 

        for(int i = 0; i< currentHand.Count; i++) 
        {
            if (currentHand[i] == (sbyte)target) { count++; }
        }

        if(count >= maxCount) {  return false; }

        return result;
    }

    /// <summary>
    /// 出力したパターンをスクリプタブルオブジェクトで保存する
    /// </summary>
    /// <param name="_patterns"></param>
    /// <param name="assetName"></param>
    private void SaveTilePattern(List<List<sbyte>> _patterns , string assetName) 
    {
        TilePattern patterns = ScriptableObject.CreateInstance<TilePattern>();

        foreach (var item in _patterns)
        {
            patterns.patterns.Add(item);
        }

        string path = $"Assets/{assetName}.asset";
        AssetDatabase.CreateAsset(patterns, path );

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


}

