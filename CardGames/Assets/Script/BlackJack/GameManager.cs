using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackJack 
{
    /// <summary>
    /// �f�b�L��`�b�v�̊Ǘ�������
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public int PlayerChip;
        List<int> cards;
        public List<int> Cards => cards;

        /// <summary>
        /// �Q�̎���ׂ�
        /// </summary>
        /// <returns></returns>
        public bool Compare() 
        {
            return true;
        }
        /// <summary>
        /// �`�b�v�𑝌�������s
        /// </summary>
        /// <param name="num"></param>
        public void DealChip(int num) 
        {
            
        }


    }
}

