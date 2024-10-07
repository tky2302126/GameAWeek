using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using uPalette.Runtime.Core;
using uPalette.Runtime.Core.Synchronizer.Color;

public static class ViewUtility
{
    public static Vector2 GetAnchoredPosition(this BoardPosition position)
    {
        return new Vector2(46 + 196 * position.col, -(46 + 196 * position.row));
    }
}

public class CellView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;

    [SerializeField]
    private Transform _animationroot;

    [SerializeField]
    private float _moveDuration;

    [SerializeField]
    private GraphicColorSynchronizer _BGColorSynchronizer;

    [SerializeField]
    private GraphicColorSynchronizer _TextColorSynchronizer;

    [SerializeField]
    private List<ColorEntryId> colorEntryIds;

    [SerializeField]
    private ColorEntryId text_black;

    [SerializeField]
    private ColorEntryId text_gray;

    public BoardPosition CurrentBoardPosition {  get; private set; }

    public bool IsShow {  get; private set; }
    
    public void Set(BoardPosition position,int number) 
    {
        IsShow = true;
        gameObject.SetActive(true);
        CurrentBoardPosition = position;
        ((RectTransform)transform).anchoredPosition = position.GetAnchoredPosition();
        SetNumber(number);
    }

    private void SetNumber(int number) 
    {
        _text.SetText(number.ToString());
        //!数値によって色を変える
        //! switch case
        
        // 字の色
        switch (number) 
        {
            case 2 or 4 or 8 or 16 or 32 or 64:
                _TextColorSynchronizer.SetEntryId(text_gray.Value);
                break;

            default:
                _TextColorSynchronizer.SetEntryId(text_black.Value);
                break;
            
            

        }

        switch (number) 
        {
            case 2:
                _BGColorSynchronizer.SetEntryId(colorEntryIds[0].Value);
                break;

            case 4:
                _BGColorSynchronizer.SetEntryId(colorEntryIds[1].Value);
                break;

            case 8:
                _BGColorSynchronizer.SetEntryId(colorEntryIds[2].Value);
                break;

            case 16:
                _BGColorSynchronizer.SetEntryId(colorEntryIds[3].Value);
                break;
            case 32:
                _BGColorSynchronizer.SetEntryId(colorEntryIds[4].Value);
                break;

            case 64:
                _BGColorSynchronizer.SetEntryId(colorEntryIds[5].Value);
                break;

            case 128:
                _BGColorSynchronizer.SetEntryId(colorEntryIds[6].Value);
                break;

            case 256:
                _BGColorSynchronizer.SetEntryId(colorEntryIds[7].Value);
                break;

            case 512:
                _BGColorSynchronizer.SetEntryId(colorEntryIds[8].Value);
                break;

            case 1024:
                _BGColorSynchronizer.SetEntryId(colorEntryIds[9].Value);
                break;

            case 2048:
                _BGColorSynchronizer.SetEntryId(colorEntryIds[10].Value);
                break;

            case 4096:
                _BGColorSynchronizer.SetEntryId(colorEntryIds[11].Value);
                break;

            case 8192:
                _BGColorSynchronizer.SetEntryId(colorEntryIds[12].Value);
                break;

            default:
                _BGColorSynchronizer.SetEntryId(colorEntryIds[13].Value);
                break;
        }

    }

    //!アニメーション制御の関数を実装
    /// <summary>
    /// アニメーション仕様
    /// 指定のポイントまで動く
    /// </summary>
    /// 
    public IEnumerator PlayMoveCoroutine(BoardPosition position, int number) 
    {
        if (!IsShow) { yield break; }

        CurrentBoardPosition = position;
        SetNumber(number);
        var startPosition = ((RectTransform)transform).anchoredPosition;
        var endPosition = position.GetAnchoredPosition();
        var progress = 0f;

        //Debug.Log( $"{CurrentBoardPosition.row}{CurrentBoardPosition.col}");

        while (progress < 1f)
        {
            progress += Time.deltaTime / Mathf.Max(Mathf.Epsilon, _moveDuration);
            ((RectTransform)transform).anchoredPosition = Vector3.Lerp(startPosition, endPosition, progress);
            progress = Mathf.Clamp01(progress);
            yield return null;
        }
    }

    public IEnumerator PlaySpawnCoroutine(BoardPosition position, int number) 
    {
        Set(position,number);

        var progress = 1.0f;

        while (progress < 1.0f) 
        {
            progress += Time.deltaTime / Mathf.Max(Mathf.Epsilon, _moveDuration);
            _animationroot.localScale = Vector3.one * Mathf.Clamp01(progress);
            yield return null;
        }
    }

    public void Hide() 
    {
        IsShow = false;
        gameObject?.SetActive(false);
    }
}
