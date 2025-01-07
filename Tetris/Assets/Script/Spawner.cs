using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> tetrominoes; // �e�g���~�m�̃��X�g
    private Queue<GameObject> nextTetrominoes; // ���ɏo������e�g���~�m�̃L���[
    public Transform nextPreviewPosition; // ���̃e�g���~�m��\������ʒu
    private GameObject previewInstance; // ���̃e�g���~�m�̃v���r���[�p�C���X�^���X

    private void Start()
    {
        nextTetrominoes = new Queue<GameObject>();
        GenerateNextSet(); // �ŏ���7�̃Z�b�g�𐶐�
        SpawnNext(); // �ŏ��̃e�g���~�m���X�|�[��
    }

    private void GenerateNextSet()
    {
        List<GameObject> shuffledTetrominoes = new List<GameObject>(tetrominoes);
        // �����_���ɃV���b�t��
        for (int i = 0; i < shuffledTetrominoes.Count; i++)
        {
            int randomIndex = Random.Range(i, shuffledTetrominoes.Count);
            GameObject temp = shuffledTetrominoes[i];
            shuffledTetrominoes[i] = shuffledTetrominoes[randomIndex];
            shuffledTetrominoes[randomIndex] = temp;
        }

        // �L���[�ɒǉ�
        foreach (var tetromino in shuffledTetrominoes)
        {
            nextTetrominoes.Enqueue(tetromino);
        }
    }

    public void SpawnNext()
    {
        // ���̃e�g���~�m���擾
        if (nextTetrominoes.Count == 1)
        {
            GenerateNextSet(); // �V�����Z�b�g�𐶐�
        }
        GameObject nextTetromino = nextTetrominoes.Dequeue();

        // �e�g���~�m���X�|�[��
        Instantiate(nextTetromino, transform.position, Quaternion.identity);

        // ���̃e�g���~�m���v���r���[�ʒu�ɕ\��
        PreviewNext();
    }

    private void PreviewNext()
    {
        // �v���r���[�I�u�W�F�N�g�����ɑ��݂��Ă���ꍇ�A�폜���Ȃ�
        if (previewInstance != null)
        {
            Destroy(previewInstance);
        }

        // �L���[�̐擪���v���r���[�Ƃ��ĕ\��
        if (nextTetrominoes.Count > 0)
        {
            GameObject previewTetromino = nextTetrominoes.Peek();
            previewInstance = Instantiate(previewTetromino, nextPreviewPosition.position, Quaternion.identity);
            previewInstance.transform.SetParent(nextPreviewPosition); // �v���r���[�p�̐e�q�֌W��ݒ�
        }
    }
}
