using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using TalkSystem.Editor;
using TalkSystem;

public class TalkEditorWindow : EditorWindow
{
    private PageNavigator _pageNavigator;
    public static Rect Position;

    [MenuItem("Window/TalkEditor")]
    private static void Open()
    {
        var window = GetWindow<TalkEditorWindow>();

        window.titleContent = new GUIContent("Talk Editor");

        window.Show();
    }

    private void Init()
    {
        if (_pageNavigator == null)
        {
            _pageNavigator = new PageNavigator();

            _pageNavigator.PushPage<TalkGroups>();
        }
    }

    public void OnGUI()
    {
        Init();

        Position = new Rect(position.x + Screen.width / 2, position.y + Screen.height / 2, position.width, position.height);

        _pageNavigator.OnGUI();
    }  
}
