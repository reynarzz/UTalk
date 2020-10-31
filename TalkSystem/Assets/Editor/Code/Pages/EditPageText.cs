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
        public delegate void TextChanged(string oldText, string newText, int charsAdded, int cursor);

        private string _text;

        private GUIStyle _labelStyle;

        private TalkData _talkData;

        private int _textPageIndex = -1;

        public event TextChanged OnTextChanged;

        private TextPage _currentTextPage;
        private StringBuilder _highlightedText;


        private SDictionary<int, Highlight> _temp;
        private GUIUtils.TextEditorInfo _textInfo;

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
            }
        }

        public void OnGUI(TalkData talkData)
        {
            _talkData = talkData;

            if (_textPageIndex >= 0)
            {
                GUILayout.Space(10);

                var oldText = _text.ToString();

                _textInfo = GUIUtils.TextArea(ref _text, SetToClipboard);

                if (_textInfo.TextLengthChanged)
                {
                    OnTextChanged(oldText, _textInfo.Text, _textInfo.AddedChars, _textInfo.CursorIndex);
                }

                //var selected = Utils.GetSelectedWords(textInfo.StartSelectIndex, textInfo.SelectedText, textInfo.Text);
                //selected.Print();
                GUILayout.Space(5);

                var pair = Utils.GetWordIndexPair(_text, _textInfo.CursorIndex);

                //Debug.Log(pair);

                UpdateHighlight(_textInfo);

                var hightligted = HighlightText(new TextPage(_text, _currentTextPage.Sprite, _currentTextPage.Event, _currentTextPage.Highlight));

                GUILayout.Space(5);

                TextPreview(hightligted);

                //TEST
                if (GUILayout.Button("Show highlight info"))
                {
                    var orderedKeys = _currentTextPage.Highlight.Keys.OrderBy(x => x);
                    //Debug.Log("Show");
                    for (int i = 0; i < _currentTextPage.Highlight.Count; i++)
                    {
                        var key = orderedKeys.ElementAt(i);
                        var highlight = _currentTextPage.Highlight[key];

                        Debug.Log("wordIndex: " + highlight.WordIndex + ", startChar: " + highlight.StartLocalChar + ", " + "length: " + highlight.HighlighLength);
                    }
                }
            }
        }

        private void SetToClipboard(GUIUtils.TextOperation operation, string clipboardText, int selectIndex, int cursor)
        {
            switch (operation)
            {
                case GUIUtils.TextOperation.Copy:
                case GUIUtils.TextOperation.Cut:
                    Debug.Log("Cut");
                    TalkEditorClipboard.SetToClipBoard(_currentTextPage, _text, clipboardText, selectIndex, cursor);
                    break;
                case GUIUtils.TextOperation.Paste:
                    Debug.Log("Paste: " + clipboardText);
                    break;
            }
        }

        private void UpdateHighlight(GUIUtils.TextEditorInfo textInfo)
        {
            //does the TextArea have text?
            if (!string.IsNullOrWhiteSpace(textInfo.SelectedText))
            {
                var selectedWords = Utils.GetSelectedWords(textInfo.StartSelectIndex, textInfo.SelectedText, textInfo.Text);

                var word = selectedWords[0].Word;
                var wordIndex = selectedWords[0].WordIndex;
                var globalStartingChar = selectedWords[0].GlobalCharIndex;//Utils.GetStartingCharIndexRaw(textInfo.Text, wordIndex);

                var containsKey = _currentTextPage.Highlight.ContainsKey(wordIndex);

                GUILayout.BeginVertical(EditorStyles.helpBox);

                var highlight = default(Highlight);
                //Debug.Log("Starting char created: " + startingChar);
                var localStartCharIndex = Utils.ToLocalStartingChar(globalStartingChar, textInfo.Text, word);
                var highlightLength = textInfo.SelectedText.Length;

                //Debug.Log("startChar: " + startingChar + ", local start: " + startCharIndex + ", length " + highlightLength);

                if (containsKey)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
                    {
                        _currentTextPage.Highlight.Remove(wordIndex);

                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();

                        return;
                    }

                    GUILayout.Label($"Highlight ({textInfo.SelectedText})", _labelStyle);

                    GUILayout.EndHorizontal();

                    highlight = _currentTextPage.Highlight[wordIndex];
                }
                else if (GUILayout.Button($"Add Hightlight to: {word}"))
                {
                    highlight = new Highlight(wordIndex, localStartCharIndex, highlightLength, Color.white, HighlightAnimation.None);

                    _currentTextPage.Highlight.Add(wordIndex, highlight);
                }

                if (highlight != default)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Color", GUILayout.MaxWidth(70));
                    var color = EditorGUILayout.ColorField(highlight.Color);
                    color.a = 1;
                    GUILayout.EndHorizontal();

                    GUILayout.Space(3);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Animation", GUILayout.MaxWidth(70));
                    var type = (HighlightAnimation)EditorGUILayout.EnumPopup(highlight.Type);
                    GUILayout.EndHorizontal();

                    highlight = new Highlight(wordIndex, localStartCharIndex, highlightLength, color, type);

                    _currentTextPage.Highlight[wordIndex] = highlight;
                }

                GUILayout.EndVertical();
            }
        }

        private void TextPreview(string text)
        {
            GUILayout.Label("Preview");

            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label(text, _labelStyle);
            GUILayout.EndVertical();
        }

        public void SetTextPageIndex(int textPageIndex, TalkData talkData)
        {
            _textPageIndex = textPageIndex;

            _currentTextPage = talkData.GetPage(_textPageIndex);

            _text = _currentTextPage.Text;
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

                _currentTextPage.Highlight.Add(index, new Highlight(index, highlight.StartLocalChar, highlight.HighlighLength, highlight.Color, highlight.Type));
            }
        }
          
        private void OnWordRemoved(int charIndex, int removedCount, string oldText)
        {
            _temp.Clear();

            //WARNING: removing characters with the "delete" keyword will have problems using "search left".
            //I need here to be exact (using left), so i will not be ending deleting a wrong word highlight.
          
            var wIndexes = Utils.GetSelectedWords(_textInfo.TextEditor.StartSelectIndexLate, _textInfo.TextEditor.SelectedTextLate, oldText);
            //wIndexes.Print(); 
            for (int i = 0; i < wIndexes.Count; i++)
            {
                var wIndex = wIndexes[i].WordIndex;
                 
                if (_currentTextPage.Highlight.ContainsKey(wIndex))
                {
                    Debug.Log("Selection removed: " + wIndexes[i].Word + " | "+ (wIndex));
                    _currentTextPage.Highlight.Remove(wIndex);
                }
            }

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
                    _currentTextPage.Highlight.Add(index, new Highlight(index, highlight.StartLocalChar, highlight.HighlighLength, highlight.Color, highlight.Type));
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

        //RULES:
        //1-if you add a char in a word and is before the start char, the startChar index should be added 1 and length remain the same.
        //1-if you add a char in a word and is after the start char, the startChar index will remain the same bu the length will be added one.
        //asd
        //TODO: if you mix two words, there should be a way to clean up.
        //Always clean: repeated charIndex of highlights, and charIndexes that points to empty chars.
        //Two problems: Clean up of unnuced highlights and word duplicates with different charIndex in the highlight object.
        private void RearrangeHighlightedWords(int wordIndex, int addedChars, int insertedIndex)
        {
            //Get the word index and modify it

            if (_currentTextPage.Highlight.Count > 0)
            {
                var highlightKeysToModify = _currentTextPage.Highlight.Values.Where(x => x.WordIndex >= wordIndex);

                for (int i = 0; i < highlightKeysToModify.Count(); i++)
                {
                    var highlight = highlightKeysToModify.ElementAt(i);

                    //Debug.Log("Indexes after: " + highlight.WordIndex);

                    //var newKey = key + addedChars;

                    //var highlight = _currentTextPage.Highlight[key];

                    ////--highlight = new Highlight(newKey, highlight.WordLength, highlight.Color, highlight.Type);

                    //_currentTextPage.Highlight.Remove(key);

                    //if (_currentTextPage.Highlight.ContainsKey(newKey))
                    //{
                    //    Debug.Log("replace key");
                    //    _currentTextPage.Highlight[newKey] = highlight;
                    //}
                    //else
                    //{
                    //    _currentTextPage.Highlight.Add(newKey, highlight);
                    //}
                }

                //Debug.Log("Count: " + highlightKeysToModify.Count());
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
                        page.Highlight[wordIndex] = new Highlight(hightlight.WordIndex, hightlight.StartLocalChar, length, hightlight.Color, hightlight.Type);

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
