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


        private Clipboard _clipboard;
        private SDictionary<int, Highlight> _temp;

        public EditPageText()
        {
            _temp = new SDictionary<int, Highlight>();

            _highlightedText = new StringBuilder();
            _clipboard = new Clipboard();

            Init();

            OnTextChanged += OnTextChangedHandler;
        }
         
        public void OnGUI(TalkData talkData)
        {
            _talkData = talkData;

            if (_textPageIndex >= 0)
            { 
                //var returnLoop = Init();

                //if (returnLoop)
                //{
                //    return;
                //}

                GUILayout.Space(10);

                var oldText = _text.ToString();

                var textInfo = GUIUtils.TextArea(ref _text, SetToClipboard);

                if (textInfo.TextLengthChanged)
                {
                    OnTextChanged(oldText, textInfo.Text, textInfo.AddedChars, textInfo.CursorIndex);
                }

                GUILayout.Space(5);

                UpdateHighlight(textInfo);

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

                        Debug.Log("Key: " + key + ", wordIndex: " + highlight.WordIndex + ", startChar: " + highlight.CharIndex + ", " + "length: " + highlight.HighlighLength);
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
                    _clipboard.SetToClipBoard(_currentTextPage, _text, clipboardText, selectIndex, cursor);
                    break;
                case GUIUtils.TextOperation.Paste:
                    break;
            }
        }

        private void UpdateHighlight(GUIUtils.TextEditorInfo textInfo)
        {
            //does the TextArea have text?
            if (!string.IsNullOrWhiteSpace(textInfo.SelectedText))
            {
                //Debug.Log("selectd: " + textInfo.SelectedText);

                //Debug.Log(textInfo.StartSelectIndex);
                var pair = Utils.GetWordIndex(textInfo.Text, textInfo.StartSelectIndex);

                var wordText = pair.Item2;
                var wordIndex = pair.Item1;

                var containsKey = _currentTextPage.Highlight.ContainsKey(wordIndex);

                GUILayout.BeginVertical(EditorStyles.helpBox);

                var highlight = default(Highlight);
                var startingChar = Utils.GetStartingCharIndex(textInfo.Text, wordIndex);

                var startCharIndex = textInfo.StartSelectIndex - startingChar;
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
                else if (GUILayout.Button($"Add Hightlight to: {wordText}"))
                {
                    highlight = new Highlight(wordIndex, startCharIndex, highlightLength, Color.white, HighlightAnimation.None);

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

                    highlight = new Highlight(wordIndex, startCharIndex, highlightLength, color, type);

                    _currentTextPage.Highlight[wordIndex] = highlight;
                }

                GUILayout.EndVertical();
            }
        }

        private bool Init()
        {
            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle(GUI.skin.label);
                _labelStyle.active.textColor = Color.white;
                _labelStyle.normal.textColor = Color.white;
                _labelStyle.richText = true;
                _labelStyle.wordWrap = true;
                return true; 
            }

            return false;
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
                var wordIndex = Utils.GetWordIndex(newText, insertedIndex).Item1;

                OnCharRemoved(insertedIndex, addedChars, newText, cursor);

                RearrangeHighlightedWords(wordIndex, addedChars, insertedIndex);
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
                OnWordRemoved(insertedIndex, Utils.GetChangedWordsCount(oldText, newText), newText);
            }
        }

        private void OnWordAdded(int charIndex, int wordsAdded, string newText)
        {
            var value = Utils.GetWordIndex(newText, charIndex);
            var wordIndex = value.Item1;
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

                _currentTextPage.Highlight.Add(index, new Highlight(index, highlight.CharIndex, highlight.HighlighLength, highlight.Color, highlight.Type));
            }

            Debug.Log("WordCountAdded: " + wordsAdded + ", Word added at char: " + charIndex + ", index: " + wordIndex + ", word: " + value.Item2);
        }

        private void OnWordRemoved(int charIndex, int removedCount, string newText)
        {
            var wordIndex = Utils.GetWordIndex(newText, charIndex).Item1;

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

                var index = highlight.WordIndex >= wordIndex ? highlight.WordIndex - removedCount : highlight.WordIndex;

                _currentTextPage.Highlight.Add(index, new Highlight(index, highlight.CharIndex, highlight.HighlighLength, highlight.Color));
            }

            Debug.Log("wIndex: " + wordIndex + ", removedCount: " + removedCount);
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

        private void RemoveDeletedHighlightWords(string oldText, string newText)
        {
            var oldSplit = oldText.Split(Utils.SplitPattern);
            var newSplit = newText.Split(Utils.SplitPattern);

            for (int i = 0; i < oldSplit.Length; i++)
            {
                var value = newSplit.ElementAtOrDefault(i);

                if (value == null || value != oldSplit[i])
                {
                    Debug.Log("Remove word: " + i);
                    _currentTextPage.Highlight.Remove(i);
                }
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

                modified = modified.Insert(highlight.CharIndex, colorOpen);

                var insertIndex = colorOpen.Length + highlight.CharIndex + highlight.HighlighLength;

                if (modified.Length > insertIndex)
                {
                    modified = modified.Insert(colorOpen.Length + highlight.CharIndex + highlight.HighlighLength, colorClose);
                }
                else
                {
                    //Adjust hightlightLength.
                    var hightlight = page.Highlight[wordIndex];

                    var length = splitted[wordIndex].Length - (highlight.CharIndex);

                    if (length > 0)
                    {
                        page.Highlight[wordIndex] = new Highlight(hightlight.WordIndex, hightlight.CharIndex, length, hightlight.Color);

                        modified = modified.Insert(modified.Length, colorClose);
                    }
                    else
                    {
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
