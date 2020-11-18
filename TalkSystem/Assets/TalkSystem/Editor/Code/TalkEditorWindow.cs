using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using TalkSystem.Editor;

public class TalkEditorWindow : EditorWindow
{
    private PageNavigator _pageNavigator;
    private TalkFileManager _talkManager;

    public static Rect Position;

    private static TalkSystem.TalkDataContainerScriptable _scriptable;

    private IPage[] _mainPages;
    private string[] _mainPageNames;

    [MenuItem("Window/TalkEditor")]
    private static void Open()
    {
        var window = GetWindow<TalkEditorWindow>();

        window.titleContent = new GUIContent("Talk Editor");
        window.minSize = new Vector2(275, 275);
        window.Show();
    }
     
    private void Init()
    {
        if (_pageNavigator == null)
        {
            _scriptable = Utils.GetTalkScriptable();
            EditorUtility.SetDirty(_scriptable);

            _pageNavigator = new PageNavigator(_scriptable.Container, _scriptable.CurrentPageState);
            _talkManager = new TalkFileManager(_scriptable.Container);

            _mainPages = new IPage[] { _pageNavigator, _talkManager };

            _mainPageNames = _mainPages.Select(x => x.NavigationName).ToArray();
        }
    }
     
    public void OnGUI()
    {
        Init();

        Position = new Rect(position.x + Screen.width / 2, position.y + Screen.height / 2, position.width, position.height);

        _scriptable.CategoryIndex = GUILayout.Toolbar(_scriptable.CategoryIndex, _mainPageNames);
        GUILayout.Space(5);
        _mainPages[_scriptable.CategoryIndex].OnGUI();

        Repaint();
    }  
          
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
