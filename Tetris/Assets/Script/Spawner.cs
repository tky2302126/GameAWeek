using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> tetrominoes; // テトリミノのリスト
    private Queue<GameObject> nextTetrominoes; // 次に出現するテトリミノのキュー
    public Transform nextPreviewPosition; // 次のテトリミノを表示する位置
    private GameObject previewInstance; // 次のテトリミノのプレビュー用インスタンス

    private void Start()
    {
        nextTetrominoes = new Queue<GameObject>();
        GenerateNextSet(); // 最初の7個のセットを生成
        SpawnNext(); // 最初のテトリミノをスポーン
    }

    private void GenerateNextSet()
    {
        List<GameObject> shuffledTetrominoes = new List<GameObject>(tetrominoes);
        // ランダムにシャッフル
        for (int i = 0; i < shuffledTetrominoes.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledTetrominoes.Count);
            GameObject temp = shuffledTetrominoes[i];
            shuffledTetrominoes[i] = shuffledTetrominoes[randomIndex];
            shuffledTetrominoes[randomIndex] = temp;
        }

        // キューに追加
        foreach (var tetromino in shuffledTetrominoes)
        {
            nextTetrominoes.Enqueue(tetromino);
        }
    }

    public void SpawnNext()
    {
        // 次のテトリミノを取得
        if (nextTetrominoes.Count == 1)
        {
            GenerateNextSet(); // 新しいセットを生成
        }
        GameObject nextTetromino = nextTetrominoes.Dequeue();

        // テトリミノをスポーン
        Instantiate(nextTetromino, transform.position, Quaternion.identity);

        // 次のテトリミノをプレビュー位置に表示
        PreviewNext();
    }

    private void PreviewNext()
    {
        // プレビューオブジェクトが既に存在している場合、削除しない
        if (previewInstance != null)
        {
            Destroy(previewInstance);
        }

        // キューの先頭をプレビューとして表示
        if (nextTetrominoes.Count > 0)
        {
            GameObject previewTetromino = nextTetrominoes.Peek();
            previewInstance = Instantiate(previewTetromino, nextPreviewPosition.position, Quaternion.identity);
            previewInstance.transform.SetParent(nextPreviewPosition); // プレビュー用の親子関係を設定
        }
    }
}
