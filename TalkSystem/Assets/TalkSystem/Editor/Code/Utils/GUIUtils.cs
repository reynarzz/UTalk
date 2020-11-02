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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using UnityEditor;

namespace TalkSystem.Editor
{
    /// <summary>A hack to get all the properties of the text area.</summary>
    public static class GUIUtils
    {
        private static GUIContent _guiContent;
        private static int _cursorIndex;

        private static PropertyInfo _compositionStringProperty;
        private static PropertyInfo _textFieldInput;
        private static FieldInfo _hasFocusProperty;
        private static StringBuilder _text;

        private static string _compositionString => (string)_compositionStringProperty.GetValue(null);

        public delegate void OnOperation(TextOperation operation, string clipboardText, int selectIndex, int cursorIndex);

        public enum TextOperation
        {
            Copy,
            Cut,
            Paste
        }


        public struct TextEditorInfo
        {
            private string _fullText;

            private int _cursorIndex;
            private bool _lengthChanged;
            private int _addedChars;
            private string _selectedText;
            private int _selectIndex;
            public int CursorIndex => _cursorIndex;

            public string Text => _fullText;

            public bool TextLengthChanged => _lengthChanged;
            public int AddedChars => _addedChars;
            public string SelectedText => _selectedText;
            public int StartSelectIndex
            {
                get
                {
                    if (_cursorIndex < _selectIndex)
                    {
                        return _cursorIndex;
                    }
                    else
                    {
                        return _selectIndex;
                    }
                }
            }

            public SmartTextEditor TextEditor { get; private set; }

            public TextEditorInfo(string fullText, string selectedText, int selectIndex, int cursorIndex, int addedChars, bool textLengthChanged, SmartTextEditor textEditor)
            {
                _fullText = fullText;
                _selectedText = selectedText;
                _selectIndex = selectIndex;
                _cursorIndex = cursorIndex;
                _addedChars = addedChars;

                _lengthChanged = textLengthChanged;
                TextEditor = textEditor;
            }
        }

        static GUIUtils()
        {
            _guiContent = new GUIContent();
            _text = new StringBuilder();

            _compositionStringProperty = typeof(GUIUtility).GetProperty("compositionString", BindingFlags.Static | BindingFlags.NonPublic);
            _textFieldInput = typeof(GUIUtility).GetProperty("textFieldInput", BindingFlags.Static | BindingFlags.NonPublic);
            _hasFocusProperty = typeof(SmartTextEditor).GetField("m_HasFocus", BindingFlags.Instance | BindingFlags.NonPublic);

        }

        public static TextEditorInfo SmartTextArea(ref string text, params GUILayoutOption[] options)
        {
            if (text == null)
            {
                text = "";
            }

            return DoTextFieldOrSomething(ref text, true, GUI.skin.textArea, options, null);
        }

        public static TextEditorInfo SmartTextArea(ref string text, OnOperation onOperationCallback, params GUILayoutOption[] options)
        {
            if (text == null)
            {
                text = "";
            }

            return DoTextFieldOrSomething(ref text, true, GUI.skin.textArea, options, onOperationCallback);
        }

        //TODO: Copy paste doesn't work correctly. 
        private static TextEditorInfo DoTextFieldOrSomething(ref string text, bool multiline, GUIStyle style, GUILayoutOption[] options,
                                                             OnOperation onTextInClipboard)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Keyboard);

            style = EditorStyles.helpBox;
            style.fontSize = 12;

            _guiContent.text = text;
            style.richText = true;
            style.wordWrap = false;
            //gUIContent = ((GUIUtility.keyboardControl == controlID) ? _guiContent.text /*+ GUIUtility.compositionString*/ : te);
            var rect = GUILayoutUtility.GetRect(_guiContent, style, options);

            var textEditor = DoTextField(rect, controlID, _guiContent, multiline, -1, style, onTextInClipboard);

            text = textEditor.Text;

            return textEditor;
        }

        private static TextEditorInfo DoTextField(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style,
                                                  OnOperation onTextInClipboard)
        {
            //GUIUtility.CheckOnGUI(); //Checks If is called from OnGUI method, otherwise it will throw an error..

            if (maxLength >= 0 && content.text.Length > maxLength)
            {
                content.text = content.text.Substring(0, maxLength);
            }

            var textEditor = (SmartTextEditor)GUIUtility.GetStateObject(typeof(SmartTextEditor), id);

            textEditor.Clear();

            var charsAdded = 0;
            var pasted = false;

            if (onTextInClipboard != null)
            {
                textEditor.OnCopy += t => onTextInClipboard(TextOperation.Copy, t, textEditor.selectIndex, textEditor.cursorIndex);
                textEditor.OnCut += t => onTextInClipboard(TextOperation.Cut, t, textEditor.selectIndex, textEditor.cursorIndex);
                textEditor.OnPaste += t => { pasted = true; charsAdded = t.Length; };
                //textEditor.OnPaste += t => onTextInClipboard(TextOperation.Paste, t, textEditor.selectIndex, textEditor.cursorIndex);
            }

            textEditor.text = content.text;
            textEditor.SaveBackup();
            textEditor.position = position;
            textEditor.style = style;
            textEditor.multiline = multiline;
            textEditor.controlID = id;
            textEditor.DetectFocusChange();

            var currentlyAdded = HandleTextFieldEventForDesktop(position, id, content, multiline, maxLength, style, textEditor);

            //This "if" avoids overriding charsAdded pasted value (if any)
            if (!pasted)
            {
                charsAdded = currentlyAdded;
            }

            textEditor.UpdateScrollOffsetIfNeeded(Event.current);

            //_text.Clear();
            //_text.Append(textEditor.text);

            ////Maintains the first char of the text white the start of the first word.
            //while (_text.Length > 0 && !_text[0].IsValidChar())
            //{
            //    _text.Remove(0, 1);
            //}

            //textEditor.text = _text.ToString();
            
            return new TextEditorInfo(textEditor.text, textEditor.SelectedText, textEditor.selectIndex, _cursorIndex, charsAdded, charsAdded != 0, textEditor);
        }

        private static int HandleTextFieldEventForDesktop(Rect position, int id, GUIContent content, bool multiline, int maxLength, GUIStyle style, SmartTextEditor editor)
        {
            Event current = Event.current;
            bool flag = false;
            var charsAdded = 0;

            _cursorIndex = editor.cursorIndex;

            switch (current.type)
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition))
                    {
                        GUIUtility.hotControl = id;
                        GUIUtility.keyboardControl = id;
                        _hasFocusProperty.SetValue(editor, true);
                        editor.MoveCursorToPosition(Event.current.mousePosition);
                        if (Event.current.clickCount == 2 && GUI.skin.settings.doubleClickSelectsWord)
                        {
                            editor.SelectCurrentWord();
                            editor.DblClickSnap(TextEditor.DblClickSnapping.WORDS);
                            editor.MouseDragSelectsWholeWords(on: true);
                        }
                        if (Event.current.clickCount == 3 && GUI.skin.settings.tripleClickSelectsLine)
                        {
                            editor.SelectCurrentParagraph();
                            editor.MouseDragSelectsWholeWords(on: true);
                            editor.DblClickSnap(TextEditor.DblClickSnapping.PARAGRAPHS);
                        }
                        current.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        if (current.shift)
                        {
                            editor.MoveCursorToPosition(Event.current.mousePosition);
                        }
                        else
                        {
                            editor.SelectToPosition(Event.current.mousePosition);
                        }

                        current.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id)
                    {
                        editor.MouseDragSelectsWholeWords(on: false);
                        GUIUtility.hotControl = 0;
                        current.Use();
                    }
                    break;
                case EventType.KeyDown:
                    {
                        var originalTextLength = content.text.Length;
                        if (GUIUtility.keyboardControl != id)
                        {
                            return charsAdded;
                        }

                        if (editor.HandleKeyEvent(current))
                        {
                            current.Use();

                            flag = true;
                            content.text = editor.text;

                            if (editor.text.Length != originalTextLength)
                            {
                                if (editor.text.Length < originalTextLength)
                                {
                                    var deleted = originalTextLength - editor.text.Length;

                                    charsAdded = -deleted;
                                }
                                else
                                {
                                    charsAdded = editor.text.Length - originalTextLength;
                                }
                            }

                            break;
                        }
                        if (current.keyCode == KeyCode.Tab || current.character == '\t')
                        {

                            return 0;
                        }

                        char character = current.character;

                        if (character == '\n' && !multiline && !current.alt)
                        {
                            return charsAdded;
                        }

                        Font font = style.font;

                        if (!font)
                        {
                            font = GUI.skin.font;
                        }

                        if (font.HasCharacter(character) || character == '\n')
                        {
                            editor.Insert(character);

                            flag = true;

                            charsAdded++;
                        }
                        else if (character == '\0') //if is null char
                        {
                            if (_compositionString.Length > 0)
                            {
                                editor.ReplaceSelection("");
                                flag = true;
                            }

                            current.Use();
                        }

                        if (editor.text.Length != originalTextLength)
                        {
                            if (editor.text.Length < originalTextLength)
                            {
                                var deleted = originalTextLength - editor.text.Length;

                                charsAdded = -deleted;
                            }
                        }
                        break;
                    }
                case EventType.Repaint:
                    if (GUIUtility.keyboardControl != id)
                    {
                        style.Draw(position, content, id, on: false);
                    }
                    else
                    {
                        editor.DrawCursor(content.text);
                    }

                    break;
            }
            if (GUIUtility.keyboardControl == id)
            {
                _textFieldInput.SetValue(null, true);
            }
            if (flag)
            {
                //changed = true;
                content.text = editor.text;
                if (maxLength >= 0 && content.text.Length > maxLength)
                {
                    content.text = content.text.Substring(0, maxLength);
                }
                current.Use();
            }

            _cursorIndex = editor.cursorIndex;

            return charsAdded;
        }
    }
}
