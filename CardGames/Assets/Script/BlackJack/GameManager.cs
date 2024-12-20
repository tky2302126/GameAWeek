using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlackJack 
{
    /// <summary>
    /// デッキやチップの管理をする
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public int PlayerChip;
        List<int> cards;
        public List<int> Cards => cards;

        /// <summary>
        /// ２つの手を比べる
        /// </summary>
        /// <returns></returns>
        public bool Compare() 
        {
            return true;
        }
        /// <summary>
        /// チップを増減させるs
        /// </summary>
        /// <param name="num"></param>
        public void DealChip(int num) 
        {
            
        }


    }
}

