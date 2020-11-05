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
    public static Rect Position;

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
            var scriptable = Utils.GetTalkScriptable();
            EditorUtility.SetDirty(scriptable);

            _pageNavigator = new PageNavigator(scriptable.Container);

            _pageNavigator.PushPage<TalkGroupsPage>();
        }
    }

    public void OnGUI()
    {
        Init();

        Position = new Rect(position.x + Screen.width / 2, position.y + Screen.height / 2, position.width, position.height);

        _pageNavigator.OnGUI();
    }
}
