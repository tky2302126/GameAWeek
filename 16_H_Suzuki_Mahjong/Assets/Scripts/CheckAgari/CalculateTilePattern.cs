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
/// 
/// 
/// </summary>
public class CalculateTilePattern : MonoBehaviour
{
    string folderPath = "Assets/Scripts/CheckAgari";

    public string fileName_ALL;

    public string fileName_Agari;
    public void CalculateALLPatterns() 
    {
        
    }

    /// <summary>
    /// 手牌パターンをすべて書き出すスクリプト(清一色版)
    /// 
    /// 手牌パターン: 118800
    /// </summary>
    public void GenerateALLHandPatternsTEST() 
    {
        int MaxNum = 8;
        int Length = 14;
        int MaxCount = 4;


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


[CreateAssetMenu(fileName = "AllHandPatterns", menuName = "Mahjong/AllHandPatterns")]
public class AllHandPatterns : ScriptableObject
{
    public List<List<sbyte>> patterns = new List<List<sbyte>>();
}

public class AllHandPatternGenerator : MonoBehaviour
{
    [SerializeField] private AllHandPatterns allHandPatterns;

    void Start()
    {
        allHandPatterns.patterns.Clear();
        var patterns = GenerateAllHandPatterns(14);
        foreach (var pattern in patterns)
        {
            allHandPatterns.patterns.Add(pattern);
        }

        Debug.Log("全ての手牌パターン数: " + allHandPatterns.patterns.Count);
    }

    // 再帰関数で14枚の手牌を生成
    private List<List<sbyte>> GenerateCombinations(sbyte[] tiles, int targetLength, List<sbyte> currentHand = null, List<List<sbyte>> results = null)
    {
        currentHand ??= new List<sbyte>();
        results ??= new List<List<sbyte>>();

        // 終了条件: 14枚の手牌を持つ場合
        if (currentHand.Count == targetLength)
        {
            results.Add(new List<sbyte>(currentHand));
            return results;
        }

        // 再帰ステップ
        for (int i = 0; i < tiles.Length; i++)
        {
            currentHand.Add(tiles[i]);
            GenerateCombinations(tiles, targetLength, currentHand, results);
            currentHand.RemoveAt(currentHand.Count - 1); // 追加した牌を削除してバックトラック
        }

        return results;
    }

    // 全ての手牌パターンを生成する
    private List<List<sbyte>> GenerateAllHandPatterns(int targetLength)
    {
        sbyte[] tiles = { 1, 2, 3, 4, 5, 6, 7, 8, 9 }; // 萬子の牌
        return GenerateCombinations(tiles, targetLength);
    }
}

