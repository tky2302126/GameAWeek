using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// �����̎�v�p�^�[�����X�N���v�^�u���I�u�W�F�N�g�ŕۑ�����
/// 
/// �������񐴈�F�����ōl����
/// 
/// ���ׂĂ̎�v�p�^�[����ۑ�����
/// 
/// ��v�p�^�[���̃��[��
/// 1��ɂ�4���܂�
/// ���v��14��
/// 
/// ���ׂĂ̂������v�p�^�[����ۑ�����
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
    /// ��v�p�^�[�������ׂď����o���X�N���v�g(����F��)
    /// 
    /// ��v�p�^�[��: 118800
    /// </summary>
    public void GenerateALLHandPatternsTEST() 
    {
        int MaxNum = 8;
        int Length = 14;
        int MaxCount = 4;


    }
    /// <summary>
    /// �o�͂����p�^�[�����X�N���v�^�u���I�u�W�F�N�g�ŕۑ�����
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
        sbyte[] tiles = { 1, 2, 3, 4, 5, 6, 7, 8, 9 }; // �ݎq�̔v
        return GenerateCombinations(tiles, targetLength);
    }
}

