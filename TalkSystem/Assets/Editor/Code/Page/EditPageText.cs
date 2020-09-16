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
        private (int, string) _selectedWord;

        private GUIStyle _labelStyle;

        private TalkData _talkData;

        private int _textPageIndex = -1;

        public event TextChanged OnTextChanged;

        private TextPage _currentTextPage;
        private StringBuilder _highlightedText;
        private class Clipboard
        {
            private Dictionary<Highlight, string> _buffer;

            public Clipboard()
            {
                _buffer = new Dictionary<Highlight, string>();
            }

            public void SetToClipBoard(TextPage page, string fullText, string copiedText, int cursor)
            {
                var startChar = cursor;

                Debug.Log(startChar);

                var splitWords = Regex.Split(fullText, " |\n");

                var matches = Regex.Matches(copiedText, " |\n");

                var moreThanOneWord = matches.Count > 0;

                if (moreThanOneWord)
                {
                    foreach (Match match in matches)
                    {
                        var cursorInWord = match.Index - 1;

                        var wordIndex = Highlight.GetWordIndex(fullText, cursorInWord);

                        Debug.Log("Word Index: " + wordIndex);
                    }
                }
                else
                {

                }

            }
        }

        private Clipboard _clipboard;

        public EditPageText()
        {
            _highlightedText = new StringBuilder();
            _clipboard = new Clipboard();

            OnTextChanged += OnTextChangedHandler;
        }


        public void OnGUI(TalkData talkData)
        {
            _talkData = talkData;

            if (_textPageIndex >= 0)
            {
                Init();

                GUILayout.Space(10);

                var textInfo = GUIUtils.TextArea(ref _text, SetToClipboard);

                if (textInfo.TextLengthChanged)
                {
                    OnTextChanged(textInfo.Text, textInfo.AddedChars, textInfo.CursorIndex);
                }

                GUILayout.Space(5);

                UpdateColor(textInfo);

                var page = new TextPage(_text, _currentTextPage.Sprite, _currentTextPage.Event, _currentTextPage.Highlight);

                var hightligted = HighlightText(page);

                GUILayout.Space(5);
                TextPreview(hightligted);

                if (GUILayout.Button("Show highlight info"))
                {
                    var orderedKeys = _currentTextPage.Highlight.Keys.OrderBy(x => x);

                    for (int i = 0; i < _currentTextPage.Highlight.Count; i++)
                    {
                        var key = orderedKeys.ElementAt(i);
                        var highlight = _currentTextPage.Highlight[key];

                        Debug.Log("Key: " + key + ", charIndex: " + highlight.WordCharIndex + ", wordIndex: " + Highlight.GetWordIndex(_text, highlight.WordCharIndex));
                    }
                }
            }
        }

        private void SetToClipboard(GUIUtils.TextOperation operation, string inclipboard, int cursor)
        {
            switch (operation)
            {
                case GUIUtils.TextOperation.Copy:
                case GUIUtils.TextOperation.Cut:
                    _clipboard.SetToClipBoard(_currentTextPage, _text, inclipboard, cursor);
                    break;
                case GUIUtils.TextOperation.Paste:
                    break;
            }
        }

        private void UpdateColor(GUIUtils.TextEditorInfo textInfo)
        {
            if (_oldCursorIndex != textInfo.CursorIndex)
            {
                _oldCursorIndex = textInfo.CursorIndex;

                var word = Highlight.GetWordIndex(_text, textInfo.CursorIndex);

                if (!string.IsNullOrEmpty(word.Item2))
                {
                    _selectedWord = word;

                    //Debug.Log(word);
                }
            }

            //does the TextArea have text?
            if (!string.IsNullOrEmpty(_selectedWord.Item2))
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.Toggle(false, GUILayout.MaxWidth(17));

                GUILayout.Label($"Highlight Options ({textInfo.SelectedText + ", " + _selectedWord.Item1})");

                GUILayout.EndHorizontal();

                GUILayout.BeginVertical(EditorStyles.helpBox);

                var startingCharIndex = Highlight.GetStartingCharIndex(_text, _selectedWord.Item1);

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

                highlight = new Highlight(startingCharIndex, _selectedWord.Item2.Length, color, HighlightAnimation.None);

                _currentTextPage.Highlight[startingCharIndex] = highlight;

                GUILayout.EndVertical();
            }
        }

        private void Init()
        {
            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle(GUI.skin.label);
                _labelStyle.active.textColor = Color.white;
                _labelStyle.normal.textColor = Color.white;
                _labelStyle.richText = true;
                _labelStyle.wordWrap = true;
            }
        }

        private void TextPreview(string text)
        {
            GUILayout.Label("Preview");

            GUILayout.BeginVertical(EditorStyles.helpBox);


            GUILayout.Label(text, _labelStyle);
            GUILayout.EndVertical();
        }



        public void SetTextPageIndex(int textPageIndex)
        {
            _textPageIndex = textPageIndex;

            _currentTextPage = _talkData.GetPage(_textPageIndex);

            _text = _currentTextPage.Text;
        }

        //TODO: detect if a modified text index was changed.
        private void OnTextChangedHandler(string newText, int addedChars, int cursor)
        {
            //basically a word have to be like a pointer, all the properties you put to a word can't be losed if a word chage it's index.
            //var oldText = _currentTextPage.Text;

            //var wasAdded = addedChars > 0;

            var insertedIndex = cursor - addedChars;

            //if (wasAdded)
            //{
            //    //Added to end of text.
            //    if (cursor == newText.Length)
            //    {
            //        Debug.Log("Added to end");
            //    }
            //    else //Inserted in a part of the text.
            //    {
            //        var textInserted = newText.Substring(cursor - addedChars, addedChars);

            //        //var notModifableText = oldText.Substring(0, insertedIndex);
            //        //var textToModify = oldText.Substring(insertedIndex, oldText.Length - insertedIndex);

            //        RearrangeHighlightedWords(addedChars, insertedIndex);


            //        Debug.Log($"Inserted \"{textInserted}\" in: " + insertedIndex + ", Recreated");
            //    }
            //}
            //else
            //{
            //    Debug.Log("Was removed: " + addedChars);

            //    RearrangeHighlightedWords(addedChars, insertedIndex);
            //}

            RearrangeHighlightedWords(addedChars, insertedIndex);

            _currentTextPage = new TextPage(newText, _currentTextPage.Highlight);
        }

        //TODO: if you mix two words, there should be a way to clean up.
        //Always clean: repeated charIndex of highlights, and charIndexes that points to empty chars.
        //Two problems: Clean up of unnuced highlights and word duplicates with different charIndex in the highlight object.
        private void RearrangeHighlightedWords(int addedChars, int insertedIndex)
        {
            if (_currentTextPage.Highlight.Count > 0)
            {
                var highlightKeysToModify = _currentTextPage.Highlight.Keys.Where(x => x >= insertedIndex);

                for (int i = 0; i < highlightKeysToModify.Count(); i++)
                {
                    var key = highlightKeysToModify.ElementAt(i);

                    var newKey = key + addedChars;

                    var highlight = _currentTextPage.Highlight[key];

                    highlight = new Highlight(newKey, highlight.WordLength, highlight.Color, highlight.Type);

                    _currentTextPage.Highlight.Remove(key);

                    if (_currentTextPage.Highlight.ContainsKey(newKey))
                    {
                        Debug.Log("replace key");
                        _currentTextPage.Highlight[newKey] = highlight;
                    }
                    else
                    {
                        _currentTextPage.Highlight.Add(newKey, highlight);
                    }
                }

                //Debug.Log("Count: " + highlightKeysToModify.Count());
            }
        }

        private string HighlightText(TextPage page)
        {
            _highlightedText.Clear();
            _highlightedText.Append(page.Text);

            var splited = Regex.Split(_highlightedText.ToString(), " |\n");

            for (int i = 0; i < page.Highlight.Count; i++)
            {
                var key = page.Highlight.Keys.ElementAt(i);

                //Get a highlight
                var highlight = page.Highlight[key];

                //Get the wordIndex where the highligh is
                var wordIndex = Highlight.GetWordIndex(_highlightedText.ToString(), highlight.WordCharIndex).Item1;

                var hex = ColorUtility.ToHtmlStringRGBA(highlight.Color);

                var colorOpen = $"<color=#{hex}>";
                var colorClose = "</color>";

                var unmmodified = splited[wordIndex];

                unmmodified = unmmodified.Insert(0, colorOpen);
                unmmodified = unmmodified.Insert(unmmodified.Length, colorClose);

                splited[wordIndex] = unmmodified;
            }

            _highlightedText.Clear();

            for (int i = 0; i < splited.Length; i++)
            {
                _highlightedText.Append(splited[i]);
                _highlightedText.Append(" ");

            }

            return _highlightedText.ToString();
        }
    }
}
