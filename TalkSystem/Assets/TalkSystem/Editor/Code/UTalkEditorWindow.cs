using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using uTalk.Editor;

public class UTalkEditorWindow : EditorWindow
{
    private PageNavigator _pageNavigator;
    private TalkFileManager _talkManager;

    public static Rect Position;

    private static uTalk.TalkDataContainerScriptable _scriptable;

    private IPage[] _mainPages;
    private string[] _mainPageNames;

    [MenuItem("Window/TalkEditor")]
    private static void Open()
    {
        var window = GetWindow<UTalkEditorWindow>();

        window.titleContent = new GUIContent("Utalk");
        window.minSize = new Vector2(275, 275);
        window.Show();
    }
     
    private void Init()
    {
        if (_pageNavigator == null)
        {
            _scriptable = Utils.GetTalkScriptable();
            //EditorUtility.SetDirty(_scriptable);

            _pageNavigator = new PageNavigator(_scriptable, _scriptable.CurrentPageState);
            _talkManager = new TalkFileManager(_scriptable.Container);

            _mainPages = new IPage[] { _pageNavigator/*, _talkManager*/ };

            _mainPageNames = _mainPages.Select(x => x.NavigationName).ToArray();

            //Undo.undoRedoPerformed += () =>
            //{
            //    Debug.Log("undo performed");
            //};
        }
    }
    
    public void OnGUI()
    {
        Init();

        Position = new Rect(position.x + Screen.width / 2, position.y + Screen.height / 2, position.width, position.height);

        //--_scriptable.CategoryIndex = GUILayout.Toolbar(_scriptable.CategoryIndex, _mainPageNames);
        GUILayout.Space(5);
        _mainPages[_scriptable.CategoryIndex].OnGUI();

        Repaint();
    }  
    
    /// <summary>Notify changes to unity after manipulating the data.</summary>
    public static void SetDirtyAndSave()
    {
        EditorUtility.SetDirty(_scriptable);
        AssetDatabase.SaveAssets();
    }

    public static void RecordToUndo(string name)
    {
        EditorUtility.SetDirty(_scriptable);

        Undo.SetCurrentGroupName(name);

        Undo.RecordObject(_scriptable, _scriptable.name);
    }
}
