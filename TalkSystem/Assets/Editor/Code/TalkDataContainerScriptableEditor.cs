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
    private (string, int) _selectedWord;

    private bool _showWordOptions = false;
    private GUIStyle _labelStyle;

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
        Init();

        GUILayout.Space(10);

        var textEditor = GUIUtils.TextArea(ref _text);

        if (_currentCursor != textEditor.cursorIndex)
        {
            _currentCursor = textEditor.cursorIndex;

            var word = GetSelectedWord(_text, textEditor.cursorIndex);

            if (!string.IsNullOrEmpty(word.Item1))
            {
                _selectedWord = word;

                _showWordOptions = false;
                //Debug.Log(word);
            }
        }

        GUILayout.Space(5);

        var page = default(TextPage);

        if (!string.IsNullOrEmpty(_selectedWord.Item1))
        {
            if (GUILayout.Button(_selectedWord.Item1 + $" ({_selectedWord.Item2})"))
            {
                _showWordOptions = true;
            }

            if (_showWordOptions)
            {
                _hightlightColor = EditorGUILayout.ColorField("Hightlight", _hightlightColor);

                page = new TextPage(_text, new Highlight(_selectedWord.Item2, _hightlightColor, HighlightAnimation.None));

                //Debug.Log(hex);
            }
        }

        var hightligted = HighlightText(page, _text);

        GUILayout.Space(5);
        TextPreview(hightligted);
    }

    private void Init()
    {
        if (_labelStyle == null)
        {
            _labelStyle = new GUIStyle(GUI.skin.label);
            _labelStyle.active.textColor = Color.white;
            _labelStyle.normal.textColor = Color.white;
            _labelStyle.richText = true;
        }
    }


    private void TextPreview(string text)
    {
        GUILayout.Label("Preview");

        var color = GUI.color;
        GUI.color = Color.white;
        GUILayout.BeginVertical(EditorStyles.helpBox);
        GUI.color = color;

        GUILayout.Label(text, _labelStyle);
        GUILayout.EndVertical();
    }

    private string HighlightText(TextPage page, string defaultText)
    {
        if (page.Highlight != null)
        {
            var modifiedText = page.Text;
            var splited = Regex.Split(page.Text, " |\n");

            for (int i = 0; i < page.Highlight.Length; i++)
            {
                var highlight = page.Highlight[i];

                var wordIndex = GetWordFullIndex(splited, highlight.WordIndex);

                var hex = ColorUtility.ToHtmlStringRGBA(highlight.Color);

                var insertColor = $"<color=#{hex}>";

                modifiedText = modifiedText.Insert(wordIndex, insertColor);
                modifiedText = modifiedText.Insert(wordIndex + splited[highlight.WordIndex].Length + insertColor.Length, $"</color>");
            }

            //Debug.Log(modifiedText);

            return modifiedText;
        }

        return defaultText;
    }

    private int GetWordFullIndex(string[] words, int index)
    {
        var charCount = 0;

        for (int i = 0; i < words.Length; i++)
        {
            if (words[index] == words[i])
            {
                return charCount;
            }

            charCount += words[i].Length + 1;
        }

        return charCount;
    }

    //very inefficient
    private (string, int) GetSelectedWord(string text, int cursor)
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
                    return (explit[i], i);
                }

                charCount++;
            }

            charCount++;
        }

        return (word, 0);
    }
}
