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
        

        Debug.Log("�S�Ă̎�v�p�^�[����: " + allHandPatterns.patterns.Count);
    }

    // �ċA�֐���14���̎�v�𐶐�
    private List<List<sbyte>> GenerateCombinations(sbyte[] tiles, int targetLength, List<sbyte> currentHand = null, List<List<sbyte>> results = null)
    {
        currentHand ??= new List<sbyte>();
        results ??= new List<List<sbyte>>();

        // �I������: 14���̎�v�����ꍇ
        if (currentHand.Count == targetLength)
        {
            results.Add(new List<sbyte>(currentHand));
            return results;
        }

        // �ċA�X�e�b�v
        for (int i = 0; i < tiles.Length; i++)
        {
            currentHand.Add(tiles[i]);
            GenerateCombinations(tiles, targetLength, currentHand, results);
            currentHand.RemoveAt(currentHand.Count - 1); // �ǉ������v���폜���ăo�b�N�g���b�N
        }

        return results;
    }

    // �S�Ă̎�v�p�^�[���𐶐�����
    private List<List<sbyte>> GenerateAllHandPatterns(int targetLength)
    {
        sbyte[] tiles = { 1, 2, 3, 4, 5, 6, 7, 8, 9}; // �ݎq�̔v
        return GenerateCombinations(tiles, targetLength);
    }

    private void GenerateAndSavePatterns(int targetLength)
    {
        var patternsBatch = new List<List<sbyte>>();
        var totalPatterns = GenerateAllHandPatterns(targetLength);

        foreach (var pattern in totalPatterns)
        {
            patternsBatch.Add(pattern);

            if (patternsBatch.Count >= 1000) // ��x��1000�p�^�[�����ۑ�
            {
                allHandPatterns.patterns.AddRange(patternsBatch);
                patternsBatch.Clear();
            }
        }

        if (patternsBatch.Count > 0) // �c���ۑ�
        {
            allHandPatterns.patterns.AddRange(patternsBatch);
        }
    }
}

