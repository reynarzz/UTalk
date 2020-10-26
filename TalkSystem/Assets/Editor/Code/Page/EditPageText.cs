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

                        //Debug.Log("Key: " + key + ", wordIndex: " + highlight.WordIndex + ", startChar: " + highlight.WordStartCharIndex + ", " + "length: " + highlight.HighlighLength);
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
                var pair = Highlight.GetWordIndex(textInfo.Text, textInfo.StartSelectIndex);

                var wordText = pair.Item2;
                var wordIndex = pair.Item1;

                var containsKey = _currentTextPage.Highlight.ContainsKey(wordIndex);

                GUILayout.BeginVertical(EditorStyles.helpBox);

                var highlight = default(Highlight);
                var startingChar = Highlight.GetStartingCharIndex(textInfo.Text, wordIndex);

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

        //TODO: Detect if a modified text index was changed.
        private void OnTextChangedHandler(string oldText, string newText, int addedChars, int cursor)
        {
            //basically a word have to be like a pointer, all the properties you put to a word can't be losed if a word chage it's index.
            //var oldText = _currentTextPage.Text;

            //Detect when a word was added: a word is a text that have white spaces around

            var wasAdded = addedChars > 0;

            var insertedIndex = cursor - addedChars;

            var wordIndex = Highlight.GetWordIndex(newText, insertedIndex).Item1;

            if (wasAdded)
            {
                //Added to end of text.
                if (cursor == newText.Length)
                {
                    //-Debug.Log("Added to end");
                }
                else //Inserted in a part of the text.
                {
                    var textInserted = newText.Substring(cursor - addedChars, addedChars);

                    //var notModifableText = oldText.Substring(0, insertedIndex);
                    //var textToModify = oldText.Substring(insertedIndex, oldText.Length - insertedIndex);

                    RearrangeHighlightedWords(wordIndex, addedChars, insertedIndex);
                    //--Debug.Log($"Inserted \"{textInserted}\" in: " + insertedIndex + ", Recreated");
                } 
            }
            else
            {
                //-Debug.Log("Was removed: " + addedChars);
                var wordDeleted = addedChars < 0 && newText.ElementAtOrDefault(cursor - 1) == ' ';

                if (wordDeleted)
                {
                    RemoveDeletedHighlightWords(oldText, newText);

                    Debug.Log("Word deleted");

                    Debug.Log("oldText: " + oldText + ", newText: " + newText);


                }
                 
                RearrangeHighlightedWords(wordIndex, addedChars, insertedIndex);
            }

            //RearrangeHighlightedWords(addedChars, insertedIndex);
            _currentTextPage = new TextPage(newText, _currentTextPage.Highlight);
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

                modified = modified.Insert(highlight.WordStartCharIndex, colorOpen);

                var insertIndex = colorOpen.Length + highlight.WordStartCharIndex + highlight.HighlighLength;

                if (modified.Length > insertIndex)
                {
                    modified = modified.Insert(colorOpen.Length + highlight.WordStartCharIndex + highlight.HighlighLength, colorClose);
                }
                else
                { 
                    //Adjust hightlightLength.
                    var hightlight = page.Highlight[wordIndex];

                    var length =  splitted[wordIndex].Length - (highlight.WordStartCharIndex);
                    
                    page.Highlight[wordIndex] = new Highlight(hightlight.WordIndex, hightlight.WordStartCharIndex, length, hightlight.Color);

                    modified = modified.Insert(modified.Length, colorClose);
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
