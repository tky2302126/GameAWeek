using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheckAgari 
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform tilesHolder;

        public RectTransform TilesHolder => tilesHolder;
    }
}

