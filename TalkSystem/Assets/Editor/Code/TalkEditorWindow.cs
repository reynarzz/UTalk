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
    public static Rect _position;

    [MenuItem("Window/TalkEditor")]
    private static void Open()
    {
        var window = GetWindow<TalkEditorWindow>();

        window.titleContent = new GUIContent("Talk Editor");

        window.Show();
    }

    public void OnGUI()
    {
        Init();

        _pageNavigator.OnGUI();
    }

    private void Update()
    {
        _position = new Rect(position.x + Screen.width / 2, position.y + Screen.height / 2, position.width, position.height);
    }

    private void Init()
    {
        if (_pageNavigator == null)
        {
            _pageNavigator = new PageNavigator();

            _pageNavigator.PushPage<TalkGroups>();
        }
    }
}
