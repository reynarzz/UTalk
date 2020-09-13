using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace TalkSystem.Editor
{
    public class EditPageText : IPage
    {
        public delegate void TextChanged(string text, int charsAdded, int cursor);

        private string _text;
        private int _oldCursorIndex;
        private (string, int) _selectedWord;

        private GUIStyle _labelStyle;

        private TalkData _talkData;

        private int _textPageIndex = -1;

        public event TextChanged OnTextChanged;

        private TextPage _currentTextPage;

        public EditPageText()
        {
            OnTextChanged += OnTextChangedHandler;
        }

        public void OnGUI(TalkData talkData)
        {
            _talkData = talkData;

            if (_textPageIndex >= 0)
            {
                Init();

                GUILayout.Space(10);

                var textEditor = GUIUtils.TextArea(ref _text);

                if (textEditor.TextLengthChanged)
                {
                    OnTextChanged(textEditor.Text, textEditor.AddedChars, textEditor.CursorIndex);
                }

                if (_oldCursorIndex != textEditor.CursorIndex)
                {
                    _oldCursorIndex = textEditor.CursorIndex;

                    var word = GetSelectedWord(_text, textEditor.CursorIndex);

                    if (!string.IsNullOrEmpty(word.Item1))
                    {
                        _selectedWord = word;

                        //Debug.Log(word);
                    }
                }

                GUILayout.Space(5);

                if (!string.IsNullOrEmpty(_selectedWord.Item1))
                {
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.Toggle(false, GUILayout.MaxWidth(17));

                    GUILayout.Label($"Highlight Options ({_selectedWord.Item1})");

                    GUILayout.EndHorizontal();

                    GUILayout.BeginVertical(EditorStyles.helpBox);

                    var startingCharIndex = Highlight.GetStartingCharIndex(_text, _selectedWord.Item2);

                    var containsKey = _currentTextPage.Highlight.ContainsKey(startingCharIndex);

                    var highlight = default(Highlight);

                    if (containsKey)
                    {
                        highlight = _currentTextPage.Highlight[startingCharIndex];
                    }
                    else
                    {
                        _currentTextPage.Highlight.Add(startingCharIndex, highlight);
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Color", GUILayout.MaxWidth(60));
                    var color = EditorGUILayout.ColorField(highlight.Color);
                    color.a = 1;
                    GUILayout.EndHorizontal();

                    highlight = new Highlight(_selectedWord.Item2, _selectedWord.Item1, color, HighlightAnimation.None);

                    _currentTextPage.Highlight[startingCharIndex] = highlight;

                    GUILayout.EndVertical();
                }

                var page = new TextPage(_text, _currentTextPage.Sprite, _currentTextPage.Event, _currentTextPage.Highlight);

                var hightligted = HighlightText(page);

                GUILayout.Space(5);
                TextPreview(hightligted);
            }
        }

        //private void
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

            GUILayout.BeginVertical(EditorStyles.helpBox);


            GUILayout.Label(text, _labelStyle);
            GUILayout.EndVertical();
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

        public void SetTextPageIndex(int textPageIndex)
        {
            _textPageIndex = textPageIndex;

            _currentTextPage = _talkData.GetPage(_textPageIndex);

            _text = _currentTextPage.Text;
        }

        //This is a text.
        //TODO: detect if a modified text index was changed.
        private void OnTextChangedHandler(string newText, int addedChars, int cursor)
        {
            //basically a word have to be like a pointer, all the properties you put to a word can't be losed if a word chage it's index.
            var oldText = _currentTextPage.Text;

            var wasAdded = addedChars > 0;

            if (wasAdded)
            {
                if (cursor == newText.Length)
                {
                    Debug.Log("Added to end");
                }
                else
                {
                    var insertedIndex = cursor - addedChars;
                    var textInserted = newText.Substring(cursor - addedChars, addedChars);

                    if (_currentTextPage.Highlight.Count > 0)
                    {
                        //_currentTextPage.Highlight[];
                    }

                    Debug.Log($"Inserted \"{textInserted}\" in: " + insertedIndex);
                }
            }
            else
            {
                Debug.Log("Was removed: " + addedChars);
            }

            if (wasAdded)
            {
                var splited = Regex.Split(oldText, " |\n");

                //Array.Exists(_currentTextPage.Highlight, x => x.
            }
            else
            {

            }

            //_currentTextPage.Highlight;

            //_currentTextPage = new TextPage(newText, );
        }

        private string HighlightText(TextPage page)
        {
            var modifiedText = page.Text;

            var splited = Regex.Split(modifiedText, " |\n");

            for (int i = 0; i < page.Highlight.Count; i++)
            {
                var highlight = page.Highlight[page.Highlight.Keys.ElementAt(i)];

                var startIndex = Highlight.GetStartingCharIndex(modifiedText, highlight.WordIndex);

                var hex = ColorUtility.ToHtmlStringRGBA(highlight.Color);

                var insertColor = $"<color=#{hex}>";

                //this can be improve it
                modifiedText = modifiedText.Insert(startIndex, insertColor)
                                           .Insert(startIndex + splited[highlight.WordIndex].Length + insertColor.Length, $"</color>");

                //var unmmodified = splited[highlight.WordIndex];

                //unmmodified = unmmodified.Insert(0, insertColor);
                //unmmodified = unmmodified.Insert(unmmodified.Length - 1, "</color>");

                ////this can be improve it
                //modifiedText = modifiedText.Insert(startIndex, insertColor);
                //modifiedText = modifiedText.Insert(startIndex + splited[highlight.WordIndex].Length + insertColor.Length, $"</color>");

                //splited[highlight.WordIndex] = unmmodified;
            }

            return modifiedText;
        }
    }
}
