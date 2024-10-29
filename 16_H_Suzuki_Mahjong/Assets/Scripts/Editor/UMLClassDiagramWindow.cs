using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using uPalette.Runtime.Core;
using uPalette.Runtime.Core.Model;
using uPalette.Runtime.Core.Synchronizer.Color;

public class UMLClassDiagramWindow : EditorWindow
{
    private List<UMLClass> classes = new List<UMLClass>();
    private bool IsDragging = false;
    private UMLClass selectedClass;

    [MenuItem("Tools/ UML ClassDiagram")]
    public static void ShowWindow()
    {
        GetWindow<UMLClassDiagramWindow>("UMLクラス図");
    }

    private void OnGUI()
    {
        foreach(var umlClass in classes) 
        {
            DrawClassDiagram(umlClass);
        }

        HandleInput();

        if(Event.current.type == EventType.ContextClick) 
        {
            Vector2 mousePos = Event.current.mousePosition;
            ShowContextMenu(mousePos);
            Event.current.Use();
        }

        
    }

    private void DrawClassDiagram(UMLClass umlClass) 
    {
        // ウインドウカラーを設定
        Color originalColor = GUI.backgroundColor;
        GUI.backgroundColor = umlClass.windowColor;

        GUILayout.BeginArea(umlClass.classRect, GUI.skin.box);
        GUILayout.Label(umlClass.className,EditorStyles.boldLabel);

        // 変数の描画
        foreach( var attribute in umlClass.attributes) 
        {
            GUILayout.Label(attribute);
        }

        //メソッドの描画
        foreach( var method in umlClass.methods) 
        {
            GUILayout.Label(method);
        }

        GUILayout.EndArea();
    }
    // 右クリックでメニュ表示
    private void ShowContextMenu(Vector2 mousePos) 
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("クラス図を追加"), false, () => AddClass(mousePos));

        UMLClass clickedClass = GetClassAtPositon(mousePos);
        if(clickedClass != null) 
        {
            menu.AddItem(new GUIContent("Change Color (uPalette)"), false, () => OpenUPaletteForClass(clickedClass));
        }

        menu.ShowAsContext();
    }

    private void AddClass(Vector2 mousePos) 
    {
        UMLClass newClass = new UMLClass();
        newClass.classRect.position = mousePos;
        classes.Add(newClass);
    }

    private UMLClass GetClassAtPositon(Vector2 mousePos) 
    {
        foreach (var umlClass in classes) 
        {
            if (umlClass.classRect.Contains(mousePos)) 
            {
                return umlClass;
            }
        }

        return null;
    }

    private void OpenUPaletteForClass(UMLClass clickedClass)
    {
        //ColorPalette palette = ColorPaletteRegistry

        //if (palette != null) 
        //{
        
        //}
    }

    // マウス入力の処理

    private void HandleInput() 
    {
        if(Event.current.type == EventType.MouseDown && Event.current.button == 0) 
        {
            foreach( var umlclass in classes) 
            {
                if (umlclass.classRect.Contains(Event.current.mousePosition)) 
                {
                    IsDragging = true;
                    selectedClass = umlclass;
                    break;
                }
            }
        }

        if(Event.current.type == EventType.MouseDrag && IsDragging) 
        {
            if( selectedClass != null) 
            {
                selectedClass.classRect.position += Event.current.delta;
                Repaint();
            }
        }

        if(Event.current.type == EventType.MouseUp) 
        {
            IsDragging = false;
            selectedClass = null;
        }
    }

    
}

[System.Serializable]
public class UMLClass
{
    public string className = "NewClass";
    public string[] attributes = new string[] { "attributte1" };
    public string[] methods = new string[] { "method1()" };
    public Rect classRect = new Rect(10, 10, 200, 100);

    [SerializeField]
    private ColorSynchronizer _colorSynchronizer;
    public Color windowColor;

    public Color GetWindowColor() 
    {
        if(_colorSynchronizer != null) 
        {
            //return _colorSynchronizer.colo
        }

        return Color.cyan;
    }
}
