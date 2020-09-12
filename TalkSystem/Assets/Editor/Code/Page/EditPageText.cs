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
        private Color32 _hightlightColor;

        public delegate void TextChanged(string text, int charsAdded, int cursor);

        private string _text;
        private int _oldCursorIndex;
        private (string, int) _selectedWord;

        private bool _showWordOptions = false;
        private GUIStyle _labelStyle;

        private TalkData _talkData;

        private int _textPageIndex = 0;

        public event TextChanged OnTextChanged;

        private TextPage _currentTextPage;

        public EditPageText()
        {
            OnTextChanged += OnTextChangedHandler;
        }

        public void OnGUI(TalkData talkData)
        {
            _talkData = talkData;

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

                //var word = GetSelectedWord(_text, textEditor.cursorIndex);

                //if (!string.IsNullOrEmpty(word.Item1))
                //{
                //    _selectedWord = word;

                //    _showWordOptions = false;
                //    //Debug.Log(word);
                //}
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

                    page = new TextPage(_text, new Highlight(_selectedWord.Item2, _selectedWord.Item1, _hightlightColor, HighlightAnimation.None));
                }
            }

            var hightligted = HighlightText(page);

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

                ProcessWords(_talkData.GetPage(0));
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

        private void ProcessWords(TextPage page)
        {
            HighlightText(page);
        }

        public void SetTextPageIndex(int textPageIndex)
        {
            _textPageIndex = textPageIndex;

            _currentTextPage = _talkData.GetPage(_textPageIndex);
        }

        //This is a text, 
        //TODO: detect if a modified text index was changed.
        private void OnTextChangedHandler(string newText, int addedChars, int cursor)
        {
            var oldText = _currentTextPage.Text;

            var wasAdded = addedChars > 0;

            Debug.Log("was added: " + wasAdded + ", how much: " + addedChars);

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

            return page.Text;
        }
    }
}
