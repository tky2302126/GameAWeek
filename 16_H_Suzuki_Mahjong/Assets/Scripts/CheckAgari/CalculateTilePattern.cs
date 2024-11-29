using System;
using System.Collections.Generic;
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
/// 


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
    ///               
    ///               405,350(普通の麻雀の場合)
    /// </summary>
    public void GenerateALLHandPatternsTEST() 
    {
        int types = 9;
        int maxCount = 4;
        int Length = 14;

        List<int[]> combinations = new List<int[]>();
        List<int[]> agari = new List<int[]>();
        

        GenerateCombinations(types,maxCount,Length,new int[types],0,combinations,agari);

        Debug.Log($"組み合わせの総数 : {combinations.Count}");
        Debug.Log($"上がりの総数 : {agari.Count}");


        HandPattern handPattern = ScriptableObject.CreateInstance<HandPattern>();
        handPattern.handPatterns = combinations;

        string path = "Assets/ALLPatternTest.asset";
        UnityEditor.AssetDatabase.CreateAsset(handPattern, path);
        UnityEditor.AssetDatabase.SaveAssets();
    }

    void GenerateCombinations(int types, int maxCount, int remaining, int[] current, int index, List<int[]> all, List<int[]> agari)
    {
        if (remaining == 0)
        {
            // 条件を満たした組み合わせを記録
            all.Add((int[])current.Clone());

            if (IsAgari(current)) 
            {
                agari.Add((int[])current.Clone());
            }
            return;
        }

        // 領域外は除外
        if (index >= types) return;
        // 再帰部分
        for (int i = 0; i <= Math.Min(maxCount, remaining); i++)
        {
            current[index] = i;
            GenerateCombinations(types, maxCount, remaining - i, current, index + 1, all, agari);
            current[index] = 0;
        }
    }
    /// <summary>
    /// 現在の手牌が和了形か判定する
    /// 
    /// ! 4面子1雀頭のみ実装
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    private bool IsAgari(int[] current)
    {
        return false;
    }

    /// <summary>
    /// 再帰関数ですべての手牌パターンを記録する
    /// インスタンスが巨大になるため、動的計画法を使う
    /// https://qiita.com/GMR0009/items/adf21c20ea16cc991fb8
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
    /// <summary>
    /// 動的計画法を用いたすべての手牌パターンを列挙する関数
    /// https://qiita.com/GMR0009/items/adf21c20ea16cc991fb8
    /// </summary>
    /// <param name="tileNum"> 牌の種類</param>
    /// <param name="length"> 牌の枚数</param>
    /// <param name="maxCount"> 重複していい枚数</param>
    private void GenerateCombinationsDP(int tileNum , int length , int maxCount = -1) 
    {
        // 重複を許すパターン
        if(maxCount == -1) { maxCount = tileNum; }
        // テーブルの初期化
        long[,] dynamicPrograming = new long[tileNum+1, length+1];
        List<List<int>>[,] combinations = new List<List<int>>[tileNum + 1, length + 1];

        // 初期条件
        // 0種の場合、0枚を選ぶ組み合わせは1通り
        dynamicPrograming[0, 0] = 1;
        combinations[0, 0] = new List<List<int>> { new List<int>() };

        // DPループ

        for(int i = 1; i <= tileNum; i++) // 種類 
        {
            for(int j = 0; j <= length; j++) // 枚数
            {

                // combinations[i, j] が null の場合は初期化する
                if (combinations[i, j] == null)
                {
                    combinations[i, j] = new List<List<int>>();
                }
                for (int k = 0; k <= maxCount; k++) // 現在の種類からk枚選ぶ 
                {
                    if(j >= k) 
                    {
                        dynamicPrograming[i, j] += dynamicPrograming[i - 1, j - k];
                        if(combinations[i - 1, j - k] == null) //nullチェック
                        {
                            combinations[i - 1, j - k] = new List<List<int>>();
                        }
                        foreach (var prevCombination in combinations[i - 1, j - k])
                        {
                            var newCombination = new List<int>(prevCombination);
                            newCombination.Add(k);
                            combinations[i, j].Add(newCombination);
                        }
                    }
                }
            }
        }

        Debug.Log($"組み合わせの総数 : {dynamicPrograming[tileNum, length]}");
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
        //TilePattern patterns = ScriptableObject.CreateInstance<TilePattern>();

        //foreach (var item in _patterns)
        //{
        //    patterns.patterns.Add(item);
        //}

        //string path = $"Assets/{assetName}.asset";
        //AssetDatabase.CreateAsset(patterns, path );

        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
    }


}

