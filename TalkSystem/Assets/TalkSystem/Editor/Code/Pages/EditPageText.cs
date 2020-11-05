//MIT License

//Copyright (c) 2020 Reynardo Perez (Reynarz)

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace TalkSystem.Editor
{
    //This class needs a huge refactorization.
    public class EditPageText : IPage
    {
        public delegate void TextChanged(string oldText, string newText, int charsAdded, int cursor);

        private GUIStyle _labelStyle;
        private GUIStyle _buttonWrapStyle;

        private GUIStyle _centeredLabel;
        private GUIStyle _prevNextButtons;
        private GUIStyle _prevNextButtonsDisabled;


        private int _textPageIndex = 0;

        public event TextChanged OnTextChanged;

        private TextPage _currentTextPage;
        private StringBuilder _highlightedText;


        private SDictionary<int, Highlight> _temp;
        private GUIUtils.TextEditorInfo _textInfo;
        private Color32 _backgroundColor = new Color(0, 0, 0, 0.2f);

        public string NavigationName { get; set; }

        public EditPageText()
        {
            _temp = new SDictionary<int, Highlight>();

            _highlightedText = new StringBuilder();

            Init();

            OnTextChanged += OnTextChangedHandler;
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
                _labelStyle.alignment = TextAnchor.UpperLeft;

                _buttonWrapStyle = new GUIStyle(GUI.skin.button);
                _buttonWrapStyle.wordWrap = true;

                _centeredLabel = new GUIStyle(GUI.skin.label);
                _centeredLabel.alignment = TextAnchor.MiddleCenter;
                _centeredLabel.fontSize = 15;
                _centeredLabel.normal.textColor = Color.yellow;

                _prevNextButtons = new GUIStyle(GUI.skin.button);
                _prevNextButtons.margin.left = 30;
                _prevNextButtons.margin.right = 30;

                _prevNextButtonsDisabled = new GUIStyle(EditorStyles.helpBox);
                _prevNextButtonsDisabled.margin.left = 30;
                _prevNextButtonsDisabled.margin.right = 30;
                _prevNextButtonsDisabled.alignment = TextAnchor.MiddleCenter;
                _prevNextButtonsDisabled.fontSize = _prevNextButtons.fontSize;

                _disabledButton = new GUIStyle(EditorStyles.helpBox);
                _disabledButton.alignment = TextAnchor.MiddleCenter;
                _disabledButton.fontSize = GUI.skin.button.fontSize;
            }
        }

        public void SetCurrentTalkData(TalkData talkData)
        {
            _talkData = talkData;

            if (_textInfo.TextEditor != null)
            {
                _textInfo.TextEditor.SelectNone();
                //_textInfo.TextEditor.cursorIndex = _textInfo.TextEditor.text.Length;
            }

            _textInfo = default;

            SetTextPageIndex(0, talkData);
        }

        private void SetTextPageIndex(int textPageIndex, TalkData talkData)
        {
            _textPageIndex = textPageIndex;

            _currentTextPage = talkData.GetPage(_textPageIndex);
        }

        private Vector2 _entireScroll;

        public void OnGUI()
        {
            if (_textPageIndex >= 0)
            {
                GUILayout.Space(5);

                // _entireScroll = GUILayout.BeginScrollView(_entireScroll);
                //here i'm creating a new text page instance!! (TextPage was an struct before, but now this need a refactor!!!)
                var hightligted = HighlightText(new TextPage(_currentTextPage.Text, _currentTextPage.Sprites, _currentTextPage.Event, _currentTextPage.Highlight));
                AddRemovePageToolbar();

               



                var oldText = _currentTextPage.Text.ToString();

                var text = oldText.ToString();

                _textInfo = GUIUtils.SmartTextArea(ref text, SetToClipboard, GUILayout.MinHeight(100));

               
                TextPreview(hightligted);

                _currentTextPage.Text = text;

                if (_textInfo.TextLengthChanged)
                {
                    OnTextChanged(oldText, _textInfo.Text, _textInfo.AddedChars, _textInfo.CursorIndex);
                }

                UpdateHighlight(_textInfo);

                GUILayout.Space(5);
                PagesToolBar();

                GUILayout.Space(5);

                //var selected = Utils.GetSelectedWords(textInfo.StartSelectIndex, textInfo.SelectedText, textInfo.Text);
                //selected.Print();
                //GUILayout.Space(5);

                

                PageOptions();


                //GUILayout.EndScrollView();

                //TEST
                //if (_showInfo = EditorGUILayout.Foldout(_showInfo, "Highlight Info"))
                //{
                //    var orderedKeys = _currentTextPage.Highlight.Keys.OrderBy(x => x);

                //    GUILayout.BeginVertical(EditorStyles.helpBox);

                //    //Debug.Log("Show");
                //    for (int i = 0; i < _currentTextPage.Highlight.Count; i++)
                //    {
                //        var key = orderedKeys.ElementAt(i);
                //        var highlight = _currentTextPage.Highlight[key];

                //        GUILayout.Label("Index: " + highlight.WordIndex + ", Char: " + highlight.StartLocalChar + ", " + "Length: " + highlight.HighlighLength);
                //    }

                //    GUILayout.EndVertical();
                //}
            }
        }

        private void PagesToolBar()
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            var nextSkin = _textPageIndex + 1 < _talkData.PagesCount ? _prevNextButtons : _prevNextButtonsDisabled;
            var prevSkin = _textPageIndex > 0 ? _prevNextButtons : _prevNextButtonsDisabled;
            
            GUI.SetNextControlName("Prev");
            if (GUILayout.Button("Prev", prevSkin) && _textPageIndex > 0)
            {
                GUI.FocusControl("Prev");
                
                _textPageIndex--;
                _textInfo.TextEditor.ClearSelectedText();

                _currentTextPage = _talkData.GetPage(_textPageIndex);
            }

            GUILayout.Label((_textPageIndex + 1).ToString() + "/" + _talkData.PagesCount, _centeredLabel, GUILayout.Width(40));

            GUI.SetNextControlName("Next");
            if (GUILayout.Button("Next", nextSkin) && _textPageIndex + 1 < _talkData.PagesCount)
            {
                GUI.FocusControl("Next");

                _textPageIndex++;
                _textInfo.TextEditor.ClearSelectedText();
                _currentTextPage = _talkData.GetPage(_textPageIndex);
            }
            GUILayout.EndHorizontal();
        }

        private void AddRemovePageToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            if (GUILayout.Button("Add Page"))
            {
                _talkData.CreateEmptyPageWithLastPageOptions();
                _textPageIndex = _talkData.PagesCount - 1;
                _currentTextPage = _talkData.GetPage(_textPageIndex);
            }

            var buttonSkin = _talkData.PagesCount > 1 ? GUI.skin.button : _disabledButton;

            if (GUILayout.Button("Delete This", buttonSkin))
            {
                if (_talkData.PagesCount > 1)
                {
                    _talkData.DeletePage(_textPageIndex);

                    if (_textPageIndex - 1 > -1)
                        _textPageIndex--;
                    _currentTextPage = _talkData.GetPage(_textPageIndex);
                }
            }
            GUILayout.EndHorizontal();
        }

        private void SetToClipboard(GUIUtils.TextOperation operation, string clipboardText, int selectIndex, int cursor)
        {
            switch (operation)
            {
                case GUIUtils.TextOperation.Copy:
                case GUIUtils.TextOperation.Cut:
                    Debug.Log("Cut");
                    TalkEditorClipboard.SetToClipBoard(_currentTextPage, _currentTextPage.Text, clipboardText, selectIndex, cursor);
                    break;
                case GUIUtils.TextOperation.Paste:
                    Debug.Log("Paste: " + clipboardText);
                    break;
            }
        }

        private Vector2 _scrollView;
        private Vector2 _textScroll;
        private TalkData _talkData;
        private GUIStyle _disabledButton;

        private void UpdateHighlight(GUIUtils.TextEditorInfo textInfo)
        {
            //does the TextArea have text?
            if (!string.IsNullOrWhiteSpace(textInfo.SelectedText))
            {
                var selectedWords = Utils.GetSelectedWords(textInfo.StartSelectIndex, textInfo.SelectedText, textInfo.Text);

                var wordsToHighlight = "";

                for (int i = 0; i < selectedWords.Count; i++)
                {
                    if (!_currentTextPage.Highlight.ContainsKey(selectedWords[i].WordIndex))
                    {
                        wordsToHighlight += selectedWords[i].Word + (i + 1 == selectedWords.Count ? null : " | ");
                    }
                }

                if (!string.IsNullOrEmpty(wordsToHighlight) && GUILayout.Button($"Add Hightlight to: {wordsToHighlight}", _buttonWrapStyle))
                {
                    for (int i = 0; i < selectedWords.Count; i++)
                    {
                        if (!_currentTextPage.Highlight.ContainsKey(selectedWords[i].WordIndex))
                        {
                            var word = selectedWords[i].Word;
                            var wordIndex = selectedWords[i].WordIndex;
                            var globalStartingChar = selectedWords[i].GlobalCharIndex;
                            var localStartCharIndex = Utils.ToLocalStartChar(globalStartingChar, textInfo.Text, word);
                            var highlightLength = textInfo.SelectedText.Length;

                            var highlight = new Highlight(wordIndex, localStartCharIndex, highlightLength, Color.white);

                            _currentTextPage.Highlight.Add(wordIndex, highlight);
                        }
                    }
                }

                GUILayout.BeginVertical(EditorStyles.helpBox);
                //GUILayout.Label("Highlights");

                GUILayout.Space(4);
                _scrollView = GUILayout.BeginScrollView(_scrollView, true, false, GUI.skin.horizontalScrollbar, GUIStyle.none, GUILayout.MinHeight(130));

                GUILayout.BeginHorizontal();

                for (int i = 0; i < selectedWords.Count; i++)
                {
                    var word = selectedWords[i].Word;
                    var wordIndex = selectedWords[i].WordIndex;
                    var globalStartingChar = selectedWords[i].GlobalCharIndex;

                    var containsKey = _currentTextPage.Highlight.ContainsKey(wordIndex);

                    //Debug.Log("Starting char created: " + startingChar);
                    var localStartCharIndex = Utils.ToLocalStartChar(globalStartingChar, textInfo.Text, word);
                    var highlightLength = textInfo.SelectedText.Length;

                    //Debug.Log("startChar: " + startingChar + ", local start: " + startCharIndex + ", length " + highlightLength);

                    if (containsKey)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(150));

                        GUILayout.Space(1);

                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
                        {
                            _currentTextPage.Highlight.Remove(wordIndex);

                            GUILayout.EndHorizontal();
                            GUILayout.EndVertical();

                            return;
                        }

                        GUILayout.Label($"{word}", _labelStyle);

                        GUILayout.EndHorizontal();

                        var highlight = _currentTextPage.Highlight[wordIndex];

                        if (highlight != default)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Color", GUILayout.MaxWidth(70));
                            var color = EditorGUILayout.ColorField(highlight.Color);
                            color.a = 1;
                            GUILayout.EndHorizontal();

                            GUILayout.Space(3);

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Anim", GUILayout.MaxWidth(70));
                            var type = (TextAnimation)EditorGUILayout.EnumPopup(highlight.Type);
                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Write Speed", GUILayout.MaxWidth(80));
                            var writeSpeedType = (Highlight.WriteSpeed)EditorGUILayout.EnumPopup(highlight.WriteSpeedType);
                            GUILayout.EndHorizontal();

                            var normalWriteSpeed = 0f;

                            if (writeSpeedType == Highlight.WriteSpeed.Custom)
                            {
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Speed", GUILayout.MaxWidth(70));
                                normalWriteSpeed = EditorGUILayout.FloatField(highlight.NormalWriteSpeed);
                                GUILayout.EndHorizontal();
                            }

                            highlight = new Highlight(wordIndex, localStartCharIndex, highlightLength, color, type, writeSpeedType, normalWriteSpeed);

                            _currentTextPage.Highlight[wordIndex] = highlight;
                        }

                        GUILayout.EndVertical();
                        GUILayout.Space(4);

                    }
                }

                GUILayout.EndHorizontal();
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
        }

        private void TextPreview(string text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Preview");
            //_backgroundColor = EditorGUILayout.ColorField(_backgroundColor, GUILayout.MaxWidth(100));

            GUILayout.EndHorizontal();

            var rect = GUILayoutUtility.GetRect(Screen.width, 100, EditorStyles.helpBox);

            EditorGUI.DrawRect(rect, _backgroundColor);

            GUI.Label(rect, text, _labelStyle);
        }

        private void PageOptions()
        {
            GUILayout.Label("Page Options");

            GUILayout.BeginVertical(EditorStyles.helpBox);
            _currentTextPage.TalkerName = EditorGUILayout.TextField("Talker Name", _currentTextPage.TalkerName);
            EditorGUILayout.Separator();
            _currentTextPage.WriteType = (WriteType)EditorGUILayout.EnumPopup("Write Type", _currentTextPage.WriteType);

            switch (_currentTextPage.WriteType)
            {
                case WriteType.Instant:
                    _currentTextPage.CharByCharInfo = default;
                    InstantInfoPageOpt();
                    break;
                case WriteType.CharByChar:
                    _currentTextPage.InstantInfo = default;
                    CharByCharPageOpt();
                    break;
            }

            EditorGUILayout.Separator();

            SpritesOption();

            GUILayout.EndVertical();
        }

        private Vector2 _spritesScroll;

        private void SpritesOption()
        {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Sprites", GUILayout.MaxWidth(135));
            GUILayout.Space(10);
            if (GUILayout.Button("Add"))
            {
                _currentTextPage.Sprites.Add(default);
            }

            GUILayout.EndHorizontal();


            if(_currentTextPage.Sprites.Count > 0)
            {
                GUILayout.Space(5);

                _spritesScroll = GUILayout.BeginScrollView(_spritesScroll, false, false, GUI.skin.horizontalScrollbar, GUIStyle.none, EditorStyles.helpBox, GUILayout.Height(100));
                _spritesScroll.y = 0;

                GUILayout.BeginHorizontal();

                for (int i = 0; i < _currentTextPage.Sprites.Count; i++)
                {
                    GUILayout.BeginVertical(GUILayout.MaxWidth(60));

                    //Sprite index
                    //GUILayout.Label(i.ToString(), _centeredLabel, GUILayout.MaxWidth(20));

                    if (GUILayout.Button("Remove", GUILayout.MaxWidth(60)))
                    {
                        _currentTextPage.Sprites.RemoveAt(i);
                        break;
                    }

                    _currentTextPage.Sprites[i] = (Sprite)EditorGUILayout.ObjectField("", _currentTextPage.Sprites[i], typeof(Sprite), false, GUILayout.MaxWidth(60));

                    GUILayout.EndVertical();
                }

                GUILayout.EndHorizontal();
                GUILayout.EndScrollView();
            }
            
            GUILayout.EndVertical();
        }

        private void InstantInfoPageOpt()
        {
            var instantInfo = _currentTextPage.InstantInfo;

            instantInfo.TextAnimation = (TextAnimation)EditorGUILayout.EnumPopup("Animation", instantInfo.TextAnimation);

            _currentTextPage.InstantInfo = instantInfo;
        }

        private void CharByCharPageOpt()
        {
            var charByCharWriteInfo = _currentTextPage.CharByCharInfo;

            charByCharWriteInfo.AnimationType = (CharByCharInfo.CharByCharAnimation)EditorGUILayout.EnumPopup("Animation", charByCharWriteInfo.AnimationType);

            switch (charByCharWriteInfo.AnimationType)
            {
                case CharByCharInfo.CharByCharAnimation.None:
                    charByCharWriteInfo.Offset = 0;
                    charByCharWriteInfo.OffsetType = default;
                    break;
                case CharByCharInfo.CharByCharAnimation.OffsetToPos:
                    charByCharWriteInfo.OffsetType = (CharByCharInfo.OffsetStartPos)EditorGUILayout.EnumPopup("Start Pos", charByCharWriteInfo.OffsetType);
                    charByCharWriteInfo.Offset = EditorGUILayout.FloatField("Offset", charByCharWriteInfo.Offset);
                    break;
            }

            charByCharWriteInfo.NormalWriteSpeed = EditorGUILayout.FloatField("Normal write delay ", charByCharWriteInfo.NormalWriteSpeed);
            charByCharWriteInfo.FastWriteSpeed = EditorGUILayout.FloatField("Fast write delay ", charByCharWriteInfo.FastWriteSpeed);

            _currentTextPage.CharByCharInfo = charByCharWriteInfo;
        }

        //TODO: Detect if a modified text index was changed.
        private void OnTextChangedHandler(string oldText, string newText, int addedChars, int cursor)
        {
            var charAdded = addedChars > 0;

            //i have to adjust the cursor to support multi whitespaces and tabs.
            var insertedIndex = cursor - addedChars;
            //removes duplicated white spaces.
            //oldText = Regex.Replace(oldText, @"\s+", " ");
            //newText = Regex.Replace(newText, @"\s+", " ");

            if (charAdded)
            {
                //char added.
                OnCharAdded(insertedIndex, cursor != newText.Length);

                //word added.
            }
            else
            {
                OnCharRemoved(insertedIndex, addedChars, newText, cursor);

            }

            var old = oldText.Split(Utils.SplitPattern, StringSplitOptions.RemoveEmptyEntries);
            var newSplit = newText.Split(Utils.SplitPattern, StringSplitOptions.RemoveEmptyEntries);

            var wordAdded = newSplit.Length > old.Length;

            if (wordAdded)
            {
                OnWordAdded(insertedIndex, Utils.GetChangedWordsCount(oldText, newText), newText);
            }
            else if (newSplit.Length < old.Length)
            {
                OnWordRemoved(insertedIndex, Utils.GetChangedWordsCount(oldText, newText), oldText);
            }
        }

        private void OnWordAdded(int charIndex, int wordsAdded, string newText)
        {
            var wordIndex = Utils.GetWordIndex(newText, charIndex);

            _temp.Clear();

            //inefficient
            for (int i = 0; i < _currentTextPage.Highlight.Count; i++)
            {
                var key = _currentTextPage.Highlight.Keys.ElementAt(i);

                _temp.Add(key, _currentTextPage.Highlight[key]);
            }

            _currentTextPage.Highlight.Clear();

            for (int i = 0; i < _temp.Count; i++)
            {
                var highlight = _temp[_temp.Keys.ElementAt(i)];

                var index = highlight.WordIndex >= wordIndex ? highlight.WordIndex + wordsAdded : highlight.WordIndex;

                _currentTextPage.Highlight.Add(index, new Highlight(index, highlight.StartLocalChar,
                    highlight.HighlighLength, highlight.Color, highlight.Type, highlight.WriteSpeedType, highlight.NormalWriteSpeed));
            }
        }

        private void OnWordRemoved(int charIndex, int removedCount, string oldText)
        {
            _temp.Clear();

            if (!string.IsNullOrEmpty(_textInfo.TextEditor.SelectedTextLate))
            {
                var wIndexes = Utils.GetSelectedWords(_textInfo.TextEditor.StartSelectIndexLate, _textInfo.TextEditor.SelectedTextLate, oldText);
                //wIndexes.Print();
                for (int i = 0; i < wIndexes.Count; i++)
                {
                    var wIndex = wIndexes[i].WordIndex;

                    if (_currentTextPage.Highlight.ContainsKey(wIndex))
                    {
                        Debug.Log("Selection removed: " + wIndexes[i].Word + " (" + (wIndex) + ")");
                        _currentTextPage.Highlight.Remove(wIndex);
                    }
                }
            }

            //WARNING: removing characters with the "delete" keyword will have problems using "search left".
            //I need here to be exact (using left), so i will not be ending deleting a wrong word highlight.
            var wexactIndex = Utils.GetWordIndex(oldText, charIndex, Utils.SearchCharType.Left);

            for (int i = 0; i < removedCount; i++)
            {
                if (_currentTextPage.Highlight.ContainsKey(wexactIndex + i))
                {
                    Debug.Log("By Char: " + (wexactIndex + i));
                    _currentTextPage.Highlight.Remove(wexactIndex + i);
                }
            }

            //inefficient 
            for (int i = 0; i < _currentTextPage.Highlight.Count; i++)
            {
                var key = _currentTextPage.Highlight.Keys.ElementAt(i);

                _temp.Add(key, _currentTextPage.Highlight[key]);
            }

            _currentTextPage.Highlight.Clear();

            var pair = Utils.GetWordIndexPair(oldText, charIndex);
            var wordIndex = pair.Item1;

            //Debug.Log("WIndex: " + wordIndex + ", removed: " + removedCount + ", word: " + pair.Item2);

            for (int i = 0; i < _temp.Count; i++)
            {
                var highlight = _temp[_temp.Keys.ElementAt(i)];

                var index = highlight.WordIndex >= wordIndex ? highlight.WordIndex - removedCount : highlight.WordIndex;

                //If you are mixing two highligted words, for now the created word will have the highligh values of the first one.
                if (!_currentTextPage.Highlight.ContainsKey(index))
                {
                    _currentTextPage.Highlight.Add(index, new Highlight(index, highlight.StartLocalChar,
                                                   highlight.HighlighLength, highlight.Color, highlight.Type, highlight.WriteSpeedType, highlight.NormalWriteSpeed));
                }
            }
        }

        private void OnWordSplitted()
        {

        }

        private void OnCharAdded(int charIndex, bool inserted)
        {
            if (inserted) //inserted in text
            {
                //if (!string.IsNullOrEmpty(indexValue.Item2) && _currentTextPage.Highlight.ContainsKey(indexValue.Item1))
                //{
                //    var startingIndex = Utils.GetStartingCharIndex(newText, indexValue.Item1);
                //    Debug.Log(startingIndex);
                //    var highlight = _currentTextPage.Highlight[indexValue.Item1];

                //    if(startingIndex + highlight.HighlighLength > cursor)
                //    {
                //        _currentTextPage.Highlight[indexValue.Item1] = new Highlight(highlight.WordIndex, highlight.WordStartCharIndex, 
                //                                                                     highlight.HighlighLength + addedChars, highlight.Color);
                //    } 
                //}
            }
            else //added at the end of the text
            {

            }
        }

        private void OnCharRemoved(int charIndex, int addedChars, string newText, int cursor)
        {
            //-Debug.Log("Was removed: " + addedChars);
            var lastChar = newText.ElementAtOrDefault(cursor - 1);

            if (addedChars < 0 && (char.IsWhiteSpace(lastChar) || lastChar == '\0'))
            {
                //RemoveDeletedHighlightWords(oldText, newText);

                //Debug.Log("oldText: " + oldText + ", newText: " + newText);
            }
            else //if was remove in the same word
            {
                //if (!string.IsNullOrEmpty(indexValue.Item2) && _currentTextPage.Highlight.ContainsKey(indexValue.Item1))
                //{
                //    var startingIndex = Utils.GetStartingCharIndex(newText, indexValue.Item1);
                //    Debug.Log(startingIndex);
                //    var highlight = _currentTextPage.Highlight[indexValue.Item1];

                //    if (startingIndex + highlight.HighlighLength > cursor)
                //    {
                //        _currentTextPage.Highlight[indexValue.Item1] = new Highlight(highlight.WordIndex, highlight.WordStartCharIndex, 
                //                                                                     highlight.HighlighLength + addedChars, highlight.Color);
                //    }
                //}
            }
        }

        //Remove word index feature, there should not be color by words but color by charStart + wordLength
        private string HighlightText(TextPage page)
        {
            _highlightedText.Clear();

            var splitted = page.Text.Split(Utils.SplitPattern, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < page.Highlight.Count; i++)
            {
                var wordIndex = page.Highlight.Keys.ElementAt(i);

                //Get a highlight.
                var highlight = page.Highlight[wordIndex];

                //Get the wordIndex where the highligh is
                var hex = ColorUtility.ToHtmlStringRGBA(highlight.Color);

                var colorOpen = $"<color=#{hex}>";
                var colorClose = "</color>";
                //Debug.Log(splitted[wordIndex]);

                //Debug.Log("Word: " + splitted[wordIndex] + ", StartChar " + highlight.WordStartCharIndex + ", end: " + highlight.HighlighLength);
                var modified = splitted[wordIndex];

                //Debug.Log("LocalChar: " + highlight.StartLocalChar);
                modified = modified.Insert(highlight.StartLocalChar, colorOpen);

                var insertIndex = colorOpen.Length + highlight.StartLocalChar + highlight.HighlighLength;

                if (modified.Length > insertIndex)
                {
                    modified = modified.Insert(colorOpen.Length + highlight.StartLocalChar + highlight.HighlighLength, colorClose);
                }
                else
                {
                    //Adjust hightlightLength.
                    var hightlight = page.Highlight[wordIndex];

                    var length = splitted[wordIndex].Length - (highlight.StartLocalChar);

                    if (length > 0)
                    {
                        page.Highlight[wordIndex] = new Highlight(hightlight.WordIndex, hightlight.StartLocalChar, length, hightlight.Color, hightlight.Type,
                                                                  highlight.WriteSpeedType, highlight.NormalWriteSpeed);

                        modified = modified.Insert(modified.Length, colorClose);
                    }
                    else
                    {
                        Debug.Log("Nothing to highlight, removed");
                        page.Highlight.Remove(wordIndex);
                        continue;
                    }
                }


                splitted[wordIndex] = modified;
            }

            for (int i = 0; i < splitted.Length; i++)
            {
                _highlightedText.Append(splitted[i] + " ");
            }

            return _highlightedText.ToString();
        }
    }
}
