using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        GenerateAndSavePatterns(14);
        

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
        sbyte[] tiles = { 1, 2, 3, 4, 5, 6, 7, 8, 9}; // 萬子の牌
        return GenerateCombinations(tiles, targetLength);
    }

    private void GenerateAndSavePatterns(int targetLength)
    {
        var patternsBatch = new List<List<sbyte>>();
        var totalPatterns = GenerateAllHandPatterns(targetLength);

        foreach (var pattern in totalPatterns)
        {
            patternsBatch.Add(pattern);

            if (patternsBatch.Count >= 1000) // 一度に1000パターンずつ保存
            {
                allHandPatterns.patterns.AddRange(patternsBatch);
                patternsBatch.Clear();
            }
        }

        if (patternsBatch.Count > 0) // 残りを保存
        {
            allHandPatterns.patterns.AddRange(patternsBatch);
        }
    }
}

