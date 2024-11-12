using System;
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
/// int �r�b�g���Z�Ŕv�����L�^�ł�����
/// �͂�17�C18���̂��߉\
/// 
/// </summary>
/// 

public struct Tiles 
{
    
}
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
    /// ��v�p�^�[�������ׂď����o���X�N���v�g(����F��)
    /// 
    /// ��v�p�^�[��: 118800
    ///               
    ///               405,350(���ʂ̖����̏ꍇ)
    /// </summary>
    public void GenerateALLHandPatternsTEST() 
    {
        int MaxNum = 9;
        int Length = 14;
        int MaxCount = 4;
        //patternTest.Clear();

        GenerateCombinations(MaxNum, Length, MaxCount);

        //Debug.Log($"�p�^�[����{patternTest.Count}");


    }
    /// <summary>
    /// �ċA�֐��ł��ׂĂ̎�v�p�^�[�����L�^����
    /// �C���X�^���X������ɂȂ邽�߁A���I�v��@���g��
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

        // length�ɒB������
        if(currentHand.Count == length) 
        {
            results.Add(new List<sbyte>(currentHand));
            return results;
        }
        //�������Ǘ��p�ɔz��̃T�C�Y�𐧌�
        if(results.Count >= 1000) 
        {
            patternTest.AddRange(results);
            results.Clear();
        }

        //�ċA����
        // !�O��菬�����������Ȃ��悤�ɂ���
        for(int i =startIndex; i <= maxNum; i++) 
        {
            // �����v��4���ȏ����Ȃ��悤�ɂ���
            if (!CheckSameTileLimit(currentHand, i, maxCount)) { continue; }
            currentHand.Add((sbyte)i);
            GeneratePattern(maxNum,length,maxCount,currentHand,results,i);
            currentHand.RemoveAt(currentHand.Count - 1); //�o�b�N�g���b�N

        }
        if(results != null) 
        {
            patternTest.AddRange(results);
        }

        return results;
    }
    /// <summary>
    /// ���I�v��@��p�������ׂĂ̎�v�p�^�[����񋓂���֐�
    /// https://qiita.com/GMR0009/items/adf21c20ea16cc991fb8
    /// </summary>
    /// <param name="tileNum"> �v�̎��</param>
    /// <param name="length"> �v�̖���</param>
    /// <param name="maxCount"> �d�����Ă�������</param>
    private void GenerateCombinations(int tileNum , int length , int maxCount = -1) 
    {
        // �d���������p�^�[��
        if(maxCount == -1) { maxCount = tileNum; }
        // �e�[�u���̏�����
        long[,] dynamicPrograming = new long[tileNum+1, length+1];
        List<List<int>>[,] combinations = new List<List<int>>[tileNum + 1, length + 1];

        // ��������
        // 0��̏ꍇ�A0����I�ԑg�ݍ��킹��1�ʂ�
        dynamicPrograming[0, 0] = 1;
        combinations[0, 0] = new List<List<int>> { new List<int>() };

        // DP���[�v

        for(int i = 1; i <= tileNum; i++) // ��� 
        {
            for(int j = 0; j <= length; j++) // ����
            {

                // combinations[i, j] �� null �̏ꍇ�͏���������
                if (combinations[i, j] == null)
                {
                    combinations[i, j] = new List<List<int>>();
                }
                for (int k = 0; k <= maxCount; k++) // ���݂̎�ނ���k���I�� 
                {
                    if(j >= k) 
                    {
                        dynamicPrograming[i, j] += dynamicPrograming[i - 1, j - k];
                        if(combinations[i - 1, j - k] == null) //null�`�F�b�N
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

        Debug.Log($"�g�ݍ��킹�̑��� : {dynamicPrograming[tileNum, length]}");
    } 


    /// <summary>
    /// �w�薇���ȏ㓯���v������Ȃ��悤�ɂ���
    /// true : �Ȃ� false : ���� 
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
    /// �o�͂����p�^�[�����X�N���v�^�u���I�u�W�F�N�g�ŕۑ�����
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

