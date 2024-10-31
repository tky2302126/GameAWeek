using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;



namespace ShisenSho 
{
    public class Player : MonoBehaviour
    {
        private List<GameObject> selectedTiles = new List<GameObject>();
        private GameObject selectedTile1;
        private GameObject selectedTile2;

        private Vector2Int index1;
        private Vector2Int index2;

        // いらなそう
        [SerializeField]
        private Board Board;

        //[SerializeField]
        //float distance = 10.0f;

        //[SerializeField]
        //bool drawRayDump = false;

        [SerializeField]
        private InputActionAsset input;

        public int remainArrayCount;
        public bool isStucked;

        private void Start()
        {
            input.FindAction("Click").canceled += OnClick;
            input.Enable();
        }

        private void OnDisable()
        {
            input.FindAction("Click").canceled -= OnClick;
            input.Disable();
        }

        private void Update()
        {
            //if (drawRayDump) 
            //{
            //    if(selectedTiles.Count > 0) 
            //    {
            //        foreach(var tile in selectedTiles) 
            //        {
            //            Vector2 pos = tile.transform.position;

            //            Debug.DrawRay(pos,Vector2.up * distance,Color.red);
            //            Debug.DrawRay(pos,Vector2.down * distance,Color.green);
            //            Debug.DrawRay(pos,Vector2.left * distance,Color.yellow);
            //            Debug.DrawRay(pos,Vector2.right * distance,Color.blue);
            //        }
            //    }
            //}
        }

        public void OnClick(InputAction.CallbackContext context) 
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.tag == "Tile")
            {
                var clickedTile = hit.collider.gameObject;

                HandleTileClick(clickedTile);
            }
        }

        private void HandleTileClick(GameObject clickedTile)
        {
            if (selectedTiles.Contains(clickedTile)) 
            {
                selectedTiles.Remove(clickedTile);
                var tile = clickedTile.GetComponent<Tile>();
                tile.SetState(State.None);
                SetTileTransparency(clickedTile, 1.0f);
#if UNITY_EDITOR
                //drawRayDump = false;
#endif
            }
            else 
            {
                selectedTiles.Add(clickedTile);
                var tile = clickedTile.GetComponent<Tile>();
                tile.SetState(State.Selected);
                SetTileTransparency(clickedTile, 0.5f);
#if UNITY_EDITOR
                //drawRayDump = true;
#endif


                if(selectedTiles.Count == 2) 
                {
                    selectedTile1 = selectedTiles[0];
                    selectedTile2 = selectedTiles[1];

                    if (IsMatchingPair(selectedTile1, selectedTile2)) 
                    {
                        // ペアのマッチングが成立した場合の処理をここに記述

                        Debug.Log("Matching Pair!");



                        // 成立したペアを非表示にするなどのアクションを実行
                        var tile1 = selectedTile1.GetComponent<Tile>();
                        tile1.SetState(State.None);
                        var tile2 = selectedTile2.GetComponent<Tile>();
                        tile2.SetState(State.None);
                        var image1 = selectedTile1.GetComponent<Image>();
                        var image2 = selectedTile2.GetComponent<Image>();
                        var boxcollider1 = tile1.GetComponent<BoxCollider2D>();
                        var boxcollider2 = tile2.GetComponent<BoxCollider2D>();
                        image1.enabled = false;
                        image2.enabled = false;
                        boxcollider1.enabled = false;
                        boxcollider2.enabled = false;
                        //selectedTile1.SetActive(false);
                        //selectedTile2.SetActive(false);

                        //　手詰まりかどうかチェックして。

                        IsStuckCheck();

                        //  残りの牌を数えて。
                        remainArrayCount = 0;
                        foreach(var item in Board.TileArray) 
                        {
                            if (item != null && item.State == State.IsAlive) 
                            {
                                remainArrayCount++;
                            }
                        }

                        //  クリアしたのか、手詰まりなのか表示する処理
                        if(isStucked && remainArrayCount == 0) 
                        {
                            Debug.Log("Game Clear!");
                        }
                        else if (isStucked) 
                        {
                            Debug.LogWarning("It's Stacked");
                        }
                    }
                    else 
                    {
                        // ペアのマッチングが成立しなかった場合の処理をここに記述

                        Debug.Log("Not a Matching Pair");



                        // 選択した2つの麻雀牌を再び非選択状態に戻す。
                        var tile1 = selectedTile1.GetComponent<Tile>();
                        tile1.SetState(State.IsAlive);
                        var tile2 = selectedTile2.GetComponent<Tile>();
                        tile2.SetState(State.IsAlive);
                        //  元の色に戻す処理

                        foreach (var item in selectedTiles)
                        {

                            SetTileTransparency(item, 1.0f);

                        }

                    }

                    // 選択した麻雀牌をリストから削除

                    selectedTiles.Clear();
                }
                }
            }

        public void IsStuckCheck()
        {
            List<GameObject> matchingList  = new List<GameObject>();
            HashSet<GameObject> processedTiles = new HashSet<GameObject>();

            for(int y = 0; y < Board.TileArray.GetLength(1)-1; y++) 
            {
                for(int x = 0; x< Board.TileArray.GetLength(0)-1; x++) 
                {
                    //Vector2Int buff = new(Board.TileArray.GetLength(0), Board.TileArray.GetLength(1));
                    try { if (Board.TileArray[x, y] == null) { continue; } }

                    catch 
                    {
                        Vector2Int buff = new(x, y);
                        Debug.Log(buff);
                    }
                    
                    if (Board.TileArray[x, y].gameObject == null) { continue; }
                    var crrTileObj = Board.TileArray[x, y].gameObject;
                    var crrTile = crrTileObj.GetComponent<Tile>();
                    if(crrTile.State == State.None ) 
                    {
                        continue;
                    }
                    if(processedTiles.Contains(crrTileObj)) { continue; }
                    if(crrTileObj.gameObject == null) { continue; }

                    foreach (var other in Board.TileArray ) 
                    {
                        if( other == crrTileObj ) { continue; }
                        if(other == null) { continue; }
                        if(other.gameObject == null) { continue; }
                        if(other.State == State.None ) { continue; }
                        if (IsMatchingPair(crrTileObj.gameObject,other.gameObject ))
                        {
                            matchingList.Add(crrTileObj);
                            matchingList.Add(other.gameObject);

                            processedTiles.Add(crrTileObj);
                            processedTiles.Add(other.gameObject);

                            //? State変更処理 いるか確認
                        }
                        else 
                        {
                            //? State変更処理 いるか確認
                        }
                    }

                }
            }

            if(matchingList.Count > 0) { isStucked = false; }
            else { isStucked = true; }
            matchingList.Clear();
            processedTiles.Clear();
        }

        private bool IsMatchingPair(GameObject selectedTile1, GameObject selectedTile2)
        {
            var result = false;
            var tile1 = selectedTile1.GetComponent<Tile>();
            var tile2 = selectedTile2.GetComponent<Tile>();
            // 経路探索
            if (IsMatchingPaiInfo(tile1,tile2)) 
            {
                index1 = GetTileArrayIndex(selectedTile1);
                index2 = GetTileArrayIndex(selectedTile2);

                if (HasValidPathBetweenTilesXYX(index1, index2)) 
                {
                    result = true;
                }

                else if(HasValidPathBetweenTilesYXY(index1, index2)) 
                {
                    result = true;
                }
            }


            return result;
        }

        
        private Vector2Int GetTileArrayIndex(GameObject tileObj)
        {
            var tile = tileObj.GetComponent<Tile>();
            for(int y =0; y <Board.TileArray.GetLength(0); y++) 
            {
                for( int x=0; x<Board.TileArray.GetLength(1); x++) 
                {
                    if (Board.TileArray[y,x] == tile) 
                    {
                        return new Vector2Int(y, x);
                    }
                }
            }

            return Vector2Int.zero;
        }

        private bool HasValidPathBetweenTilesYXY(Vector2Int index1, Vector2Int index2)
        {
            //二つの麻雀牌のもっとも遠い左右の空白セルのX座標（横軸）を調べる。
            int downA = GetEmptyPositionToDown(index1);
            int upA = GetEmptyPositionToUp(index1);
            int downB = GetEmptyPositionToDown(index2);
            int upB = GetEmptyPositionToUp(index2);

            int downY = Mathf.Max(downA, downB);
            int upY = Mathf.Min(upA, upB);


            //重複部分があるか調べる準備
            //1つめのタイルが上にある場合
            if (index1[0] <= index2[0] && upB>= downA) 
            {
                // 経路が見つかったらtrue
                return HasHorizontalBlanksInRectAngle(downY, upY);
            }
            //1つめのタイルが下にある場合
            else if (index1[0] > index2[0] && upB>= upA) 
            {
                return HasHorizontalBlanksInRectAngle(downY, upY);
            }
            else 
            {
                //見つからなければfalse
                return false;
            }
            
            bool HasHorizontalBlanksInRectAngle(int Down, int Up)
            {
                int minX = Mathf.Min(index1[1], index2[1]);
                int maxX = Mathf.Max(index1[1], index2[1]);
                int absValue = maxX - minX;
                int emptyCellCount;
                for(int y = Down; y <= Up; y++) 
                {
                    emptyCellCount = 0;
                    for(int x = minX; x <= maxX; x++) 
                    {
                        if (Board.TileArray[y,x].State == State.None) 
                        {
                            emptyCellCount++;
                            if(emptyCellCount == absValue + 1) 
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            
        }

        private bool HasValidPathBetweenTilesXYX(Vector2Int index1, Vector2Int index2)
        {
            //二つの麻雀牌のもっとも遠い上下の空白セルのY座標（縦軸）を調べる。
            int leftA = GetEmptyPositionToLeft(index1);
            int rightA = GetEmptyPositionToRight(index1);
            int leftB = GetEmptyPositionToLeft(index2);
            int rightB = GetEmptyPositionToRight(index2);


            //
            int leftX = Mathf.Max(leftA, leftB);
            int rightX = Mathf.Min(rightA, rightB);

            // 1つめのタイルが左にある時
            if (index1[1] <= index2[1] && rightA >= leftB) 
            {
                // 経路が見つかったらtrue
                return HasVerticalBlanksRectAngle(leftX, rightX);
            }
            // 1つめのタイルが右にある時
            else if (index1[1] > index2[1] && rightB >= leftA) 
            {
                return HasVerticalBlanksRectAngle(leftX, rightX);
            } 
            
            else 
            {
                //見つからなければfalse
                return false;
            }

            bool HasVerticalBlanksRectAngle(int Left, int Right)
            {
                int minY = Mathf.Min(index1[0], index2[0]);
                int maxY = Mathf.Max(index1[0], index2[0]);

                int absValue = maxY - minY;

                int emptycellCount;

                for(int x =Left;x <= Right; x++) 
                {
                    emptycellCount = 0;
                    for(int y = minY; y <= maxY; y++) 
                    {
                        if (Board.TileArray[y,x].State == State.None) 
                        {
                            emptycellCount++;

                            if(emptycellCount == absValue + 1) 
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }

        private int GetEmptyPositionToLeft(Vector2Int tileIndex)
        {
            for (int x = tileIndex[1] - 1; x >= 0; x--)
            {
                if (Board.TileArray[tileIndex[0], x].State == State.IsAlive)
                {
                    return x+1;
                }
            }
            return 0;
        }

        private int GetEmptyPositionToRight(Vector2Int tileIndex) 
        {
            for(int  x = tileIndex[1] +1; x < Board.TileArray.GetLength(1); x++) 
            {
                if (Board.TileArray[tileIndex[0],x].State == State.IsAlive) 
                {
                    return x - 1;
                }
            }

            return 18;
        }

        private int GetEmptyPositionToDown(Vector2Int tileIndex) 
        {
            for(int y = tileIndex[0]-1;y>= 0; y--) 
            {
                if (Board.TileArray[y, tileIndex[1]].State == State.IsAlive) 
                {
                    return y + 1;
                }
            }
            return 0;
        }

        private int GetEmptyPositionToUp(Vector2Int tileIndex) 
        {
            for (int y = tileIndex[0] + 1; y < Board.TileArray.GetLength(0); y++)
            {
                if (Board.TileArray[y, tileIndex[1]].State == State.IsAlive)
                {
                    return y -1;
                }
            }
            return 9;
        }

        private bool IsMatchingPaiInfo()
        {
            var tile1 = selectedTile1.GetComponent<Tile>();
            var tile2 = selectedTile2.GetComponent<Tile>();

            return tile1.Suit == tile2.Suit && tile1.Rank == tile2.Rank;
        }

        private bool IsMatchingPaiInfo(Tile tile1,Tile tile2) 
        {
            return tile1.Suit == tile2.Suit && tile1.Rank == tile2.Rank;
        }

        private void SetTileTransparency(GameObject clickedTile, float alpha)
        {
            var image = clickedTile.GetComponent<Image>();
            var tileColor = image.color;
            tileColor.a = alpha;
            image.color = tileColor;
        }

        public void OnClickButton() 
        {
            OnClickHint().Forget();
        }
        private async UniTaskVoid OnClickHint() 
        {
            List<GameObject> matchingList = new List<GameObject>();
            HashSet<GameObject> processedTiles = new HashSet<GameObject>();

            for (int y = 0; y < Board.TileArray.GetLength(1) - 1; y++)
            {
                for (int x = 0; x < Board.TileArray.GetLength(0) - 1; x++)
                {
                    //Vector2Int buff = new(Board.TileArray.GetLength(0), Board.TileArray.GetLength(1));
                    try { if (Board.TileArray[x, y] == null) { continue; } }

                    catch
                    {
                        Vector2Int buff = new(x, y);
                        Debug.Log(buff);
                    }

                    if (Board.TileArray[x, y].gameObject == null) { continue; }
                    var crrTileObj = Board.TileArray[x, y].gameObject;
                    var crrTile = crrTileObj.GetComponent<Tile>();
                    if (crrTile.State == State.None)
                    {
                        continue;
                    }
                    if (processedTiles.Contains(crrTileObj)) { continue; }
                    if (crrTileObj.gameObject == null) { continue; }

                    foreach (var other in Board.TileArray)
                    {
                        if (other == crrTileObj) { continue; }
                        if (other == null) { continue; }
                        if (other.gameObject == null) { continue; }
                        if (other.State == State.None) { continue; }
                        if (IsMatchingPair(crrTileObj.gameObject, other.gameObject))
                        {
                            matchingList.Add(crrTileObj);
                            matchingList.Add(other.gameObject);

                            processedTiles.Add(crrTileObj);
                            processedTiles.Add(other.gameObject);

                            //? State変更処理 いるか確認
                        }
                        else
                        {
                            //? State変更処理 いるか確認
                        }
                    }

                }
            }

            // ! fixme 色変更メソッドの再構築
            if (matchingList.Count > 0) 
            {
                foreach (var item in matchingList)
                {
                    if(item != null) 
                    {
                        var tileColor = item.GetComponent<Image>().color;
                        tileColor.r = 0.5f;
                        item.GetComponent<Image>().color = tileColor;
                        await ResetColor(item.gameObject);
                    }

                }

                
            }

            

        }

        private async UniTask ResetColor(GameObject tile)
        {
            var tileColor = tile.GetComponent<Image>().color;
            await UniTask.Delay(2000);
            tileColor.r = 1.0f;
            
        }

    }

        
}


