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
    private EditPageText _editPageText;
    private TalkData _test;

    [MenuItem("Window/TalkEditor")]
    private static void Open()
    {
        var window = GetWindow<TalkEditorWindow>();

        window.titleContent = new GUIContent("Talk Editor");

        window.Show();
    }

    private bool _pageSet = false;

    private void OnEnable()
    {

    }

    public void OnGUI()
    {
        Init();

        _editPageText.OnGUI(_test);

        if (!_pageSet)
        {
            _pageSet = true;

            _editPageText.SetTextPageIndex(0);
        }
    }

    private void Init()
    {
        if (_editPageText == null)
        {
            _test = new TalkData();
            _pageSet = false;
            var text = "Lorem ipsum dolor";// sit amet, consectetur adipiscing elit. Maecenas varius ligula ac dui \nermentum, sed finibus tortor aliquam.ni";

            _test.AddPage(new TextPage(text, new SDictionary<int, Highlight> { { 1, new Highlight(1, 1, 3, Color.green) } }));

            _editPageText = new EditPageText();

        }
    }

}
