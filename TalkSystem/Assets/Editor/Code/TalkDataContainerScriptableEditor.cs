using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using TalkSystem.Editor;
using TalkSystem;

public class TalkDataContainerScriptableEditor : EditorWindow
{
    private string _text;
    private int _currentCursor;
    private string _selectedWord;

    private bool _showWordOptions = false;

    private void OnEnable()
    {

    }

    [MenuItem("Window/TalkEditor")]
    private static void Open()
    {
        var window = GetWindow<TalkDataContainerScriptableEditor>();

        window.Show();
    }

    private Color32 _hightlightColor;
    private string _text2;

    public void OnGUI()
    {
        GUILayout.Space(10);

        var textEditor = GUIUtils.TextArea(ref _text);

        var textEditor2 = GUIUtils.TextArea(ref _text2);

        if (_currentCursor != textEditor.cursorIndex)
        {
            _currentCursor = textEditor.cursorIndex;

            var word = GetSelectedWord(_text, textEditor.cursorIndex);

            if (!string.IsNullOrEmpty(word))
            {
                _selectedWord = word;

                _showWordOptions = false;
                //Debug.Log(word);
            }
        }

        if (!string.IsNullOrEmpty(_selectedWord))
        {
            if (GUILayout.Button(_selectedWord))
            {
                _showWordOptions = true;
            }

            if (_showWordOptions)
            {
                _hightlightColor = EditorGUILayout.ColorField("Hightlight", _hightlightColor);
            }
        }
    }

    private void TextPreview(TextPage page)
    {

    }

    //very inefficient
    private string GetSelectedWord(string text, int cursor)
    {
        var explit = Regex.Split(text, " |\n");
        var charCount = 0;

        var word = "";

        for (int i = 0; i < explit.Length; i++)
        {
            for (int j = 0; j < explit[i].Length; j++)
            {
                if (charCount == cursor)
                {
                    return explit[i];
                }

                charCount++;
            }

            charCount++;
        }

        return word;
    }
}
