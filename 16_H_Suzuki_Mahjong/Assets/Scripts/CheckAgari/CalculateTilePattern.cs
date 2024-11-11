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
    ///               143975635
    ///               ��������
    ///               �f�[�^�ʂ��������ă��������[�N���N�����Ă���
    ///               405,350(���ʂ̖����̏ꍇ)
    /// </summary>
    public void GenerateALLHandPatternsTEST() 
    {
        int MaxNum = 8;
        int Length = 14;
        int MaxCount = 4;
        patternTest.Clear();

        GeneratePattern(MaxNum, Length, MaxCount);

        Debug.Log($"�p�^�[����{patternTest.Count}");


    }
    /// <summary>
    /// �ċA�֐��ł��ׂĂ̎�v�p�^�[�����L�^����
    /// �C���X�^���X������ɂȂ邽�߁A�������ċA�֐����g��
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

    private List<byte[]> GeneratePatternByte(int maxNum, int length, int maxCount,
                                         byte[] currentHand = null, List<byte[]> results = null,
                                         int startIndex = 0) 
    {
        currentHand ??= new byte[5];
        results ??= new List<byte[]>();



        return results;
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

