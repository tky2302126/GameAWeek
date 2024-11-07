using UnityEditor;
using UnityEngine;
using System.IO;


public class CreateClassByMVPMethod : MonoBehaviour
{
    public class ScriptCreateByMVP : EditorWindow
    {
        private string folderPath = "Assets/Script/MVP";
        private string className = "NewClass";

        [MenuItem("Tools/CreateScriptMVP")]
        public static void ShowWindow()
        {
            GetWindow<ScriptCreateByMVP>("スクリプトを作成...");
        }

        private void OnGUI()
        {
            GUILayout.Label("Script Creation Settings", EditorStyles.boldLabel);

            // 現在のフォルダパスを表示
            EditorGUILayout.LabelField("Folder Path", folderPath);

            // フォルダ選択ボタン
            if (GUILayout.Button("Select Folder"))
            {
                string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // Unityプロジェクト内のパスに変換
                    if (selectedPath.StartsWith(Application.dataPath))
                    {
                        folderPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                    }
                    else
                    {
                        Debug.LogWarning("Selected folder is outside the project. Please select a folder inside the Assets directory.");
                    }
                }
            }
            className = EditorGUILayout.TextField("Class Name", className);

            if (GUILayout.Button("Create Script"))
            {
                CreateFolderAndScript(folderPath, className);
            }
        }

        private static void CreateFolderAndScript(string path, string name)
        {
            // フォルダの作成
            if (!AssetDatabase.IsValidFolder(path))
            {
                Directory.CreateDirectory(path);
                AssetDatabase.Refresh();
            }

            string folderPath = Path.Combine(path, name);

            if (!AssetDatabase.IsValidFolder(folderPath)) 
            {
                Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }

            // スクリプトのパスと内容の指定
            #region スクリプトを作成
            #region modelクラス
            // string scriptPath = Path.Combine(folderPath, name + ".cs");
            string modelName = name + "Model";
            string modelPath = Path.Combine(folderPath, modelName + ".cs");
            if (File.Exists(modelPath))
            {
                Debug.LogWarning("The script already exists!");
                return;
            }

            string scriptContentModel = $@"
using System;
using System.Collections.Generic;
using UnityEngine;

public class {modelName} : MonoBehaviour
{{
    // you may write here fields
}}";
            File.WriteAllText(modelPath, scriptContentModel);
            #endregion

            #region プレゼンタークラス
            string presenterName = name + "Presenter";
            string presenterPath = Path.Combine(folderPath, presenterName + ".cs");
            if (File.Exists(presenterPath))
            {
                Debug.LogWarning("The script already exists!");
                return;
            }

            string scriptContentPresenter = $@"
using System;
using System.Collections.Generic;
using UnityEngine;

public class {presenterName} : MonoBehaviour
{{
    // you may write here methods
}}";
            File.WriteAllText(presenterPath, scriptContentPresenter);
            #endregion

            #region ビュークラス
            string viewName = name + "View";
            string viewPath = Path.Combine(folderPath, viewName + ".cs");
            if (File.Exists(viewPath))
            {
                Debug.LogWarning("The script already exists!");
                return;
            }

            string scriptContentView = $@"
using System;
using System.Collections.Generic;
using UnityEngine;

public class {viewName} : MonoBehaviour
{{
    // you may write here view contents
}}";
            File.WriteAllText(viewPath, scriptContentView);
            #endregion

            AssetDatabase.Refresh();
            #endregion


        }
    }
}


