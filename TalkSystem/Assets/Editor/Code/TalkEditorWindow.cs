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
    private Home _homePage;

    private TalkData _test;

    [MenuItem("Window/TalkEditor")]
    private static void Open()
    {
        var window = GetWindow<TalkEditorWindow>();

        window.titleContent = new GUIContent("Talk Editor");

        window.Show();
    }

    private bool _pageSet = false;

    public static Rect Position;

    public void OnGUI()
    {
        Position = new Rect(position.x + Screen.width / 2, position.y + Screen.height / 2, position.width, position.height);
        Init();

        _homePage.OnGUI();

        //_editPageText.OnGUI(_test);
    }  
    
    private void Init()
    {
        if (_editPageText == null)
        {
            _homePage = new Home(null);

            _test = new TalkData();
            _pageSet = false;
            var text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas varius ligula ac dui \nermentum, sed finibus tortor aliquam.ni";

            _test.AddPage(new TextPage(text, new SDictionary<int, Highlight> { { 1, new Highlight(1, 1, 3, Color.green) },
                                                                               { 8, new Highlight(8, 0, 8, Color.yellow) },
                                                                               { 16, new Highlight(16, 0, 6, Color.red) }}));
            _editPageText = new EditPageText();
               
            if (!_pageSet)  
            {
                _pageSet = true;
                 
                _editPageText.SetTextPageIndex(0, _test);
            }
        }
    }

}
