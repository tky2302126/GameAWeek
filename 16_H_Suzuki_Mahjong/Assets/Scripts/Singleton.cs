using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // シーンから既存のインスタンスを検索
                _instance = FindObjectOfType<T>();

                // シーン上にインスタンスがない場合は新しく作成
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                    DontDestroyOnLoad(singletonObject);  // シーン遷移時に破棄されないようにする
                }
            }
            return _instance;
        }
    }

    // インスタンスが重複しないようにチェック
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(this.gameObject); // シーンが切り替わっても残る
        }
        else
        {
            Destroy(gameObject); // 重複したインスタンスを破棄
        }
    }
}
